using TQL.Common.Filters.Pipeline;

namespace TQL.Common.Filters
{
    public class LowerCase : FilterBase<string>
    {
        /// <summary>
        /// Makes the input lowercases.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Lowercased input.</returns>
        protected override string Process(string input) => input.ToLowerInvariant();
    }
}