using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Explanations;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components
{
   public sealed class SimpleCollection : IDisposable, IServiceBuilder
   {
      #region Fields
      private readonly Dictionary<Type, RegistrationBase> _registrations = new Dictionary<Type, RegistrationBase>();
      private readonly IServiceBuilder _builder;
      private readonly SimpleCollection? _outerScope;
      private const ReplaceMode DefaultReplaceMode = ReplaceMode.Throw;
      private readonly bool _mustImplementServiceType;
      #endregion
      public SimpleCollection(IServiceBuilder builder, bool mustImplementServiceType = true) : this(builder, mustImplementServiceType, null) { }
      private SimpleCollection(IServiceBuilder builder, bool mustImplementServiceType, SimpleCollection? outerScope = null)
      {
         _builder = builder;
         _outerScope = outerScope;
         _mustImplementServiceType = mustImplementServiceType;
      }

      #region Methods
      #region Registrar
      #region Instance
      public void Instance(Type serviceType, object instance, ReplaceMode mode = DefaultReplaceMode)
      {
         if (_mustImplementServiceType) Guard.IsAssignableToType(instance, serviceType);

         CheckRegistration(serviceType, mode);

         _registrations[serviceType] = new InstanceRegistration(instance);
      }
      public void Instance(object instance, ReplaceMode mode = DefaultReplaceMode)
         => Instance(instance.GetType(), instance, mode);
      public void Instance<TService>(TService instance, ReplaceMode mode = DefaultReplaceMode) where TService : notnull
         => Instance(typeof(TService), instance, mode);
      #endregion

      #region Per Request
      public void PerRequest(Type serviceType, Type concreteType, ReplaceMode mode = DefaultReplaceMode)
      {
         RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, _mustImplementServiceType);
         CheckRegistration(serviceType, mode);

         _registrations[serviceType] = new PerRequestRegistration(concreteType);
      }
      public void PerRequest(Type concreteType, ReplaceMode mode = DefaultReplaceMode)
         => PerRequest(concreteType, concreteType, mode);
      public void PerRequest<TService, TConcrete>(ReplaceMode mode = DefaultReplaceMode)
         where TService : notnull
         where TConcrete : notnull, TService
         => PerRequest(typeof(TService), typeof(TConcrete), mode);
      public void PerRequest<TConcrete>(ReplaceMode mode = DefaultReplaceMode)
         where TConcrete : notnull
         => PerRequest(typeof(TConcrete), mode);
      #endregion

      #region Singleton
      public void Singleton(Type serviceType, Type concreteType, ReplaceMode mode = DefaultReplaceMode)
      {
         RegistrarUtility.CheckTypeImplementation(serviceType, concreteType, _mustImplementServiceType);
         CheckRegistration(serviceType, mode);

         _registrations[serviceType] = new SingletonRegistration(concreteType);
      }
      public void Singleton(Type concreteType, ReplaceMode mode = DefaultReplaceMode)
         => Singleton(concreteType, concreteType, mode);
      public void Singleton<TService, TConcrete>(ReplaceMode mode = DefaultReplaceMode)
         where TService : notnull
         where TConcrete : notnull, TService
         => Singleton(typeof(TService), typeof(TConcrete), mode);
      public void Singleton<TConcrete>(ReplaceMode mode = DefaultReplaceMode)
         where TConcrete : notnull
         => Singleton(typeof(TConcrete), mode);
      #endregion
      #endregion
      #region Requester
      public object Get(Type serviceType)
      {
         object? instance = GetOptional(serviceType);

         if (instance is null)
            return ThrowHelper.ThrowArgumentException<object>(nameof(serviceType), $"The service type {serviceType} has not been registered.");

         return instance;
      }
      public T Get<T>() => (T)Get(typeof(T));

      public object? GetOptional(Type serviceType)
      {
         if (_registrations.TryGetValue(serviceType, out RegistrationBase? registration))
         {
            if (registration is InstanceRegistration instance)
               return instance.Instance;
            if (registration is SingletonRegistration singleton)
            {
               if (singleton.Instance is null)
                  singleton.Instance = _builder.Build(singleton.Type);

               return singleton.Instance;
            }
            if (registration is PerRequestRegistration perRequest)
               return _builder.Build(perRequest.Type);

            ThrowHelper.ThrowNotSupportedException($"The registration ({registration}) is not supported.");
         }

         return _outerScope?.GetOptional(serviceType);
      }
      public T? GetOptional<T>() where T : notnull => (T?)GetOptional(typeof(T));

      public bool IsRegistered(Type serviceType)
      {
         if (_registrations.ContainsKey(serviceType))
            return true;

         return _outerScope?.IsRegistered(serviceType) == true;
      }
      public bool IsRegistered<T>() => IsRegistered(typeof(T));
      #endregion
      #region Builder
      public ITypeExplanation? Explain(Type type) => _builder.Explain(type);
      public object Build(Type type) => _builder.Build(type);
      public bool CanBuild(Type type) => _builder.CanBuild(type);
      #endregion
      public SimpleCollection CreateScope() => new SimpleCollection(_builder, _mustImplementServiceType, this);
      public void Dispose()
      {
         foreach (RegistrationBase registration in _registrations.Values)
            registration.Dispose();

         _registrations.Clear();
      }
      #endregion

      #region Helper
      private void CheckRegistration(Type serviceType, ReplaceMode mode)
      {
         bool isRegistered = _registrations.TryGetValue(serviceType, out RegistrationBase? registration);
         if (!isRegistered) return;

         if (mode == ReplaceMode.Throw)
            ThrowTypeRegistered(serviceType);

         bool hasInstance = registration is InstanceRegistration
            || (registration is SingletonRegistration singleton && singleton.Instance is not null);

         if (hasInstance)
            ThrowTypeRegistered(serviceType);

         return;
      }

      [DoesNotReturn]
      private static void ThrowTypeRegistered(Type serviceType) => ThrowHelper.ThrowArgumentException(nameof(serviceType), $"The service type ({serviceType}) has already been registered.");
      #endregion
   }
}
