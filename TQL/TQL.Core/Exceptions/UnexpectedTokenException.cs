using System;
using TQL.Core.Tokens;

namespace TQL.Core.Exceptions
{
    public class UnexpectedTokenException<TTokenType> : Exception
        where TTokenType : struct, IComparable, IFormattable
    {
        public UnexpectedTokenException(int pos, GenericToken<TTokenType> token)
        {
            Position = pos;
            Token = token;
        }

        public override string Message => $"Unexpected token {Token.Value} occured at position {Position}";
        public int Position { get; }
        public GenericToken<TTokenType> Token { get; }
    }
}