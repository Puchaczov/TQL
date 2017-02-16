namespace TQL.Common.Filters.Pipeline
{
    public interface IFilter<T>
    {
        /// <summary>
        /// Registers the fiter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        void Register(IFilter<T> filter);

        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Filtered input.</returns>
        T Execute(T input);
    }
}