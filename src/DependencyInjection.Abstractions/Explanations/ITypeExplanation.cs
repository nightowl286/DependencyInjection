using System;
using System.Collections.Generic;
using TNO.DependencyInjection.Abstractions.Exceptions;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   public interface ITypeExplanation : IExplanation
   {
      #region Properties
      Type Type { get; }
      IReadOnlyCollection<IConstructorExplanation> ConstructorExplanations { get; }
      #endregion

      #region Methods
      new TypeException ToException();
      Exception IExplanation.ToException() => ToException();
      #endregion
   }
}
