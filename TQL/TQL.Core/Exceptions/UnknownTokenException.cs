using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.Core.Exceptions
{
    public class UnknownTokenException : Exception
    {
        private readonly char currentChar;
        private readonly int pos;

        public UnknownTokenException(int pos, char currentChar, string message)
            : base(message)
        {
            this.pos = pos;
            this.currentChar = currentChar;
        }
    }
}
