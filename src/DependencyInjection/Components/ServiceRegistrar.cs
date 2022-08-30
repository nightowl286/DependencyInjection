using CommunityToolkit.Diagnostics;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components
{
   internal sealed class ServiceRegistrar : IServiceRegistrar
   {
      #region Fields
      private readonly ServiceContext _context;
      #endregion

      #region Properties
      public RegistrationMode DefaultRegistrationMode { get; }
      #endregion
      public ServiceRegistrar(ServiceContext context, RegistrationMode defaultMode = RegistrationMode.ReplaceAll)
      {
         DefaultRegistrationMode = defaultMode;
         _context = context;
      }

      #region Methods
      public IServiceRegistrar Instance(Type serviceType, object instance, RegistrationMode? mode = null)
      {
         Guard.IsAssignableToType(instance, serviceType);

         InstanceRegistration registration = new InstanceRegistration(instance);
         _context.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

         return this;
      }
      public IServiceRegistrar PerRequest(Type serviceType, Type concreteType, RegistrationMode? mode = null)
      {
         RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, true);

         PerRequestRegistration registration = new PerRequestRegistration(concreteType);
         _context.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

         return this;
      }
      public IServiceRegistrar Singleton(Type serviceType, Type concreteType, RegistrationMode? mode = null)
      {
         RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, true);

         RegistrationBase registration;
         if (concreteType.IsGenericTypeDefinition)
            registration = new GenericSingletonRegistration(concreteType);
         else
            registration = new SingletonRegistration(concreteType);

         _context.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

         return this;
      }
      public bool IsRegistered(Type type) => _context.Facade.IsRegistered(type);
      public IServiceFacade CreateScope(RegistrationMode? defaultMode = null) => _context.Facade.CreateScope(defaultMode ?? DefaultRegistrationMode);
      public void Dispose() { }
      public IServiceRegistrar RegisterSelf()
      {
         _context.Facade.RegisterSelf();
         return this;
      }
      #endregion
   }
}
