using System;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   public interface IParameterTypeExplanation : IExplanation
   {
      #region Properties
      Type Type { get; }
      #endregion

      #region Methods
      new ParameterTypeException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
