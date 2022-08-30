using System;

namespace TNO.DependencyInjection.Abstractions.Components
{
   public interface IServiceScope
   {
      #region Methods
      IServiceFacade CreateScope(RegistrationMode? defaultMode = null);
      bool IsRegistered(Type type);
      #endregion
   }
}
