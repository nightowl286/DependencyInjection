using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components
{
   internal sealed class ServiceContext : IDisposable
   {
      #region Properties
      public IServiceFacade Facade { get; }
      public ServiceContext? OuterContext { get; }
      public TypeCollectionStore<RegistrationBase> Registrations { get; }
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
      #endregion
   }
}
