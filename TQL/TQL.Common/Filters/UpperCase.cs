using TQL.Common.Pipeline;

namespace TQL.Common.Filters
{
    public class UpperCase : FilterBase<string>
    {
        protected override string Process(string input) => input.ToUpperInvariant();
    }
}
