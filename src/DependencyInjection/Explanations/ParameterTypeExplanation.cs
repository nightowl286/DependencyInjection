using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed class ParameterTypeExplanation : IParameterTypeExplanation
{
   #region Properties
   /// <inheritdoc/>
   public Type Type { get; }

   /// <inheritdoc/>
   public string Explanation { get; }
   #endregion

   #region Constructors
   /// <summary>Creates a new instance of the <see cref="ParameterTypeExplanation"/>.</summary>
   /// <param name="type">The parameter's <see cref="System.Type"/> that this explanation is associated with.</param>
   /// <param name="explanation">The explanation string that should be shown to the developer.</param>
   public ParameterTypeExplanation(Type type, string explanation)
   {
      Type = type;
      Explanation = explanation;
   }
   #endregion

   #region Methods
   /// <inheritdoc/>
   public ParameterTypeException ToException() => new ParameterTypeException(Type, Explanation);
   #endregion
}