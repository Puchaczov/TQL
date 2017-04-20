using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TQL.Interfaces;

namespace TQL.Threading
{
    public abstract class TimerBase: MarshalByRefObject, IDisposable
    {
        #region Private variables
        private readonly IList<IFireTimeEvaluator> _evaluators;
        private int _evaluatorIndex;

        protected bool ShouldStop { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="evaluators">The evaluators.</param>
        /// <param name="token">The cancellation token.</param>
        protected TimerBase(IReadOnlyCollection<IFireTimeEvaluator> evaluators, CancellationToken token)
        {
            _evaluators = evaluators.ToList();
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
        /// Starts the timer.
        /// </summary>
        public abstract void StartTick();

        /// <summary>
        /// Dispose the timer.
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the current evaluator.
        /// </summary>
        protected IFireTimeEvaluator Current => _evaluators[_evaluatorIndex];

        /// <summary>
        /// Determine if has usable evaluator.
        /// </summary>
        /// <returns>True if has current evaluator, otherwise false.</returns>
        protected bool HasCurrent()
        {
            return _evaluators.Count > 0 && _evaluatorIndex > -1 && _evaluatorIndex < _evaluators.Count;
        }

        /// <summary>
        /// Removes from evaluators.
        /// </summary>
        protected void Remove()
        {
            _evaluators.RemoveAt(_evaluatorIndex);
            if (!HasNext())
                _evaluatorIndex = 0;
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
