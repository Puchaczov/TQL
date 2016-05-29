﻿using TQL.Core.Tokens;

namespace TQL.Core.Helpers
{
    public static class TextSpanHelper
    {
        public static bool IsEqual(this TextSpan span1, TextSpan span2)
        {
            if (span1.Start == span2.Start && span1.End == span2.End)
            {
                return true;
            }
            return false;
        }
        public static bool IsInside(this TextSpan span1, TextSpan span2)
        {
            if (span1.Start >= span2.Start && span1.End <= span2.End)
            {
                return true;
            }
            return false;
        }
    }
}
