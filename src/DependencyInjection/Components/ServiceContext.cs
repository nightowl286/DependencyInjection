using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components;

internal sealed class ServiceContext : IDisposable
{
   #region Properties
   public IServiceFacade Facade { get; }
   public ServiceContext? OuterContext { get; }
   public TypeCollectionStore<RegistrationBase> Registrations { get; }
   public bool IsLocked => Facade.IsLocked;
   #endregion
   public ServiceContext(IServiceFacade facade, ServiceContext? outerScope)
   {
      Facade = facade;
      OuterContext = outerScope;
      Registrations = new TypeCollectionStore<RegistrationBase>();
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
   #endregion
}