using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Abstractions
{
   /// <summary>
   /// Provides extension methods to the interfaces defined in the <see cref="Abstractions"/> namespace.
   /// </summary>
   public static class DependencyInjectionExtensions
   {
      #region Service Registrar
      #region Per Request
      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as the <typeparamref name="TService"/> type,
      /// where a new instance will be created each time <typeparamref name="TService"/> is requested.
      /// </summary>
      /// <typeparam name="TService">The type that can be used to retrieve an instance of the type <typeparamref name="TConcrete"/>.</typeparam>
      /// <typeparam name="TConcrete">The type of the instance that will be created.</typeparam>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="mode">
      /// The registration mode to use, if <see langword="null"/> then the 
      /// <see cref="IServiceRegistrar.DefaultRegistrationMode"/> will be used.
      /// </param>
      /// <returns>The current instance of the <see cref="IServiceRegistrar"/>, following the builder pattern.</returns>
      public static IServiceRegistrar PerRequest<TService, TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TService : notnull
         where TConcrete : notnull, TService
         => registrar.PerRequest(typeof(TService), typeof(TConcrete), mode);

      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as itself, where a new instance 
      /// will be created each time <typeparamref name="TConcrete"/> is requested.
      /// </summary>
      /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar PerRequest<TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TConcrete : notnull
         => registrar.PerRequest(typeof(TConcrete), mode);

      /// <summary>
      /// Registers the given <paramref name="concreteType"/> type as itself, where a new
      /// instance will be created each time <paramref name="concreteType"/> is requested.
      /// </summary>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="concreteType">The type of the instance that will be created.</param>
      /// <param name="mode">
      /// The registration mode to use, if <see langword="null"/> then the 
      /// <see cref="IServiceRegistrar.DefaultRegistrationMode"/> will be used.
      /// </param>
      /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar PerRequest(this IServiceRegistrar registrar, Type concreteType, RegistrationMode? mode = null)
         => registrar.PerRequest(concreteType, concreteType, mode);

      #endregion

      #region Singleton
      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as the <typeparamref name="TService"/>,
      /// where a single instance will be created when the <typeparamref name="TService"/> type is requested,
      /// and then cached and reused for any following requests.
      /// </summary>
      /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar Singleton<TService, TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TService : notnull
         where TConcrete : notnull, TService
         => registrar.Singleton(typeof(TService), typeof(TConcrete), mode);

      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as itself, where a single
      /// instance will be created each time the <typeparamref name="TConcrete"/> type
      /// is requested, and then cached and reused for any following requests.
      /// </summary>
      /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar Singleton<TConcrete>(this IServiceRegistrar registrar, RegistrationMode? mode = null)
         where TConcrete : notnull
         => registrar.Singleton(typeof(TConcrete), mode);

      /// <summary>
      /// Registers the given <paramref name="concreteType"/> type as itself, where a single
      /// instance will be created each time <paramref name="concreteType"/> is requested,
      /// and then cached and reused for any following requests.
      /// </summary>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="concreteType">The type of the instance that will be created.</param>
      /// <param name="mode">
      /// The registration mode to use, if <see langword="null"/> then the 
      /// <see cref="IServiceRegistrar.DefaultRegistrationMode"/> will be used.
      /// </param>
      /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar Singleton(this IServiceRegistrar registrar, Type concreteType, RegistrationMode? mode = null)
         => registrar.Singleton(concreteType, concreteType, mode);
      #endregion

      #region Instance
      /// <summary>
      /// Registers the given <paramref name="instance"/> as the <typeparamref name="TService"/> type, where the given
      /// <paramref name="instance"/> will be reused each time the <typeparamref name="TService"/> type is requested.
      /// </summary>
      /// <typeparam name="TService">The type that can be used to retrieve the given <paramref name="instance"/>.</typeparam>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="instance">The instance that will be cached and reused.</param>
      /// <param name="mode">
      /// The registration mode to use, if <see langword="null"/> then the 
      /// <see cref="IServiceRegistrar.DefaultRegistrationMode"/> will be used.
      /// </param>
      /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar Instance<TService>(this IServiceRegistrar registrar, TService instance, RegistrationMode? mode = null)
         where TService : notnull
         => registrar.Instance(typeof(TService), instance, mode);

      /// <summary>
      /// Registers the given <paramref name="instance"/> as itself, using <see cref="object.GetType"/>,
      /// where the given <paramref name="instance"/> will be reused each time the obtained type is requested.
      /// </summary>
      /// <inheritdoc cref="Instance{TService}(IServiceRegistrar, TService, RegistrationMode?)"/>
      public static IServiceRegistrar Instance(this IServiceRegistrar registrar, object instance, RegistrationMode? mode = null)
         => registrar.Instance(instance.GetType(), instance, mode);

      #endregion

      #region PerRequest If Missing
      /// <summary>
      /// Tries to register the given <paramref name="concreteType"/> as the given <paramref name="serviceType"/>, if it has not
      /// been registered already, where a new instance will be created each time <paramref name="serviceType"/> is requested.
      /// </summary>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="serviceType">The type that can be used to retrieve an instance of the given <paramref name="concreteType"/>.</param>
      /// <param name="concreteType">The type of the instance that will be created.</param>
      /// <returns>The current instance of the <see cref="IServiceRegistrar"/>, following the builder pattern.</returns>
      public static IServiceRegistrar PerRequestIfMissing(this IServiceRegistrar registrar, Type serviceType, Type concreteType)
      {
         if (!registrar.IsRegistered(serviceType))
            registrar.PerRequest(serviceType, concreteType);

         return registrar;
      }

      /// <summary>
      /// Registers the given <paramref name="concreteType"/> type as itself, if it has not been registered 
      /// already, where a new instance will be created each time <paramref name="concreteType"/> is requested.
      /// </summary>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="concreteType">The type of the instance that will be created.</param>
      /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar PerRequestIfMissing(this IServiceRegistrar registrar, Type concreteType) => PerRequestIfMissing(registrar, concreteType, concreteType);

      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as the <typeparamref name="TService"/> type,
      /// where a new instance will be created each time <typeparamref name="TService"/> is requested.
      /// </summary>
      /// <typeparam name="TService">The type that can be used to retrieve an instance of the type <typeparamref name="TConcrete"/>.</typeparam>
      /// <typeparam name="TConcrete">The type of the instance that will be created.</typeparam>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <returns>The current instance of the <see cref="IServiceRegistrar"/>, following the builder pattern.</returns>
      public static IServiceRegistrar PerRequestIfMissing<TService, TConcrete>(this IServiceRegistrar registrar)
         where TService : notnull
         where TConcrete : notnull, TService
         => PerRequestIfMissing(registrar, typeof(TService), typeof(TConcrete));

      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as itself, if it has not bee registered already,
      /// where a new instance will be created each time the <typeparamref name="TConcrete"/> type is requested.
      /// </summary>
      /// <typeparam name="TConcrete"></typeparam>
      /// <inheritdoc cref="PerRequestIfMissing{TService, TConcrete}(IServiceRegistrar)"/>
      public static IServiceRegistrar PerRequestIfMissing<TConcrete>(this IServiceRegistrar registrar)
         where TConcrete : notnull
         => PerRequestIfMissing(registrar, typeof(TConcrete), typeof(TConcrete));
      #endregion

      #region Singleton If Missing
      /// <summary>
      /// Registers the given <paramref name="concreteType"/> as the given <paramref name="serviceType"/>, if it has not
      /// been registered already, where a single instance will be created when <paramref name="serviceType"/>
      /// is requested, and then cached and reused for any following requests.
      /// </summary>
      /// <param name="registrar">The registrar instance to use.</param>
      /// <param name="serviceType">The type that can be used to retrieve an instance of the given <paramref name="concreteType"/>.</param>
      /// <param name="concreteType">The type of the instance that will be created.</param>
      /// <inheritdoc cref="IServiceRegistrar.PerRequest(Type, Type, RegistrationMode?)"/>
      public static IServiceRegistrar SingletonIfMissing(this IServiceRegistrar registrar, Type serviceType, Type concreteType)
      {
         if (!registrar.IsRegistered(serviceType))
            registrar.Singleton(serviceType, concreteType);

         return registrar;
      }

      /// <summary>
      /// Registers the given <paramref name="concreteType"/> as itself, if it has not
      /// been registered already, where a single instance will be created when <paramref name="concreteType"/>
      /// is requested, and then cached and reused for any following requests.
      /// </summary>
      /// <inheritdoc cref="SingletonIfMissing(IServiceRegistrar, Type, Type)"/>
      public static IServiceRegistrar SingletonIfMissing(this IServiceRegistrar registrar, Type concreteType) => SingletonIfMissing(registrar, concreteType, concreteType);

      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> type as the <typeparamref name="TService"/> type, if it has not
      /// been registered already, where a single instance will be created when <typeparamref name="TService"/> type
      /// is requested, and then cached and reused for any following requests.
      /// </summary>
      /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar SingletonIfMissing<TService, TConcrete>(this IServiceRegistrar registrar)
         where TService : notnull
         where TConcrete : notnull, TService
         => SingletonIfMissing(registrar, typeof(TService), typeof(TConcrete));

      /// <summary>
      /// Registers the <typeparamref name="TConcrete"/> as itself, if it has not
      /// been registered already, where a single instance will be created when <typeparamref name="TConcrete"/>
      /// is requested, and then cached and reused for any following requests.
      /// </summary>
      /// <inheritdoc cref="Singleton{TConcrete}(IServiceRegistrar, RegistrationMode?)"/>
      public static IServiceRegistrar SingletonIfMissing<TConcrete>(this IServiceRegistrar registrar)
         where TConcrete : notnull
         => SingletonIfMissing(registrar, typeof(TConcrete), typeof(TConcrete));
      #endregion

      #endregion

      #region Service Requester
      /// <summary>Requests an instance that was registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the service to request.</typeparam>
      /// <param name="requester">The requester instance to use.</param>
      /// <returns>An instance of the registered service/</returns>
      public static T Get<T>(this IServiceRequester requester) where T : notnull
         => (T)requester.Get(typeof(T));

      /// <summary>Attempts to request an instance that was registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the service to request.</typeparam>
      /// <param name="requester">The requester instance to use.</param>
      /// <returns>
      /// An instance of the registered service, or <see langword="null"/>
      /// if the type <typeparamref name="T"/> has not been registered.
      /// </returns>
      public static T? GetOptional<T>(this IServiceRequester requester) where T : notnull
         => (T?)requester.GetOptional(typeof(T));

      /// <summary>Tries to get an instance registered for the given <paramref name="type"/>.</summary>
      /// <param name="requester">The requester instance to use.</param>
      /// <param name="type">The type to try and request.</param>
      /// <param name="instance">
      /// An instance of the given <paramref name="type"/>, or 
      /// <see langword="null"/> if an instance couldn't be obtained.
      /// </param>
      /// <returns><see langword="true"/> if an instance was obtained, <see langword="false"/> otherwise.</returns>
      public static bool TryGet(this IServiceRequester requester, Type type, [NotNullWhen(true)] out object? instance)
      {
         instance = requester.GetOptional(type);
         return instance is not null;
      }

      /// <summary>Tries to get an instance registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type to try and request.</typeparam>
      /// <param name="requester">The requester instance to use.</param>
      /// <param name="instance">
      /// An instance of the type <typeparamref name="T"/> or 
      /// <see langword="null"/> if an instance couldn't be obtained.
      /// </param>
      /// <returns><see langword="true"/> if an instance was obtained, <see langword="false"/> otherwise.</returns>
      public static bool TryGet<T>(this IServiceRequester requester, [NotNullWhen(true)] out T? instance)
         where T : notnull
      {
         instance = (T?)requester.GetOptional(typeof(T));
         return instance is not null;
      }

      /// <summary>Requests all the instances registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the services to request.</typeparam>
      /// <param name="requester">The requester instance.</param>
      /// <returns>A collection of the registered instances.</returns>
      public static IEnumerable<T> GetAll<T>(this IServiceRequester requester) where T : notnull
      {
         IEnumerable<object> all = requester.GetAll(typeof(T));
         foreach (object obj in all)
            yield return (T)obj;
      }
      #endregion

      #region Service Builder
      /// <summary>
      /// Generates an explanation as to why an instance of the type
      /// <typeparamref name="T"/> cannot be built.
      /// </summary>
      /// <typeparam name="T">The type to try and explain.</typeparam>
      /// <param name="builder">The builder instance to use.</param>
      /// <returns>
      /// An explanation as to why the type <typeparamref name="T"/> cannot
      /// be built, or <see langword="null"/> if there are no problems.
      /// </returns>
      public static ITypeExplanation? Explain<T>(this IServiceBuilder builder) => builder.Explain(typeof(T));

      /// <summary>Build an instance of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type to build.</typeparam>
      /// <param name="builder">The builder instance to use.</param>
      /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
      public static T Build<T>(this IServiceBuilder builder) where T : notnull
         => (T)builder.Build(typeof(T));

      /// <summary>Tries to build an instance of the given <paramref name="type"/>.</summary>
      /// <param name="builder">The builder instance to use.</param>
      /// <param name="type">The type to try and build.</param>
      /// <param name="instance">
      /// The built instance, or <see langword="null"/> if an instance
      /// of the given <paramref name="type"/> could not be built.
      /// </param>
      /// <remarks>If building has failed, you can call <see cref="IServiceBuilder.Explain(Type)"/> to see the reason why.</remarks>
      /// <returns><see langword="true"/> if building was successful, <see langword="false"/> otherwise.</returns>
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

      /// <summary>Tries to build an instance of the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type to try and build.</typeparam>
      /// <param name="builder">The builder instance to use.</param>
      /// <param name="instance">
      /// The built instance, or <see langword="null"/> if an instance 
      /// of the type <typeparamref name="T"/> could not be built.
      /// </param>
      /// <remarks>If building has failed, you can call <see cref="Explain{T}(IServiceBuilder)"/> to see the reason why.</remarks>
      /// <returns><see langword="true"/> if building was successful, <see langword="false"/> otherwise.</returns>
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
      /// <summary>
      /// Try to obtain an instance registered with the given <paramref name="type"/>, 
      /// if that fails, build an instance of the given <paramref name="type"/>.
      /// </summary>
      /// <param name="facade">The facade instance to use.</param>
      /// <param name="type">The type to request, or build.</param>
      /// <returns>An instance of the given <paramref name="type"/>.</returns>
      public static object GetOrBuild(this IServiceFacade facade, Type type)
      {
         if (TryGet(facade, type, out object? instance))
            return instance;

         return facade.Build(type);
      }

      /// <summary>
      /// Try to obtain an instance registered with the type <typeparamref name="T"/>,
      /// if that fails, build an instance of type <typeparamref name="T"/>.
      /// </summary>
      /// <typeparam name="T">The type to request, or build.</typeparam>
      /// <param name="facade">The facade instance to use.</param>
      /// <returns>An instance of type <typeparamref name="T"/>.</returns>
      public static T GetOrBuild<T>(this IServiceFacade facade) where T : notnull
         => (T)GetOrBuild(facade, typeof(T));
      #endregion
   }
}
