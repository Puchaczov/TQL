using System;
using TQL.Core.Tokens;

namespace TQL.Core.Syntax
{
    public abstract class ParserBase<TToken, TTokenType>
        where TToken : GenericToken<TTokenType>
        where TTokenType : struct, IComparable, IFormattable
    { }
}