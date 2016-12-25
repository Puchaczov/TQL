using TQL.Core.Exceptions;
using TQL.Core.Tokens;
using System;

namespace TQL.Core.Syntax
{
    public abstract class ParserBase<TToken, TTokenType> 
        where TToken : GenericToken<TTokenType>
        where TTokenType : struct, IComparable, IFormattable
    {
        public abstract TToken CurrentToken { get; protected set; }
        public abstract TToken LastToken { get; protected set; }
        protected abstract ILexer<TToken> Lexer { get; }

        public void Consume(TTokenType tokenType)
        {
            if (CurrentToken.TokenType.Equals(tokenType))
            {
                LastToken = CurrentToken;
                CurrentToken = Lexer.NextToken();
                return;
            }
            throw new UnexpectedTokenException<TTokenType>(Lexer.Position, CurrentToken);
        }
    }
}
