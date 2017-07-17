using System;
using TQL.Interfaces;

namespace TQL.Common.Evaluators
{
    public class TimeZoneChangerDecorator : IFireTimeEvaluator
    {
        private readonly TimeZoneInfo _destinationZoneInfo;
        private readonly TimeZoneInfo _sourceZoneInfo;
        private readonly IFireTimeEvaluator _evaluator;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="source">Evaluator working timezone</param>
        /// <param name="destinationZoneInfo">Destination timezone.</param>
        /// <param name="evaluator">The evaluator.</param>
        public TimeZoneChangerDecorator(TimeZoneInfo source, TimeZoneInfo destinationZoneInfo, IFireTimeEvaluator evaluator)
        {
            _destinationZoneInfo = destinationZoneInfo;
            _evaluator = evaluator;
            _sourceZoneInfo = source;
        }

        /// <summary>
        /// Determine if passed datetime fits the where condition.
        /// </summary>
        /// <param name="dateTime">The datetime.</param>
        /// <returns>True if where conditions have been met, otherwise false.</returns>
        public bool IsSatisfiedBy(DateTimeOffset dateTime)
        {
            var dt = TimeZoneInfo.ConvertTime(dateTime, _sourceZoneInfo);

            return _evaluator.IsSatisfiedBy(dt);
        }

        /// <summary>
        /// Evaluate next occurence.
        /// </summary>
        /// <returns>Value if next occurence founded, otherwise null.</returns>
        public DateTimeOffset? NextFire()
        {
            var generatedTime = _evaluator.NextFire();

            if (generatedTime.HasValue)
                return TimeZoneInfo.ConvertTime(generatedTime.Value, _destinationZoneInfo);
            return null;
        }
    }
}