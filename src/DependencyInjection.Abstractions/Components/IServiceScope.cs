namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Denotes that the implementing type is inside a service scope.
/// </summary>
public interface IServiceScope : IServiceRegistrationChecker
{
   #region Properties
   /// <summary>A requester that can be used to request the services from this scope.</summary>
   IServiceRequester Requester { get; }

   /// <summary>A registrar that can be used to register new services in this scope.</summary>
   IServiceRegistrar Registrar { get; }

   /// <summary>A builder that can be used to build new instances using the services in this scope.</summary>
   IServiceBuilder Builder { get; }
   #endregion

   #region Methods
   /// <summary>Creates a new service scope nested in the current one.</summary>
   /// <param name="defaultMode">
   /// The <see cref="AppendValueMode"/> to set the as the default one, if <see langword="null"/> 
   /// then the current scopes <see cref="IServiceRegistrar.DefaultRegistrationMode"/> will be used.
   /// </param>
   /// <returns>A new <see cref="IServiceFacade"/> that represents everything in the new scope.</returns>
   IServiceScope CreateScope(AppendValueMode? defaultMode = null);
   #endregion
}