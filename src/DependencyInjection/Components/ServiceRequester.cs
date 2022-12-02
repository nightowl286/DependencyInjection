using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Components
{
   internal sealed class ServiceRequester : IServiceRequester
   {
      #region Fields
      private readonly ServiceContext _context;
      #endregion
      public ServiceRequester(ServiceContext context)
      {
         _context = context;
      }

      #region Methods
      public object Get(Type type)
      {
         CheckRequestedType(type);

         try
         {
            if (_context.Registrations.TryGet(type, out RegistrationBase? registration))
               return Get(type, registration);

            if (type.IsConstructedGenericType)
            {
               Type genericTypeDefinition = type.GetGenericTypeDefinition();
               if (_context.Registrations.TryGet(genericTypeDefinition, out registration))
                  return Get(type, registration);
            }

            if (_context.OuterContext != null)
               return _context.OuterContext.Facade.Get(type);
         }
         catch (Exception innerException)
         {
            throw new ArgumentException($"Could not get the instance associated with the given type ({type}), check inner exception for more information.", nameof(type), innerException);
         }

         throw new ArgumentException($"Could not get the instance associated with the given type ({type}) as it has not been registered.", nameof(type));
      }
      private object Get(Type type, RegistrationBase registration)
      {
         if (registration is InstanceRegistration instance)
            return instance.Instance;
         else if (registration is SingletonRegistration singleton)
         {
            if (singleton.Instance is not null)
               return singleton.Instance;

            object singletonInstance = Build(type, singleton.Type);

            singleton.Instance = singletonInstance;
            return singletonInstance;
         }
         else if (registration is GenericSingletonRegistration genericSingleton)
         {
            if (genericSingleton.Instances.TryGet(type, out object? genericInstance))
               return genericInstance;

            genericInstance = Build(type, genericSingleton.Type);

            genericSingleton.Instances.Add(type, genericInstance, AppendValueMode.Append);
            return genericInstance;
         }
         else if (registration is PerRequestRegistration perRequest)
         {
            if (_context.IsLocked)
            {
               Type actualType = GetActualType(type, perRequest.Type);

               if (!perRequest.Optimisations.TryGetValue(actualType, out Func<object>? buildDelegate))
               {
                  buildDelegate = _context.Facade.BuildDelegate(actualType);
                  perRequest.Optimisations.Add(actualType, buildDelegate);
               }

               return buildDelegate.Invoke();
            }

            return Build(type, perRequest.Type);
         }

         throw new NotSupportedException($"Unknown registration type ({registration.GetType()}).");
      }
      public IEnumerable<object> GetAll(Type type)
      {
         CheckRequestedType(type);

         IEnumerable<RegistrationBase> registrations = _context.Registrations.GetAll(type);
         // Todo(Nightowl): Modify this to account for generics, should be a rare case that is not needed ATM;

         List<Exception>? aggregates = null;

         List<object> instances = new List<object>();
         foreach (RegistrationBase registration in registrations)
         {
            try
            {
               object instance = Get(type, registration);
               instances.Add(instance);
            }
            catch (Exception aggregate)
            {
               aggregates ??= new List<Exception>();
               aggregates.Add(aggregate);
            }
         }

         if (aggregates?.Count > 0)
         {
            Exception innerException = aggregates.Count switch
            {
               1 => aggregates[0],
               _ => new AggregateException(aggregates)
            };

            throw new ArgumentException($"Could not get all instances of the given type ({type}), check inner exception for more information.", nameof(type), innerException);
         }

         if (_context.OuterContext is not null)
            instances.AddRange(_context.OuterContext.Facade.GetAll(type));

         return instances;
      }
      public object? GetOptional(Type type)
      {
         CheckRequestedType(type);

         if (_context.Registrations.TryGet(type, out RegistrationBase? registration))
            return Get(type, registration);

         if (type.IsConstructedGenericType)
         {
            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            if (_context.Registrations.TryGet(genericTypeDefinition, out registration))
               return Get(type, registration);
         }

         return _context.OuterContext?.Facade.GetOptional(type);
      }
      public bool IsRegistered(Type type) => _context.Facade.IsRegistered(type);
      public IServiceFacade CreateScope(AppendValueMode? defaultMode = null) => _context.Facade.CreateScope(defaultMode);
      public void Dispose() { }
      #endregion

      #region Helpers
      private static void CheckRequestedType(Type type)
      {
         if (type.IsGenericTypeDefinition)
            throw new ArgumentException($"Could not get an instance of the given type ({type}) because it is a generic type definition.", nameof(type));
      }
      private Type GetActualType(Type requestedType, Type concreteType)
      {
         if (CheckGenericType(requestedType, concreteType, out Type? constructedGeneric))
            return constructedGeneric;
         else
            return concreteType;
      }
      private object Build(Type requestedType, Type concreteType)
      {
         Type type = GetActualType(requestedType, concreteType);

         try
         {
            return _context.Facade.Build(type);
         }
         catch (Exception innerException)
         {
            if (type != concreteType) // generic type was being built
               throw new ArgumentException($"Failed to build a constructed generic type for the given type ({concreteType}), check inner exception for more information", nameof(concreteType), innerException);

            throw;
         }
      }
      private static bool CheckGenericType(Type requestedType, Type concreteType, [NotNullWhen(true)] out Type? constructedGeneric)
      {
         if (concreteType.IsGenericTypeDefinition)
         {
            Debug.Assert(requestedType.IsConstructedGenericType);
            Type[] genericArguments = requestedType.GetGenericArguments();

            constructedGeneric = concreteType.MakeGenericType(genericArguments);
            return true;
         }

         constructedGeneric = null;
         return false;
      }
      #endregion
   }
}
