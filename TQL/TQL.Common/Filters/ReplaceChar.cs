﻿using TQL.Common.Pipeline;

namespace TQL.Common.Filters
{
    public class ReplaceChar : FilterBase<string>
    {
        private char sourceChar;
        private char destinationChar;

        public ReplaceChar(char source, char destination)
        {
            this.sourceChar = source;
            this.destinationChar = destination;
        }

        protected override string Process(string input) => input.Replace(sourceChar, destinationChar);
    }
}
