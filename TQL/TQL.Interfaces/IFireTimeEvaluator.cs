using System;

namespace TQL.Interfaces
{
    public interface IFireTimeEvaluator
    {
        /// <summary>
        /// Compute the next occurence.
        /// </summary>
        /// <returns>Value if next occurence exist, otherwise null.</returns>
        DateTimeOffset? NextFire();

        /// <summary>
        /// Determine if passed time fits the conditions.
        /// </summary>
        /// <param name="dateTime">The datetime.</param>
        /// <returns>True if fits the conditions, otherwise false.</returns>
        bool IsSatisfiedBy(DateTimeOffset dateTime);
    }
}