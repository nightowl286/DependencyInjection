using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Abstractions
{
   /// <summary>
   /// Provides extension methods to the interfaces defined in the <see cref="Abstractions"/> namespace.
   /// </summary>
   public static class DependencyInjectionExtensions
   {
      #region Service Registrar
      #region Per Request
      public static IServiceRegistrar PerRequest<TService, TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TService : notnull
         where TConcrete : notnull, TService
         => registrar.PerRequest(typeof(TService), typeof(TConcrete), mode);

      public static IServiceRegistrar PerRequest<TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TConcrete : notnull
         => registrar.PerRequest(typeof(TConcrete), mode);

      public static IServiceRegistrar PerRequest(this IServiceRegistrar registrar, Type concreteType, RegistrationMode? mode = null)
         => registrar.PerRequest(concreteType, concreteType, mode);

      #endregion

      #region Singleton
      public static IServiceRegistrar Singleton<TService, TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TService : notnull
         where TConcrete : notnull, TService
         => registrar.Singleton(typeof(TService), typeof(TConcrete), mode);

      public static IServiceRegistrar Singleton<TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TConcrete : notnull
         => registrar.Singleton(typeof(TConcrete), mode);

      public static IServiceRegistrar Singleton(this IServiceRegistrar registrar, Type concreteType, RegistrationMode? mode = null)
         => registrar.Singleton(concreteType, concreteType, mode);
      #endregion

      #region Instance

      public static IServiceRegistrar Instance<TService>(this IServiceRegistrar registrar, TService instance, RegistrationMode? mode = null)
         where TService : notnull
         => registrar.Instance(typeof(TService), instance, mode);

      public static IServiceRegistrar Instance(this IServiceRegistrar registrar, object instance, RegistrationMode? mode = null)
         => registrar.Instance(instance.GetType(), instance, mode);

      #endregion

      #region PerRequest If Missing
      public static IServiceRegistrar PerRequestIfMissing(this IServiceRegistrar registrar, Type serviceType, Type concreteType)
      {
         if (!registrar.IsRegistered(serviceType))
            registrar.PerRequest(serviceType, concreteType);

         return registrar;
      }

      public static IServiceRegistrar PerRequestIfMissing(this IServiceRegistrar registrar, Type concreteType) => PerRequestIfMissing(registrar, concreteType, concreteType);

      public static IServiceRegistrar PerRequestIfMissing<TService, TConcrete>(this IServiceRegistrar registrar)
         where TService : notnull
         where TConcrete : notnull, TService
         => PerRequestIfMissing(registrar, typeof(TService), typeof(TConcrete));

      public static IServiceRegistrar PerRequestIfMissing<TConcrete>(this IServiceRegistrar registrar)
         where TConcrete : notnull
         => PerRequestIfMissing(registrar, typeof(TConcrete), typeof(TConcrete));
      #endregion

      #region Singleton If Missing
      public static IServiceRegistrar SingletonIfMissing(this IServiceRegistrar registrar, Type serviceType, Type concreteType)
      {
         if (!registrar.IsRegistered(serviceType))
            registrar.Singleton(serviceType, concreteType);

         return registrar;
      }

      public static IServiceRegistrar SingletonIfMissing(this IServiceRegistrar registrar, Type concreteType) => SingletonIfMissing(registrar, concreteType, concreteType);

      public static IServiceRegistrar SingletonIfMissing<TService, TConcrete>(this IServiceRegistrar registrar)
         where TService : notnull
         where TConcrete : notnull, TService
         => SingletonIfMissing(registrar, typeof(TService), typeof(TConcrete));

      public static IServiceRegistrar SingletonIfMissing<TConcrete>(this IServiceRegistrar registrar)
         where TConcrete : notnull
         => SingletonIfMissing(registrar, typeof(TConcrete), typeof(TConcrete));
      #endregion

      #endregion

      #region Service Requester
      public static T Get<T>(this IServiceRequester requester) where T : notnull
         => (T)requester.Get(typeof(T));

      public static T? GetOptional<T>(this IServiceRequester requester) where T : notnull
         => (T?)requester.GetOptional(typeof(T));

      public static bool TryGet(this IServiceRequester requester, Type type, [NotNullWhen(true)] out object? instance)
      {
         instance = requester.GetOptional(type);
         return instance is not null;
      }

      public static bool TryGet<T>(this IServiceRequester requester, [NotNullWhen(true)] out T? instance)
         where T : notnull
      {
         instance = (T?)requester.GetOptional(typeof(T));
         return instance is not null;
      }

      public static IEnumerable<T> GetAll<T>(this IServiceRequester requester) where T : notnull
      {
         IEnumerable<object> all = requester.GetAll(typeof(T));
         foreach (object obj in all)
            yield return (T)obj;
      }
      #endregion

      #region Service Builder
      public static T Build<T>(this IServiceBuilder builder) where T : notnull
         => (T)builder.Build(typeof(T));

      public static bool TryBuild(this IServiceBuilder builder, Type type, [NotNullWhen(true)] out object? instance)
      {
         if (builder.CanBuild(type))
         {
            instance = builder.Build(type);
            return true;
         }

         instance = null;
         return false;
      }

      public static bool TryBuild<T>(this IServiceBuilder builder, [NotNullWhen(true)] out T? instance) where T : notnull
      {
         if (TryBuild(builder, typeof(T), out object? inst))
         {
            instance = (T)inst;
            return true;
         }

         instance = default;
         return false;
      }
      #endregion

      #region Facade
      public static object GetOrBuild(this IServiceFacade facade, Type type)
      {
         if (TryGet(facade, type, out object? instance))
            return instance;

         return facade.Build(type);
      }

      public static T GetOrBuild<T>(this IServiceFacade facade) where T : notnull
         => (T)GetOrBuild(facade, typeof(T));

      #endregion
   }
}
