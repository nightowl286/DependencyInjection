using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed record TypeExplanation(Type Type, string Explanation, IReadOnlyCollection<IConstructorExplanation> ConstructorExplanations) : ITypeExplanation
{
   #region Methods
   /// <inheritdoc/>
   public TypeException ToException()
   {
      if (ConstructorExplanations.Count > 0)
      {
         Exception inner;
         if (ConstructorExplanations.Count == 1)
            inner = ConstructorExplanations.First().ToException();
         else
         {
            IEnumerable<Exception> aggregates = ConstructorExplanations.Select(pe => pe.ToException());
            inner = new AggregateException(aggregates);
         }

         return new TypeException(Type, Explanation, inner);
      }

      return new TypeException(Type, Explanation);
   }
   #endregion
}