using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components;

internal sealed class ServiceScope : IServiceScope, IDisposable
{
   #region Fields
   private AppendValueMode _defaultMode;
   #endregion

   #region Properties
   public ServiceScope? OuterScope { get; }
   public TypeCollectionStore<RegistrationBase> Registrations { get; }
   public IServiceRequester Requester { get; }
   public IServiceRegistrar Registrar { get; }
   public IServiceBuilder Builder { get; }
   #endregion
   public ServiceScope(ServiceScope? outerScope, AppendValueMode defaultMode)
   {
      _defaultMode = defaultMode;

      OuterScope = outerScope;
      Registrations = new TypeCollectionStore<RegistrationBase>();

      Requester = new ServiceRequester(this);
      Registrar = new ServiceRegistrar(this, defaultMode);
      Builder = new ServiceBuilder(this);
   }

   #region Methods
   public void Dispose()
   {
      Registrations.Dispose();
   }
   public void RemoveOptimisations()
   {
      foreach (RegistrationBase registration in Registrations.GetAllValues())
         registration.RemoveOptimisations();
   }

   public IServiceScope CreateScope(AppendValueMode? defaultMode = null) => new ServiceScope(this, defaultMode ?? _defaultMode);
   public bool IsRegistered(Type serviceType)
   {
      if (Registrations.Contains(serviceType))
         return true;

      if (OuterScope is not null)
         return OuterScope.IsRegistered(serviceType);

      return false;
   }
   #endregion
}