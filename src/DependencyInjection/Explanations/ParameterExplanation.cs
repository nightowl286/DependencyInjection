using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed record ParameterExplanation(ParameterInfo Parameter, string Explanation, IParameterTypeExplanation? ParameterTypeExplanation) : IParameterExplanation
{
   #region Methods
   /// <inheritdoc/>
   public ParameterException ToException()
   {
      if (ParameterTypeExplanation is not null)
      {
         ParameterTypeException typeException = ParameterTypeExplanation.ToException();
         return new ParameterException(Parameter, Explanation, typeException);
      }

      return new ParameterException(Parameter, Explanation);
   }
   #endregion
}