using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TQL.Threading
{
    public class Timer : TimerBase<IdentifiableEvaluator>
    {
        #region Private Variables
        private bool _canBeStarted = true;
        private readonly TimerCallback _callback;
        private readonly EventWaitHandle _waitHandle;
        private readonly EventWaitHandle _onStoppedWaitHandle;
        private bool _shouldPause;
        private bool _paused;

        private readonly Func<DateTimeOffset> _nowFunc;
        private bool _disposed;
        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="evaluators">The evaluators.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="timerCallback">The timer callback.</param>
        public Timer(IReadOnlyCollection<IdentifiableEvaluator> evaluators, CancellationToken token, TimerCallback timerCallback)
            : base(evaluators, token)
        {
            _callback = timerCallback;
            _nowFunc = () => DateTimeOffset.Now;
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _shouldPause = false;
            _paused = true;
            _onStoppedWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="evaluators">The evaluators.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="currentTimeProvider">The current time provider.</param>
        /// <param name="timerCallback">The timer callback.</param>
        public Timer(IReadOnlyCollection<IdentifiableEvaluator> evaluators, CancellationToken token, Func<DateTimeOffset> currentTimeProvider, TimerCallback timerCallback)
            : base(evaluators, token)
        {
            _callback = timerCallback;
            _nowFunc = currentTimeProvider;
            _waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            _shouldPause = false;
            _paused = true;
            _onStoppedWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine if Timer can be started.
        /// </summary>
        public override bool CanStart => _canBeStarted;

        /// <summary>
        /// Blocks the thread until timer in paused mode
        /// </summary>
        public WaitHandle PausedWaitHandle => _onStoppedWaitHandle;

        /// <summary>
        /// Determine if timer is already paused.
        /// </summary>
        public bool IsPaused => _paused;

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public override void StartTick()
        {
            if (!_canBeStarted) throw new NotSupportedException();

            _canBeStarted = false;
            _paused = false;
            _onStoppedWaitHandle.Reset();
            Task.Factory.StartNew(CalculateNextOccurences);
        }
        
        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public override void ResumeTick()
        {
            _waitHandle.Set();
            _shouldPause = false;
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public override void PauseTick()
        {
            _waitHandle.Reset();
            _shouldPause = true;
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        /// <param name="disposing">The is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            _disposed = true;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Calculates next occurences.
        /// </summary>
        private async void CalculateNextOccurences()
        {
            var queue = new ConcurrentQueue<IdentifiableEvaluator>();
            var sortedOccurencesSync = new object();
            var sortedOccurences = new SortedList<DateTimeOffset, List<IdentifiableEvaluator>>();

            do
            {
                if (!HasCurrent())
                    break;

                queue.Enqueue(Current);
            } while (Next());
            Reset();

            try
            {
                while (!ShouldStop)
                {
                    if (_shouldPause)
                    {
                        _paused = true;
                        _onStoppedWaitHandle.Set();
                        _waitHandle.WaitOne(TimeSpan.FromSeconds(1));

                        foreach (var occurence in sortedOccurences.ToList())
                        {
                            var diff = GetNow() - occurence.Key;

                            if (diff <= TimeSpan.FromSeconds(1))
                                continue;

                            foreach (var evaluator in occurence.Value)
                            {
                                queue.Enqueue(evaluator);
                            }
                            sortedOccurences.Remove(occurence.Key);
                        }

                        do
                        {
                            if(!HasCurrent())
                                break;

                            var shouldEvaluatorBeQueued = sortedOccurences.Any(occurence => !occurence.Value.Contains(Current));

                            var isEvaluatorQueued = queue.Contains(Current);

                            if(!shouldEvaluatorBeQueued && !isEvaluatorQueued)
                                queue.Enqueue(Current);

                        } while (Next());
                        Reset();

                        continue;
                    }
                    _paused = false;
                    _onStoppedWaitHandle.Reset();

                    Parallel.ForEach(new DequeueCustomEnumerator(queue), (evaluator) =>
                    {
                        var occurence = evaluator.NextFire();
                        while (occurence.HasValue && occurence.Value < GetNow())
                            occurence = evaluator.NextFire();

                        if (!occurence.HasValue)
                            return;

                        var adjustedDatetime = AdjustDateTime(occurence.Value);
                        lock (sortedOccurencesSync)
                        {
                            if (!sortedOccurences.ContainsKey(adjustedDatetime))
                                sortedOccurences.Add(adjustedDatetime, new List<IdentifiableEvaluator>() { evaluator });
                            else
                                sortedOccurences[adjustedDatetime].Add(evaluator);
                        }
                    });

                    if(sortedOccurences.Count == 0)
                        continue;

                    var nearestOccurence = sortedOccurences.ElementAt(0);
                    var span = nearestOccurence.Key - GetNow();

                    if (span > TimeSpan.FromMilliseconds(-500))
                    {
                        var waitPeriod = span < TimeSpan.Zero ? TimeSpan.Zero : span;

                        if (waitPeriod > TimeSpan.FromMilliseconds(20))
                            await Task.Delay(waitPeriod);

                        if (!_shouldPause)
                            await Task.Factory.StartNew(() => _callback(nearestOccurence.Key, nearestOccurence.Value));
                    }

                    sortedOccurences.RemoveAt(0);

                    foreach (var item in nearestOccurence.Value)
                        queue.Enqueue(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Gets current time.
        /// </summary>
        /// <returns></returns>
        private DateTimeOffset GetNow() => _nowFunc();

        /// <summary>
        /// Adjusts the time.
        /// </summary>
        /// <param name="datetime">Time variable to adjust.</param>
        /// <returns>Adjusted time.</returns>
        private static DateTimeOffset AdjustDateTime(DateTimeOffset datetime)
        {
            return new DateTimeOffset(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Offset);
        }

        #endregion
    }
}