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
        private readonly bool _throwOnError;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="throwOnError">Should throw on error?</param>
        protected ConverterBase(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        /// <summary>
        /// Converts the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="fun">Converter function from TNode to TResponse.</param>
        /// <returns>The TResponse.</returns>
        protected virtual TResponse Convert(
            TRequest request, Func<TNode, TResponse> fun)
            => Convert(request, new ConvertionByFunc<TNode, TResponse>(fun));

        /// <summary>
        /// Converts the request to response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>The response.</returns>
        protected virtual TResponse Convert(TRequest request, IConvertible<TNode, TResponse> converter)
        {
            try
            {
                if (!IsValid(request))
                    throw new ArgumentException(nameof(request));
                return converter.Convert(InstantiateRootNodeFromRequest(request));
            }
            catch (Exception exc)
            {
                if (_throwOnError)
                    throw;
                return GetErrorResponse(exc);
            }
        }

        /// <summary>
        /// Gets the response with error.
        /// </summary>
        /// <param name="exc">The exception.</param>
        /// <returns>The reponse.</returns>
        protected abstract TResponse GetErrorResponse(Exception exc);

        /// <summary>
        /// Determine if the request is valid.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>True if request is valid, else false.</returns>
        protected abstract bool IsValid(TRequest request);

        /// <summary>
        /// Create instance of TNode from TRequest.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The TNode.</returns>
        protected abstract TNode InstantiateRootNodeFromRequest(TRequest request);
    }
}