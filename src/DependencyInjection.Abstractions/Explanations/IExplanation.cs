using System;

namespace TNO.DependencyInjection.Abstractions.Explanations
{
   public interface IExplanation
   {
      #region Properties
      string Explanation { get; }
      #endregion

      #region Methods
      Exception ToException();
      #endregion
   }
}
