using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Explanation;

/// <inheritdoc/>
public sealed class ParameterExplanation : IParameterExplanation
{
   #region Properties
   /// <inheritdoc/>
   public ParameterInfo Parameter { get; }

   /// <inheritdoc/>
   public string Explanation { get; }

   /// <inheritdoc/>
   public IParameterTypeExplanation? ParameterTypeExplanation { get; }
   #endregion

   #region Constructors
   /// <summary>Creates a new instance of the <see cref="ParameterExplanation"/>.</summary>
   /// <param name="parameter">The <see cref="ParameterInfo"/> that this explanation is associated with.</param>
   /// <param name="explanation">The explanation string that should be shown to the developer.</param>
   /// <param name="parameterTypeExplanation">A possible <see cref="IParameterTypeExplanation"/> about the type of <paramref name="parameter"/>.</param>
   public ParameterExplanation(ParameterInfo parameter, string explanation, IParameterTypeExplanation? parameterTypeExplanation)
   {
      Parameter = parameter;
      Explanation = explanation;
      ParameterTypeExplanation = parameterTypeExplanation;
   }
   #endregion

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