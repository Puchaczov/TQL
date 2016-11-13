using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TQL.Core.Exceptions;

namespace TQL.Core.Syntax
{
    /// <summary>
    /// Idea how to implement this piece of code where founded here:
    /// https://blogs.msdn.microsoft.com/drew/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions/
    /// </summary>
    public abstract class LexerBase<TToken>: ILexer<TToken>
    {
        #region TokenUtils

        protected sealed class TokenDefinition
        {
            public Regex Regex { get; }

            public TokenDefinition(string pattern)
            {
                Regex = new Regex(pattern);
            }

            public TokenDefinition(string pattern, RegexOptions options)
            {
                Regex = new Regex(pattern, options);
            }
        }
        protected sealed class TokenPosition
        {
            public int Index { get; }
            public int Length { get; }

            public TokenPosition(int index, int length)
            {
                this.Index = index;
                this.Length = length;
            }
        }

        #endregion

        #region Private variables

        private TokenDefinition[] definitions;
        private TToken currentToken;
        private TToken lastToken;
        private int pos;

        #endregion

        protected readonly string input;

        #region constructors

        protected LexerBase(string input, TToken defaultToken, params TokenDefinition[] definitions)
        {
            if (input == null || input == string.Empty)
            {
                throw new ArgumentException(nameof(input));
            }

            if(definitions == null || definitions.Length == 0)
            {
                throw new ArgumentException(nameof(definitions));
            }

            this.input = input.Trim();

            pos = 0;
            currentToken = defaultToken;
            this.definitions = definitions;
        }

        #endregion

        protected TToken AssignTokenOfType(Func<TToken> instantiate)
        {
            if (instantiate == null)
            {
                throw new ArgumentNullException(nameof(instantiate));
            }

            lastToken = currentToken;
            currentToken = instantiate();
            return currentToken;
        }

        protected bool IsOutOfRange => pos >= input.Length;

        public int Position => pos;

        #region Interface implementation

        public virtual TToken LastToken() => lastToken;

        public virtual TToken CurrentToken() => currentToken;

        public virtual TToken NextToken()
        {
            while(!IsOutOfRange)
            {
                TokenDefinition matchedDefinition = null;
                int matchLength = 0;

                Match match = null;

                foreach (var rule in definitions)
                {
                    match = rule.Regex.Match(input, pos);

                    if (match.Success && match.Index - pos == 0)
                    {
                        matchedDefinition = rule;
                        matchLength = match.Length;
                        break;
                    }
                }

                if(matchedDefinition == null)
                {
                    throw new UnknownTokenException(pos, input[pos], string.Format("Unrecognized token exception at {0} for {1}", pos, input.Substring(pos)));
                }
                else
                {
                    var oldPos = pos;
                    var token = GetToken(matchedDefinition, match);
                    pos += matchLength;

                    return AssignTokenOfType(() => token);
                }
            }

            return AssignTokenOfType(() => GetEndOfFileToken());
        }

        #endregion

        protected abstract TToken GetToken(TokenDefinition matchedDefinition, Match match);
        protected abstract TToken GetEndOfFileToken();
    }

}
