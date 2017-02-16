using TQL.Common.Filters.Pipeline;

namespace TQL.Common.Filters
{
    public class ReplaceChar : FilterBase<string>
    {
        private readonly char _destinationChar;
        private readonly char _sourceChar;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="source">Source character.</param>
        /// <param name="destination">Destination character.</param>
        public ReplaceChar(char source, char destination)
        {
            _sourceChar = source;
            _destinationChar = destination;
        }

        /// <summary>
        /// Turns all source characters into destination characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The input.</returns>
        protected override string Process(string input) => input.Replace(_sourceChar, _destinationChar);
    }
}