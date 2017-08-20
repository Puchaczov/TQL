using System;
using TQL.Interfaces;

namespace TQL.Common.Timezone
{
    public class DaylightSavingTimeTracker : IFireTimeEvaluator
    {
        private readonly TimeZoneInfo _timeZone;
        private readonly IFireTimeEvaluator _evaluator;
        private TimeSpan _lastDiff;
        private DateTimeOffset _lastlyEvaluated = DateTimeOffset.MinValue;

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="timeZone">The source timezone.</param>
        /// <param name="evaluator">The evaluator.</param>
        public DaylightSavingTimeTracker(TimeZoneInfo timeZone, IFireTimeEvaluator evaluator)
        {
            _timeZone = timeZone;
            _evaluator = evaluator;
        }

        /// <summary>
        /// Calculate the next fire.
        /// </summary>
        /// <returns>DateTimeOffset if next fire exists, otherwise false.</returns>
        public DateTimeOffset? NextFire()
        {
            var calcFire = _evaluator.NextFire();

            if (!calcFire.HasValue)
                return null;

            if (!_timeZone.SupportsDaylightSavingTime)
                return calcFire;

            var value = calcFire.Value;

            if (_lastlyEvaluated == DateTimeOffset.MinValue)
                _lastlyEvaluated = value;

            var newOffset = _timeZone.GetUtcOffset(calcFire.Value);

            if (_timeZone.IsAmbiguousTime(calcFire.Value))
            {
                var diff = newOffset - _lastlyEvaluated.Offset;
                var newDate = new DateTimeOffset(value.DateTime.Add(diff), _lastlyEvaluated.Offset + diff);
                _lastDiff = diff;
                _lastlyEvaluated = newDate;
                return newDate;
            }

            if (newOffset == _lastlyEvaluated.Offset)
            {
                _lastlyEvaluated = new DateTimeOffset(value.DateTime.Add(_lastDiff), newOffset);
                return _lastlyEvaluated;
            }

            _lastlyEvaluated = new DateTimeOffset(value.DateTime, newOffset);
            return _lastlyEvaluated;
        }

        /// <summary>
        /// Determine if passed DateTime fits the query condition.
        /// </summary>
        /// <param name="dateTime">The DateTime.</param>
        /// <returns>True if fits the query conditions, otherwise false.</returns>
        public bool IsSatisfiedBy(DateTimeOffset dateTime)
        {
            return _evaluator.IsSatisfiedBy(dateTime);
        }
    }
}
