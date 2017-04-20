using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TQL.Interfaces;

namespace TQL.Threading
{
    public class Timer : TimerBase
    {
        #region Private Variables
        private readonly System.Threading.Timer _timer;
        private KeyValuePair<DateTimeOffset, List<IFireTimeEvaluator>> _timerOccurence;
        private bool _canBeStarted = true;

        private readonly Func<DateTimeOffset> _nowFunc;
        private readonly SortedList<DateTimeOffset, List<IFireTimeEvaluator>> _nextOccurences;

        private readonly object _syncObj = new object();
        private bool _disposed;
        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="evaluators">The evaluators.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="timerCallback">The timer callback.</param>
        public Timer(IReadOnlyCollection<IFireTimeEvaluator> evaluators, CancellationToken token, TimerCallback timerCallback)
            : base(evaluators, token)
        {
            _timer = new System.Threading.Timer((obj) =>
            {
                lock (_syncObj)
                {
                    _timerOccurence.Value.Clear();
                }
                timerCallback(obj);
            });
            _nowFunc = () => DateTimeOffset.Now;
            _nextOccurences = new SortedList<DateTimeOffset, List<IFireTimeEvaluator>>();
            _timerOccurence = new KeyValuePair<DateTimeOffset, List<IFireTimeEvaluator>>(DateTimeOffset.MinValue, new List<IFireTimeEvaluator>());
        }

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="evaluators">The evaluators.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="currentTimeProvider">The current time provider.</param>
        /// <param name="timerCallback">The timer callback.</param>
        public Timer(IReadOnlyCollection<IFireTimeEvaluator> evaluators, CancellationToken token, Func<DateTimeOffset> currentTimeProvider, TimerCallback timerCallback)
            : base(evaluators, token)
        {
            _timer = new System.Threading.Timer((obj) =>
            {
                lock (_syncObj)
                {
                    _timerOccurence.Value.Clear();
                }
                timerCallback(obj);
            });
            _nowFunc = currentTimeProvider;
            _nextOccurences = new SortedList<DateTimeOffset, List<IFireTimeEvaluator>>();
            _timerOccurence = new KeyValuePair<DateTimeOffset, List<IFireTimeEvaluator>>(DateTimeOffset.MinValue, new List<IFireTimeEvaluator>());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public override void StartTick()
        {
            if (!_canBeStarted) throw new NotSupportedException();

            _canBeStarted = false;
            Task.Factory.StartNew(CalculateNextOccurences);
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
                _timer.Dispose();
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
            try
            {
                while (!ShouldStop)
                {

                    await Task.Delay(100);

                    Token.ThrowIfCancellationRequested();

                    if (!HasCurrent())
                    {
                        break;
                    }

                    do
                    {
                        Token.ThrowIfCancellationRequested();

                        if (_nextOccurences.Any(f => f.Value.Contains(Current)))
                            continue;

                        lock (_syncObj)
                        {
                            if (_timerOccurence.Value != null && _timerOccurence.Value.Contains(Current))
                                continue;
                        }

                        var evaluator = Current;
                        var occurence = evaluator.NextFire();

                        while (occurence.HasValue && occurence.Value < GetNow())
                            occurence = evaluator.NextFire();

                        if (occurence.HasValue)
                        {
                            var adjustedDatetime = AdjustDateTime(occurence.Value);
                            if (!_nextOccurences.ContainsKey(adjustedDatetime))
                                _nextOccurences.Add(adjustedDatetime, new List<IFireTimeEvaluator>() { Current });
                            else
                                _nextOccurences[adjustedDatetime].Add(Current);
                        }
                        else
                            Remove();

                    } while (Next());

                    if (_nextOccurences.Count == 0)
                        continue;

                    lock (_syncObj)
                    {
                        var closestCalculatedOccurence = _nextOccurences.First();
                        if (closestCalculatedOccurence.Key < _timerOccurence.Key)
                        {
                            if (!_nextOccurences.ContainsKey(_timerOccurence.Key))
                                _nextOccurences.Add(_timerOccurence.Key, _timerOccurence.Value);
                            else
                                _nextOccurences[_timerOccurence.Key].AddRange(_timerOccurence.Value);

                            _timerOccurence = closestCalculatedOccurence;
                            _nextOccurences.Remove(closestCalculatedOccurence.Key);

                            var span = _timerOccurence.Key - GetNow();

                            _timer.Change(span < TimeSpan.Zero ? TimeSpan.Zero : span, Timeout.InfiniteTimeSpan);
                        }
                        else if (_timerOccurence.Key == DateTimeOffset.MinValue || _timerOccurence.Key < closestCalculatedOccurence.Key)
                        {
                            _timerOccurence = closestCalculatedOccurence;
                            _nextOccurences.Remove(closestCalculatedOccurence.Key);
                            var span = _timerOccurence.Key - GetNow();

                            _timer.Change(span < TimeSpan.Zero ? TimeSpan.Zero : span, Timeout.InfiniteTimeSpan);
                        }
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
            }
            finally
            {
                _timer.Dispose();
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