using TQL.Core.Exceptions;
using TQL.Core.Tokens;
using System;

namespace TQL.Core.Syntax
{
    public abstract class ParserBase<TToken, TTokenType> 
        where TToken : GenericToken<TTokenType>
        where TTokenType : struct, IComparable, IFormattable
    {
        protected TToken currentToken;
        protected TToken lastToken;
        protected ILexer<TToken> lexer;

        public ParserBase(ILexer<TToken> lexer)
        {
            this.lexer = lexer;
        }

        public void Consume(TTokenType tokenType)
        {
            if (currentToken.TokenType.Equals(tokenType))
            {
                lastToken = currentToken;
                currentToken = lexer.NextToken();
                return;
            }
            throw new UnexpectedTokenException<TTokenType>(lexer.Position, currentToken);
        }
    }
}
