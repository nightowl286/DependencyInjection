using System;
using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   public interface IParameterExplanation : IExplanation
   {
      #region Properties
      ParameterInfo Parameter { get; }
      IParameterTypeExplanation? ParameterTypeExplanation { get; }
      #endregion

      #region Methods
      new ParameterException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
