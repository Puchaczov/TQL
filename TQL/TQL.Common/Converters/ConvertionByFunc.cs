﻿using System;

namespace TQL.Common.Converters
{
    public class ConvertionByFunc<Input, Output> : IConvertible<Input, Output>
    {
        private readonly Func<Input, Output> converter;

        public ConvertionByFunc(Func<Input, Output> converter)
        {
            this.converter = converter;
        }

        public Output Convert(Input input)
        {
            if(converter == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return converter(input);
        }
    }
}
