using System;

namespace TNO.DependencyInjection.Abstractions.Components
{
   /// <summary>
   /// Denotes a registrar that can be used to register different kinds of services.
   /// </summary>
   public interface IServiceRegistrar : IServiceScope, IDisposable
   {
      #region Properties
      /// <summary>The default <see cref="RegistrationMode"/> to use when registering a new service.</summary>
      RegistrationMode DefaultRegistrationMode { get; }
      #endregion

      #region Methods
      /// <summary>
      /// Registers the given <paramref name="concreteType"/> as the given <paramref name="serviceType"/>,
      /// where a new instance will be created each time <paramref name="serviceType"/> is requested.
      /// </summary>
      /// <param name="serviceType">The type that can be used to retrieve an instance of the given <paramref name="concreteType"/>.</param>
      /// <param name="concreteType">The type of the instance that will be created.</param>
      /// <param name="mode">
      /// The registration mode to use, if <see langword="null"/> then the 
      /// <see cref="DefaultRegistrationMode"/> will be used.
      /// </param>
      /// <returns>The current instance of the <see cref="IServiceRegistrar"/>, following the builder pattern.</returns>
      IServiceRegistrar PerRequest(Type serviceType, Type concreteType, RegistrationMode? mode = null);

      /// <summary>
      /// Registers the given <paramref name="concreteType"/> as the given <paramref name="serviceType"/>,
      /// where a single instance will be created when <paramref name="serviceType"/> is requested,
      /// and then cached and reused for any following requests.
      /// </summary>
      /// <inheritdoc cref="PerRequest(Type, Type, RegistrationMode?)"/>
      IServiceRegistrar Singleton(Type serviceType, Type concreteType, RegistrationMode? mode = null);

      /// <summary>
      /// Registers the given <paramref name="instance"/> as the given <paramref name="serviceType"/>, where the
      /// given <paramref name="instance"/> will be used each time <paramref name="serviceType"/> is requested.
      /// </summary>
      /// <param name="serviceType">The type that can be used to retrieve the given <paramref name="instance"/>.</param>
      /// <param name="instance">The instance that will be cached and reused.</param>
      /// <param name="mode">
      /// The registration mode to use, if <see langword="null"/> then the
      /// <see cref="DefaultRegistrationMode"/> will be used.
      /// </param>
      /// <returns>The current instance of the <see cref="IServiceRegistrar"/>, following the builder pattern.</returns>
      IServiceRegistrar Instance(Type serviceType, object instance, RegistrationMode? mode = null);

      /// <summary>
      /// Will register itself as being available in the dependency injection system.
      /// This will likely register the following services.
      /// </summary>
      /// <remarks>
      /// <list type="bullet">
      ///   <item><see cref="IServiceFacade"/></item>
      ///   <item><see cref="IServiceRegistrar"/></item>
      ///   <item><see cref="IServiceRequester"/></item>
      ///   <item><see cref="IServiceBuilder"/></item>
      ///   <item><see cref="IServiceProvider"/></item>
      /// </list>
      /// </remarks>
      /// <returns>The current instance of the <see cref="IServiceRegistrar"/>, following the builder pattern.</returns>
      IServiceRegistrar RegisterSelf();
      #endregion
   }
}