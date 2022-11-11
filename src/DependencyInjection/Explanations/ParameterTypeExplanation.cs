using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation
{
   /// <inheritdoc/>
   public sealed record ParameterTypeExplanation(Type Type, string Explanation) : IParameterTypeExplanation
   {
      #region Methods
      /// <inheritdoc/>
      public ParameterTypeException ToException() => new ParameterTypeException(Type, Explanation);
      #endregion
   }
}
