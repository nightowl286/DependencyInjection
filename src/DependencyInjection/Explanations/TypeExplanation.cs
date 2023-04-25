using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed class TypeExplanation : ITypeExplanation
{
   #region Properties
   /// <inheritdoc/>
   public Type Type { get; }

   /// <inheritdoc/>
   public string Explanation { get; }

   /// <inheritdoc/>
   public IReadOnlyCollection<IConstructorExplanation> ConstructorExplanations { get; }
   #endregion

   #region Constructors
   /// <summary>Creates a new instance of the <see cref="TypeExplanation"/>.</summary>
   /// <param name="type">The <see cref="System.Type"/> that this explanation is associated with.</param>
   /// <param name="explanation">The explanation string that should be shown to the developer.</param>
   /// <param name="constructorExplanations">
   /// A collection of relevant <see cref="IConstructorExplanation"/>
   /// for the constructors of the <paramref name="type"/>.
   /// </param>
   public TypeExplanation(Type type, string explanation, IReadOnlyCollection<IConstructorExplanation> constructorExplanations)
   {
      Type = type;
      Explanation = explanation;
      ConstructorExplanations = constructorExplanations;
   }
   #endregion

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