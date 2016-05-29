﻿using System.Collections.Generic;
using System.Linq;

namespace TQL.Common.List
{
    public static class ListHelper
    {
        public static void AddRange(this IList<int> list, IEnumerable<int> values)
        {
            foreach (var item in values)
            {
                list.Add(item);
            }
        }

        public static void Cut(this IList<int> list, int min, int max, int cutNotEveryNth)
        {
            list = list.Where((f, i) => f >= min && f < max && i % cutNotEveryNth != 0).ToList();
        }

        public static void Cut(this IList<int> list, string min, string max, string cutNotEveryNth)
        {
            list.Cut(int.Parse(min), int.Parse(max), int.Parse(cutNotEveryNth));
        }

        public static IList<int> CutMe(this IList<int> list, string min, string max, string cutNotEveryNth)
        {
            ListHelper.Cut(list, min, max, cutNotEveryNth);
            return list;
        }

        public static List<int> Empty() => new List<int>();

        public static IList<int> Expand(int from, int to, int inc)
        {
            var values = new List<int>();
            var i = from;
            for (int j = to - inc; i <= j; i += inc)
            {
                values.Add(i);
            }
            if (i <= to)
            {
                values.Add(i);
            }
            return values;
        }
    }
}
