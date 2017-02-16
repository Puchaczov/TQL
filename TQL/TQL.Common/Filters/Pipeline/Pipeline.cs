namespace TQL.Common.Filters.Pipeline
{
    public class Pipeline<T> : IFilterChain<T>
    {
        private IFilter<T> _root;

        /// <summary>
        /// Executes the filters on input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Filtered input.</returns>
        public T Execute(T input) => _root.Execute(input);

        /// <summary>
        /// Register the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Filter chain.</returns>
        public IFilterChain<T> Register(IFilter<T> filter)
        {
            if (_root == null) _root = filter;
            else _root.Register(filter);
            return this;
        }
    }
}