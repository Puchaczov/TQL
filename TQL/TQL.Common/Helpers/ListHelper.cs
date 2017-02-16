using System.Collections.Generic;
using System.Linq;

namespace TQL.Common.Helpers
{
    public static class ListHelper
    {
        /// <summary>
        /// Add values to list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="values">The values.</param>
        public static void AddRange(this IList<int> list, IEnumerable<int> values)
        {
            foreach (var item in values)
                list.Add(item);
        }

        /// <summary>
        /// Cuts the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <param name="cutNotEveryNth">Cut not every nth.</param>
        public static void Cut(this IList<int> list, int min, int max, int cutNotEveryNth)
        {
            list = list.Where((f, i) => f >= min && f < max && i % cutNotEveryNth != 0).ToList();
        }

        /// <summary>
        /// Cuts the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <param name="cutNotEveryNth">Cut not every nth.</param>
        public static void Cut(this IList<int> list, string min, string max, string cutNotEveryNth)
        {
            list.Cut(int.Parse(min), int.Parse(max), int.Parse(cutNotEveryNth));
        }

        /// <summary>
        /// Cuts the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <param name="cutNotEveryNth">Cut not every nth.</param>
        /// <returns>Cutted list.</returns>
        public static IList<int> CutMe(this IList<int> list, string min, string max, string cutNotEveryNth)
        {
            list.Cut(min, max, cutNotEveryNth);
            return list;
        }

        /// <summary>
        /// Gets the empty list.
        /// </summary>
        /// <returns>The empty list.</returns>
        public static List<int> Empty() => new List<int>();

        /// <summary>
        /// Expands the list.
        /// </summary>
        /// <param name="from">The From.</param>
        /// <param name="to">The to.</param>
        /// <param name="inc">The inc.</param>
        /// <returns>Expanded list.</returns>
        public static IList<int> Expand(int from, int to, int inc)
        {
            var values = new List<int>();
            var i = from;
            for (var j = to - inc; i <= j; i += inc)
                values.Add(i);
            if (i <= to)
                values.Add(i);
            return values;
        }
    }
}