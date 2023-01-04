using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.Common.Locking;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components;

internal sealed class ServiceRegistrar : IServiceRegistrar
{
   #region Fields
   private readonly ServiceContext _context;
   private ReferenceKey? _lockedWith;
   private readonly object _lockingLock = new object();
   #endregion

   #region Properties
   public AppendValueMode DefaultRegistrationMode { get; }
   public bool IsLocked { get; private set; }
   #endregion
   public ServiceRegistrar(ServiceContext context, AppendValueMode defaultMode = AppendValueMode.ReplaceAll)
   {
      DefaultRegistrationMode = defaultMode;
      _context = context;
   }

   #region Methods
   public bool TryLock([NotNullWhen(true)] out ReferenceKey? key)
   {
      lock (_lockingLock)
      {
         if (IsLocked == false)
         {
            Debug.Assert(_lockedWith is null);
            key = _lockedWith = new ReferenceKey();
            IsLocked = true;
            return true;
         }

         key = null;
         return false;
      }
   }
   public bool TryUnlock(ReferenceKey key) // Todo(Anyone): Maybe this should return an enum instead;
   {
      lock (_lockingLock)
      {
         if (IsLocked == false)
         {
            Debug.Assert(_lockedWith is null);
            return false;
         }

         Debug.Assert(_lockedWith is not null);
         if (ReferenceEquals(_lockedWith, key))
         {
            IsLocked = false;
            _lockedWith = null;
            return true;
         }

         return false;
      }
   }

   public IServiceRegistrar Instance(Type serviceType, object instance, AppendValueMode? mode = null)
   {
      CheckLock();

      if (!serviceType.IsAssignableFrom(instance.GetType()))
         throw new ArgumentException($"The type of the given instance ({instance.GetType()}) cannot be assigned to the given service type ({serviceType}).");

      InstanceRegistration registration = new InstanceRegistration(instance);
      _context.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

      return this;
   }
   public IServiceRegistrar PerRequest(Type serviceType, Type concreteType, AppendValueMode? mode = null)
   {
      CheckLock();

      RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, true);

      PerRequestRegistration registration = new PerRequestRegistration(concreteType);
      _context.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

      return this;
   }
   public IServiceRegistrar Singleton(Type serviceType, Type concreteType, AppendValueMode? mode = null)
   {
      CheckLock();

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
   public IServiceFacade CreateScope(AppendValueMode? defaultMode = null) => _context.Facade.CreateScope(defaultMode ?? DefaultRegistrationMode);
   public void Dispose() { }
   public IServiceRegistrar RegisterSelf()
   {
      CheckLock();

      _context.Facade.RegisterSelf();
      return this;
   }
   #endregion

   #region Helpers
   private void CheckLock()
   {
      if (IsLocked)
         throw new RegistrarLockedException();
   }
   #endregion
}