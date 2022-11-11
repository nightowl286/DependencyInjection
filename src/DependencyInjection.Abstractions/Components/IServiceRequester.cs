using System;
using System.Collections.Generic;

namespace TNO.DependencyInjection.Abstractions.Components
{
   /// <summary>
   /// Denotes that the implementing type can be used to request services registered with an <see cref="IServiceRegistrar"/>.
   /// </summary>
   public interface IServiceRequester : IServiceProvider, IServiceScope, IDisposable
   {
      #region Methods
      /// <summary>Requests an instance that was registered for the given <paramref name="type"/>.</summary>
      /// <param name="type">The type of the service to request.</param>
      /// <returns>An instance of the registered service.</returns>
      object Get(Type type);

      /// <summary>Attempts to request an instance that was registered for the given <paramref name="type"/>.</summary>
      /// <param name="type">The type of the service to request.</param>
      /// <returns>
      /// An instance of the registered service, or <see langword="null"/> 
      /// if the given <paramref name="type"/> hasn't been registered.
      /// </returns>
      object? GetOptional(Type type);

      /// <summary>Requests all the instances registered for the given <paramref name="type"/>.</summary>
      /// <param name="type">The type of the services to request.</param>
      /// <returns>A collection of the registered instances.</returns>
      IEnumerable<object> GetAll(Type type);
      object IServiceProvider.GetService(Type serviceType) => Get(serviceType);
      #endregion
   }
}
