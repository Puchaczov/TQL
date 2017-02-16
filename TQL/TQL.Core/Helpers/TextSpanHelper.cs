using TQL.Core.Tokens;

namespace TQL.Core.Helpers
{
    public static class TextSpanHelper
    {
        /// <summary>
        /// Determine if two TextSpans are equals.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>True if spans are equal, otherwise false.</returns>
        public static bool IsEqual(this TextSpan first, TextSpan second)
        {
            if (first.Start == second.Start && first.End == second.End)
                return true;
            return false;
        }

        /// <summary>
        /// Determine if the second span is inside the first.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The seciond.</param>
        /// <returns>True if the second span is inside the first, otherwise false.</returns>
        public static bool IsInside(this TextSpan first, TextSpan second)
        {
            if (first.Start >= second.Start && first.End <= second.End)
                return true;
            return false;
        }
    }
}