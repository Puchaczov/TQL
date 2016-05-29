namespace TQL.Common.Evaluators
{
    public interface IEvaluable<T>
    {
        T Evaluator { get; }
    }
}
