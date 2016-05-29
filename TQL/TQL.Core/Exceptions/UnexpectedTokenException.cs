using TQL.Core.Tokens;
using System;

namespace TQL.Core.Exceptions
{
    public class UnexpectedTokenException<TTokenType> : Exception
        where TTokenType : struct, IComparable, IFormattable
    {
        public UnexpectedTokenException(int pos, GenericToken<TTokenType> token)
        {
            this.Position = pos;
            this.Token = token;
        }

        public override string Message => $"Unexpected token {Token.Value} occured at position {Position}";
        public int Position { get; }
        public GenericToken<TTokenType> Token { get; }
    }
}
