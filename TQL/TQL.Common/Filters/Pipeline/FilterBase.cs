namespace TQL.Common.Filters.Pipeline
{
    public abstract class FilterBase<T> : IFilter<T>
    {
        private IFilter<T> _next;

        /// <summary>
        /// Process the input and run next filter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Filtered input.</returns>
        public T Execute(T input)
        {
            var val = Process(input);
            if (_next != null) val = _next.Execute(val);
            return val;
        }

        /// <summary>
        /// Registers new filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Register(IFilter<T> filter)
        {
            if (_next == null)
                _next = filter;
            else
                _next.Register(filter);
        }

        /// <summary>
        /// Process the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Processed input.</returns>
        protected abstract T Process(T input);
    }
}