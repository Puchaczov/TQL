using System;
using TQL.Interfaces;

namespace TQL.Common.Evaluators
{
    public class TimeZoneChangerDecorator : IFireTimeEvaluator
    {
        private readonly TimeZoneInfo destinationZoneInfo;
        private readonly IFireTimeEvaluator evaluator;

        public TimeZoneChangerDecorator(TimeZoneInfo destinationZoneInfo, IFireTimeEvaluator evaluator)
        {
            this.destinationZoneInfo = destinationZoneInfo;
            this.evaluator = evaluator;
        }

        public DateTimeOffset? NextFire()
        {
            var generatedTime = evaluator.NextFire();

            if (generatedTime.HasValue)
                return TimeZoneInfo.ConvertTime(generatedTime.Value, destinationZoneInfo);
            return generatedTime;
        }
    }
}
