using System.Collections.Generic;

namespace TQL.Core.Syntax
{
    public interface ILexer<TToken>
    {
        TToken NextToken();
        TToken LastToken();
        TToken CurrentToken();
        int Position { get; }
    }
}
