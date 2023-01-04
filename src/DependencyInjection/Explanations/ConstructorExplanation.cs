using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed record ConstructorExplanation(ConstructorInfo Constructor, string Explanation, IReadOnlyCollection<IParameterExplanation> ParameterExplanations) : IConstructorExplanation
{
   #region Methods
   /// <inheritdoc/>
   public ConstructorException ToException()
   {
      if (ParameterExplanations.Count > 0)
      {
         Exception inner;
         if (ParameterExplanations.Count == 1)
            inner = ParameterExplanations.First().ToException();
         else
         {
            IEnumerable<Exception> aggregates = ParameterExplanations.Select(pe => pe.ToException());
            inner = new AggregateException(aggregates);
         }

         return new ConstructorException(Constructor, Explanation, inner);
      }

      return new ConstructorException(Constructor, Explanation);
   }
   #endregion
}