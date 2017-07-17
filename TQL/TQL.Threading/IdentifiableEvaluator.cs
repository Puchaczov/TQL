using System;
using TQL.Interfaces;

namespace TQL.Threading
{
    public class IdentifiableEvaluator : IFireTimeEvaluator, IKey
    {
        private readonly IFireTimeEvaluator _evaluator;

        public IdentifiableEvaluator(IFireTimeEvaluator evaluator, Guid key)
        {
            _evaluator = evaluator;
            Key = key;
        }

        public Guid Key { get; }

        public bool IsSatisfiedBy(DateTimeOffset dateTime) => _evaluator.IsSatisfiedBy(dateTime);

        public DateTimeOffset? NextFire() => _evaluator.NextFire();
    }
}