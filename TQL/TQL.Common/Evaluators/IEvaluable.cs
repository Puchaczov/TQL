namespace TQL.Common.Evaluators
{
    public interface IEvaluable<out T>
    {
        /// <summary>
        /// Gets the evaluator.
        /// </summary>
        T Evaluator { get; }
    }
}