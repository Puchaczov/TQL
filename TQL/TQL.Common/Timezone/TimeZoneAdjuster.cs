using System;
using TQL.Interfaces;

namespace TQL.Common.Timezone
{
    public class TimeZoneAdjuster : IFireTimeEvaluator
    {
        private readonly TimeZoneInfo _timeZone;
        private readonly IFireTimeEvaluator _evaluator;
        private TimeSpan _fireSpan = TimeSpan.MinValue;

        /// <summary>
        /// Instantiate and configure this instance.
        /// </summary>
        /// <param name="timeZone">The source timezone.</param>
        /// <param name="evaluator">The evaluator.</param>
        public TimeZoneAdjuster(TimeZoneInfo timeZone, IFireTimeEvaluator evaluator)
        {
            _timeZone = timeZone;
            _evaluator = evaluator;
        }

        /// <summary>
        /// Calculate the next fire.
        /// </summary>
        /// <returns>DateTimeOffset if next fire exists, otherwise false.</returns>
        public bool IsSatisfiedBy(DateTimeOffset dateTime)
        {
            return _evaluator.IsSatisfiedBy(dateTime);
        }

        /// <summary>
        /// Calculate the next fire.
        /// </summary>
        /// <returns>DateTimeOffset if next fire exists, otherwise false.</returns>
        public DateTimeOffset? NextFire()
        {
            if (_fireSpan == TimeSpan.MinValue)
            {
                var fire = _evaluator.NextFire();

                if (!fire.HasValue)
                    return null;

                _fireSpan = _timeZone.GetUtcOffset(fire.Value.DateTime);

                return new DateTimeOffset(fire.Value.DateTime, _fireSpan);
            }
            var result = _evaluator.NextFire();

            if (!result.HasValue)
                return null;

            var dt = result.Value;

            return new DateTimeOffset(dt.DateTime, _fireSpan);
        }
    }
}
