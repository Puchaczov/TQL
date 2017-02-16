using System;

namespace TQL.Common.Converters
{
    public class ConvertionByFunc<TInput, TOutput> : IConvertible<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> _converter;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="converter">Function that converts TInput to TOutput</param>
        public ConvertionByFunc(Func<TInput, TOutput> converter)
        {
            _converter = converter;
        }

        /// <summary>
        /// Converts TInput to TOutput.
        /// </summary>
        /// <param name="input">Type to convert.</param>
        /// <returns>Converted object.</returns>
        public TOutput Convert(TInput input)
        {
            if (_converter == null)
                throw new ArgumentNullException(nameof(input));

            return _converter(input);
        }
    }
}