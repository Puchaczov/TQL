using System;

namespace TQL.Interfaces
{
    public interface IFireTimeEvaluator
    {
        DateTimeOffset? NextFire();
    }
}
