using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TQL.Core.Syntax
{
    public abstract class LexerBase<TToken>: ILexer<TToken>
    {
        protected TToken currentToken;
        protected TToken lastToken;

        protected readonly Dictionary<char, char> endLines = new Dictionary<char, char>();

        protected readonly string input;
        protected int pos;

        public int Position => pos;

        protected LexerBase(string input, TToken defaultToken)
        {
            if (input == null || input == string.Empty)
            {
                throw new ArgumentException(nameof(input));
            }
            this.input = input.Trim();

            pos = 0;
            currentToken = defaultToken;
            endLines.Add('\r', '\n');
        }

        public abstract TToken NextToken();
        public abstract TToken LastToken();
        public abstract TToken CurrentToken();

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

        protected bool IsEndLine(char currentChar)
        {
            if (pos + 1 < input.Length && IsEndLineCharacter(currentChar, input[pos + 1]))
            {
                return true;
            }
            return false;
        }

        protected bool IsEndLineCharacter(char currentChar, char nextChar)
        {
            if (this.endLines.ContainsKey(currentChar))
            {
                return nextChar == this.endLines[currentChar];
            }
            return false;
        }

        protected static bool IsLetter(char currentChar)
        {
            if (Regex.IsMatch(currentChar.ToString(), "[a-zA-Z@]+"))
            {
                return true;
            }
            return false;
        }

        protected static bool IsDigit(char letter)
        {
            if (letter >= '0' && letter <= '9')
            {
                return true;
            }
            return false;
        }
    }
}
