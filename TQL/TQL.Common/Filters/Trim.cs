using TQL.Common.Filters.Pipeline;

namespace TQL.Common.Filters
{
    public class Trim : FilterBase<string>
    {
        /// <summary>
        /// Trims the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Trimmed input.</returns>
        protected override string Process(string input) => input.Trim();
    }
}