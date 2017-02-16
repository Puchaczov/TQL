namespace TQL.Common.Filters.Pipeline
{
    public interface IFilterChain<T>
    {
        /// <summary>
        /// Executes the filter chain.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Processed input.</returns>
        T Execute(T input);

        /// <summary>
        /// Register the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The filter chain.</returns>
        IFilterChain<T> Register(IFilter<T> filter);
    }
}