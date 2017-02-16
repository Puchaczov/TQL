namespace TQL.Common.Converters
{
    public interface IConvertible<in TIn, out TOut>
    {
        /// <summary>
        /// Converts TInput to TOutput.
        /// </summary>
        /// <param name="input">Value that converts will be performed.</param>
        /// <returns>Converted value.</returns>
        TOut Convert(TIn input);
    }
}