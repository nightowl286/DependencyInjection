using System;

namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Denotes that the implementing type is inside a service scope.
/// </summary>
public interface IServiceScope
{
   #region Methods
   /// <summary>Creates a new service scope nested in the current one.</summary>
   /// <param name="defaultMode">
   /// The <see cref="AppendValueMode"/> to set the as the default one, if <see langword="null"/> 
   /// then the current scopes <see cref="IServiceRegistrar.DefaultRegistrationMode"/> will be used.
   /// </param>
   /// <returns>A new <see cref="IServiceFacade"/> that represents everything in the new scope.</returns>
   IServiceFacade CreateScope(AppendValueMode? defaultMode = null);

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