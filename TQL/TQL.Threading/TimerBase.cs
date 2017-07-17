using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TQL.Interfaces;

namespace TQL.Threading
{
    public abstract class TimerBase<TIdentifiableEvaluator>:  MarshalByRefObject, IDisposable
        where TIdentifiableEvaluator: IFireTimeEvaluator, IKey
    {
        #region Private variables
        private readonly IReadOnlyCollection<TIdentifiableEvaluator> _evaluators;
        private int _evaluatorIndex;

        protected bool ShouldStop { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="evaluators">The evaluators.</param>
        /// <param name="token">The cancellation token.</param>
        protected TimerBase(IReadOnlyCollection<TIdentifiableEvaluator> evaluators, CancellationToken token)
        {
            _evaluators = evaluators;
            _evaluatorIndex = 0;
            Token = token;
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTick()
        {
            ShouldStop = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determine if Timer can be started.
        /// </summary>
        public abstract bool CanStart { get; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public abstract void StartTick();

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public abstract void PauseTick();

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public abstract void ResumeTick();

        /// <summary>
        /// Dispose the timer.
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the current evaluator.
        /// </summary>
        protected TIdentifiableEvaluator Current => _evaluators.ElementAt(_evaluatorIndex);

        /// <summary>
        /// Determine if has usable evaluator.
        /// </summary>
        /// <returns>True if has current evaluator, otherwise false.</returns>
        protected bool HasCurrent()
        {
            return _evaluators.Count > 0 && _evaluatorIndex > -1 && _evaluatorIndex < _evaluators.Count;
        }

        /// <summary>
        /// Set the next evaluator.
        /// </summary>
        /// <returns>True if evaluator setted, otherwise false.</returns>
        protected bool Next()
        {
            if (HasNext())
            {
                _evaluatorIndex += 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the current evaluator to the first.
        /// </summary>
        protected void Reset()
        {
            _evaluatorIndex = 0;
        }

        /// <summary>
        /// Cancellation Token.
        /// </summary>
        protected CancellationToken Token { get; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determine if has next evaluator.
        /// </summary>
        /// <returns>True if has next evaluator, otherwise false.</returns>
        private bool HasNext()
        {
            return _evaluators.Count > 0 && _evaluatorIndex + 1 < _evaluators.Count;
        }

        #endregion
    }
}
