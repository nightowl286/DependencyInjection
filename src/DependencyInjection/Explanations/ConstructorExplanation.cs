using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed class ConstructorExplanation : IConstructorExplanation
{
   #region Properties
   /// <inheritdoc/>
   public ConstructorInfo Constructor { get; }

   /// <inheritdoc/>
   public string Explanation { get; }

   /// <inheritdoc/>
   public IReadOnlyCollection<IParameterExplanation> ParameterExplanations { get; }
   #endregion

   #region Constructors
   /// <summary>Creates a new instance of the <see cref="ConstructorExplanation"/>.</summary>
   /// <param name="constructor">The <see cref="ConstructorInfo"/> that this explanation is associated with.</param>
   /// <param name="explanation">The explanation string that should be shown to the developer.</param>
   /// <param name="parameterExplanations">
   /// A collection of relevant <see cref="IParameterExplanation"/>
   /// for the parameters of the <paramref name="constructor"/>.
   /// </param>
   public ConstructorExplanation(ConstructorInfo constructor, string explanation, IReadOnlyCollection<IParameterExplanation> parameterExplanations)
   {
      Constructor = constructor;
      Explanation = explanation;
      ParameterExplanations = parameterExplanations;
   }
   #endregion

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