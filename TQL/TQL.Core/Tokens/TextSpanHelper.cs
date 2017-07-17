namespace TQL.Core.Tokens
{
    /// <summary>
    ///     This class contains usefull extensions to handle TextSpans
    /// </summary>
    public static class TextSpanHelper
    {
        /// <summary>
        /// Clones the TextSpan.
        /// </summary>
        /// <param name="textSpan">The TextSpan object.</param>
        /// <returns>Cloned TextSpan.</returns>
        public static TextSpan Clone(this TextSpan textSpan) => new TextSpan(textSpan.Start, textSpan.Length);
    }
}