using TQL.Common.Pipeline;

namespace TQL.Common.Filters
{
    public class LowerCase : FilterBase<string>
    {
        protected override string Process(string input) => input.ToLowerInvariant();
    }
}
