using System;

namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Contains extension methods for the <see cref="IServiceRegistrar"/> type.
/// </summary>
public static class ServiceRegistrarExtensions
{
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
   public static IServiceRegistrar PerRequest<TService, TConcrete>(this IServiceRegistrar registrar, AppendValueMode? mode = null)
      where TService : notnull
      where TConcrete : notnull, TService
      => registrar.PerRequest(typeof(TService), typeof(TConcrete), mode);

   /// <summary>
   /// Registers the <typeparamref name="TConcrete"/> type as itself, where a new instance 
   /// will be created each time <typeparamref name="TConcrete"/> is requested.
   /// </summary>
   /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar PerRequest<TConcrete>(this IServiceRegistrar registrar, AppendValueMode? mode = null)
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
   /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar PerRequest(this IServiceRegistrar registrar, Type concreteType, AppendValueMode? mode = null)
      => registrar.PerRequest(concreteType, concreteType, mode);

   #endregion

   #region Singleton
   /// <summary>
   /// Registers the <typeparamref name="TConcrete"/> type as the <typeparamref name="TService"/>,
   /// where a single instance will be created when the <typeparamref name="TService"/> type is requested,
   /// and then cached and reused for any following requests.
   /// </summary>
   /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar Singleton<TService, TConcrete>(this IServiceRegistrar registrar, AppendValueMode? mode = null)
      where TService : notnull
      where TConcrete : notnull, TService
      => registrar.Singleton(typeof(TService), typeof(TConcrete), mode);

   /// <summary>
   /// Registers the <typeparamref name="TConcrete"/> type as itself, where a single
   /// instance will be created each time the <typeparamref name="TConcrete"/> type
   /// is requested, and then cached and reused for any following requests.
   /// </summary>
   /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar Singleton<TConcrete>(this IServiceRegistrar registrar, AppendValueMode? mode = null)
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
   /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar Singleton(this IServiceRegistrar registrar, Type concreteType, AppendValueMode? mode = null)
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
   /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar Instance<TService>(this IServiceRegistrar registrar, TService instance, AppendValueMode? mode = null)
      where TService : notnull
      => registrar.Instance(typeof(TService), instance, mode);

   /// <summary>
   /// Registers the given <paramref name="instance"/> as itself, using <see cref="object.GetType"/>,
   /// where the given <paramref name="instance"/> will be reused each time the obtained type is requested.
   /// </summary>
   /// <inheritdoc cref="Instance{TService}(IServiceRegistrar, TService, AppendValueMode?)"/>
   public static IServiceRegistrar Instance(this IServiceRegistrar registrar, object instance, AppendValueMode? mode = null)
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
   /// <inheritdoc cref="PerRequest{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
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
   /// <inheritdoc cref="IServiceRegistrar.PerRequest(Type, Type, AppendValueMode?)"/>
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
   /// <inheritdoc cref="Singleton{TService, TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar SingletonIfMissing<TService, TConcrete>(this IServiceRegistrar registrar)
      where TService : notnull
      where TConcrete : notnull, TService
      => SingletonIfMissing(registrar, typeof(TService), typeof(TConcrete));

   /// <summary>
   /// Registers the <typeparamref name="TConcrete"/> as itself, if it has not
   /// been registered already, where a single instance will be created when <typeparamref name="TConcrete"/>
   /// is requested, and then cached and reused for any following requests.
   /// </summary>
   /// <inheritdoc cref="Singleton{TConcrete}(IServiceRegistrar, AppendValueMode?)"/>
   public static IServiceRegistrar SingletonIfMissing<TConcrete>(this IServiceRegistrar registrar)
      where TConcrete : notnull
      => SingletonIfMissing(registrar, typeof(TConcrete), typeof(TConcrete));
   #endregion
}