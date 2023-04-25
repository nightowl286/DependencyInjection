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
   private readonly ServiceScope _scope;
   private ReferenceKey? _lockedWith;
   private readonly object _lockingLock = new object();
   #endregion

   #region Properties
   public AppendValueMode DefaultRegistrationMode { get; }
   public bool IsLocked { get; private set; }
   #endregion
   public ServiceRegistrar(ServiceScope scope, AppendValueMode defaultMode = AppendValueMode.ReplaceAll)
   {
      DefaultRegistrationMode = defaultMode;
      _scope = scope;
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
   public UnlockResult TryUnlock(ReferenceKey key)
   {
      lock (_lockingLock)
      {
         if (IsLocked == false)
         {
            Debug.Assert(_lockedWith is null);
            return UnlockResult.AlreadyUnlocked;
         }

         Debug.Assert(_lockedWith is not null);
         if (ReferenceEquals(_lockedWith, key))
         {
            IsLocked = false;
            _lockedWith = null;
            return UnlockResult.Unlocked;
         }

         return UnlockResult.IncorrectKey;
      }
   }

   public IServiceRegistrar Instance(Type serviceType, object instance, AppendValueMode? mode = null)
   {
      CheckLock();

      if (!serviceType.IsAssignableFrom(instance.GetType()))
         throw new ArgumentException($"The type of the given instance ({instance.GetType()}) cannot be assigned to the given service type ({serviceType}).");

      InstanceRegistration registration = new InstanceRegistration(instance);
      _scope.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

      return this;
   }
   public IServiceRegistrar PerRequest(Type serviceType, Type concreteType, AppendValueMode? mode = null)
   {
      RegistrationBase registration = PerRequestBase(serviceType, concreteType);
      _scope.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

      return this;
   }
   public IServiceRegistrar PerRequestIfMissing(Type serviceType, Type concreteType)
   {
      RegistrationBase registration = PerRequestBase(serviceType, concreteType);
      _scope.Registrations.TryAdd(serviceType, registration);

      return this;
   }
   public IServiceRegistrar Singleton(Type serviceType, Type concreteType, AppendValueMode? mode = null)
   {
      RegistrationBase registration = SingletonBase(serviceType, concreteType);
      _scope.Registrations.Add(serviceType, registration, mode ?? DefaultRegistrationMode);

      return this;
   }
   public IServiceRegistrar SingletonIfMissing(Type serviceType, Type concreteType)
   {
      RegistrationBase registration = SingletonBase(serviceType, concreteType);
      _scope.Registrations.TryAdd(serviceType, registration);

      return this;
   }
   public bool IsRegistered(Type type) => _scope.IsRegistered(type);
   public IServiceRegistrar RegisterComponents()
   {
      CheckLock();

      Instance(typeof(IServiceRequester), _scope.Requester, AppendValueMode.ReplaceAll);
      Instance(typeof(IServiceProvider), _scope.Requester, AppendValueMode.ReplaceAll);
      Instance(typeof(IServiceBuilder), _scope.Builder, AppendValueMode.ReplaceAll);
      Instance(typeof(IServiceRegistrar), _scope.Registrar, AppendValueMode.ReplaceAll);
      Instance(typeof(IServiceScope), _scope, AppendValueMode.ReplaceAll);

      return this;
   }
   #endregion

   #region Helpers
   private void CheckLock()
   {
      if (IsLocked)
         throw new RegistrarLockedException();
   }
   private PerRequestRegistration PerRequestBase(Type serviceType, Type concreteType)
   {
      CheckLock();

      RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, true);

      PerRequestRegistration registration = new PerRequestRegistration(concreteType);
      return registration;
   }
   private RegistrationBase SingletonBase(Type serviceType, Type concreteType)
   {
      CheckLock();

      RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, true);

      if (concreteType.IsGenericTypeDefinition)
         return new GenericSingletonRegistration(concreteType);
      else
         return new SingletonRegistration(concreteType);
   }
   #endregion
}