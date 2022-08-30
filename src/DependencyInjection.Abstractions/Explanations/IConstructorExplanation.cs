using System;
using System.Collections.Generic;
using System.Reflection;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   public interface IConstructorExplanation : IExplanation
   {
      #region Properties
      ConstructorInfo Constructor { get; }
     IReadOnlyCollection<IParameterExplanation> ParameterExplanations { get; }
      #endregion

      #region Methods
      new ConstructorException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
