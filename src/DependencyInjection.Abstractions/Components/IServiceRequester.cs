using System;
using System.Collections.Generic;

namespace TNO.DependencyInjection.Abstractions.Components
{
   public interface IServiceRequester : IServiceProvider, IServiceScope, IDisposable
   {
      #region Methods
      object Get(Type type);
      object? GetOptional(Type type);
      IEnumerable<object> GetAll(Type type);
      object IServiceProvider.GetService(Type serviceType) => Get(serviceType);
      #endregion
   }
}
