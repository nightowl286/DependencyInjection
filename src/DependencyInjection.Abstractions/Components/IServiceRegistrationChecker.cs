using System;

namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Denotes that the implementing type can check whether a given service is registered.
/// </summary>
public interface IServiceRegistrationChecker
{
   #region Methods
   /// <summary>
   /// Checks if the given <paramref name="serviceType"/> has been registered in the current scope/chain of scopes.
   /// </summary>
   /// <param name="serviceType">The type to check.</param>
   /// <returns>
   /// <see langword="true"/> if the given <paramref name="serviceType"/> 
   /// has been registered, <see langword="false"/> otherwise.
   /// </returns>
   bool IsRegistered(Type serviceType);
   #endregion
}
