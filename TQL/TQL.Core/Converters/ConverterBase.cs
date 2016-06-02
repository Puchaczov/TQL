using System;
using TQL.Common.Converters;
using TQL.Core.Syntax;

namespace TQL.Core.Converters
{
    public abstract class ConverterBase<T, TResponse, TVisitor, TTokenType, TNode, TRequest>
        where TResponse : ConvertionResponseBase<T>
        where TTokenType : struct, IComparable, IFormattable
        where TNode : SyntaxNodeBase<TVisitor, TTokenType>
    {
        private bool throwOnError;

        protected ConverterBase(bool throwOnError)
        {
            this.throwOnError = throwOnError;
        }

        protected virtual TResponse Convert(
            TRequest request, Func<TNode, TResponse> fun)
            => Convert(request, new ConvertionByFunc<TNode, TResponse>(fun));

        protected virtual TResponse Convert(TRequest request, IConvertible<TNode, TResponse> converter)
        {
            try
            {
                if (!IsValid(request))
                {
                    throw new ArgumentException(nameof(request));
                }
                return converter.Convert(InstantiateRootNodeFromRequest(request));
            }
            catch(Exception exc)
            {
                if (throwOnError)
                    throw;
                return GetErrorResponse(exc);
            }
        }

        protected abstract TResponse GetErrorResponse(Exception exc);

        protected abstract bool IsValid(TRequest request);

        protected abstract TNode InstantiateRootNodeFromRequest(TRequest request);
    }
}
