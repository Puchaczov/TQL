using System;

namespace TQL.Core.Exceptions
{
    public class UnknownTokenException : Exception
    {
        private readonly char _currentChar;
        private readonly int _pos;

        public UnknownTokenException(int pos, char currentChar, string message)
            : base(message)
        {
            _pos = pos;
            _currentChar = currentChar;
        }
    }
}