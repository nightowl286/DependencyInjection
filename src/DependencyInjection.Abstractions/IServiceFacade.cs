using System;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Abstractions
{
   public interface IServiceFacade : IServiceRegistrar, IServiceRequester, IServiceBuilder , IDisposable { }
}
