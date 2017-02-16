using TQL.Common.Filters.Pipeline;

namespace TQL.Common.Filters
{
    public class UpperCase : FilterBase<string>
    {
        /// <summary>
        /// Uppercases the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Uppercased input.</returns>
        protected override string Process(string input) => input.ToUpperInvariant();
    }
}