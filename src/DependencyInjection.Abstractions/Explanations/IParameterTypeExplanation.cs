using System;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   /// <summary>
   /// Denotes an explanation about a parameter's <see cref="System.Type"/>.
   /// </summary>
   public interface IParameterTypeExplanation : IExplanation
   {
      #region Properties
      /// <summary>
      /// The parameter's <see cref="System.Type"/> that this explanation is associated with.
      /// </summary>
      Type Type { get; }
      #endregion

      #region Methods
      /// <summary>Converts this explanation into a <see cref="ParameterTypeException"/>.</summary>
      /// <returns>An instance of <see cref="ParameterTypeException"/> that contains this explanation's data.</returns>
      new ParameterTypeException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
