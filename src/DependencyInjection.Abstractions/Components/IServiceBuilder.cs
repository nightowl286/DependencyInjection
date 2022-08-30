using System;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Abstractions.Components
{
   public interface IServiceBuilder : IDisposable
   {
      #region Methods
      ITypeExplanation? Explain(Type type);
      object Build(Type type);
      bool CanBuild(Type type);
      #endregion
   }
}
