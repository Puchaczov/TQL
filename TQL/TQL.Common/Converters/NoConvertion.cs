namespace TQL.Common.Converters
{
    public class NoConvertion<T> : IConvertible<T, T>
    {
        /// <summary>
        /// Doesn't do any convertion.
        /// </summary>
        /// <param name="input">Converting value.</param>
        /// <returns>Converted value.</returns>
        public T Convert(T input) => input;
    }
}