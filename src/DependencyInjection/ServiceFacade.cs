using System.Diagnostics.CodeAnalysis;
using TNO.Common.Locking;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Explanations;
using TNO.DependencyInjection.Components;

namespace TNO.DependencyInjection
{
   /// <inheritdoc cref="IServiceFacade"/>
   public sealed class ServiceFacade : IServiceFacade
   {
      #region Fields
      private readonly ServiceContext _context;
      private readonly ServiceRegistrar _registrar;
      private readonly ServiceBuilder _builder;
      private readonly ServiceRequester _requester;
      #endregion

      #region Properties
      /// <inheritdoc/>
      public AppendValueMode DefaultRegistrationMode => _registrar.DefaultRegistrationMode;

      /// <inheritdoc/>
      public bool IsLocked => _registrar.IsLocked;
      #endregion

      /// <summary>Creates a new instance of the <see cref="ServiceFacade"/> with the given <paramref name="defaultMode"/>.</summary>
      /// <param name="defaultMode">The default mode to use when registering new services.</param>
      public ServiceFacade(AppendValueMode defaultMode = AppendValueMode.ReplaceAll) : this(defaultMode, null) { }
      private ServiceFacade(AppendValueMode defaultMode, ServiceContext? outerContext)
      {
         _context = new ServiceContext(this, outerContext);
         _registrar = new ServiceRegistrar(_context, defaultMode);
         _requester = new ServiceRequester(_context);
         _builder = new ServiceBuilder(_context);
      }

      #region Methods
      #region Registrar
      /// <inheritdoc/>
      public bool TryLock([NotNullWhen(true)] out ReferenceKey? key) => _registrar.TryLock(out key);

      /// <inheritdoc/>
      public bool TryUnlock(ReferenceKey key) => _registrar.TryUnlock(key);

      /// <inheritdoc/>
      public IServiceRegistrar PerRequest(Type serviceType, Type concreteType, AppendValueMode? mode = null) => _registrar.PerRequest(serviceType, concreteType, mode);

      /// <inheritdoc/>
      public IServiceRegistrar Singleton(Type serviceType, Type concreteType, AppendValueMode? mode = null) => _registrar.Singleton(serviceType, concreteType, mode);

      /// <inheritdoc/>
      public IServiceRegistrar Instance(Type serviceType, object instance, AppendValueMode? mode = null) => _registrar.Instance(serviceType, instance, mode);

      /// <inheritdoc/>
      public IServiceRegistrar RegisterSelf()
      {
         Instance(typeof(IServiceRequester), _requester, AppendValueMode.ReplaceAll);
         Instance(typeof(IServiceProvider), _requester, AppendValueMode.ReplaceAll);
         Instance(typeof(IServiceBuilder), _builder, AppendValueMode.ReplaceAll);
         Instance(typeof(IServiceRegistrar), _registrar, AppendValueMode.ReplaceAll);
         Instance(typeof(IServiceFacade), this, AppendValueMode.ReplaceAll);

         return _registrar;
      }
      #endregion

      #region Requester
      /// <inheritdoc/>
      public object Get(Type type) => _requester.Get(type);

      /// <inheritdoc/>
      public object? GetOptional(Type type) => _requester.GetOptional(type);

      /// <inheritdoc/>
      public IEnumerable<object> GetAll(Type type) => _requester.GetAll(type);
      #endregion

      #region Builder
      /// <inheritdoc/>
      public object Build(Type type) => _builder.Build(type);

      /// <inheritdoc/>
      public Func<object> BuildDelegate(Type type) => _builder.BuildDelegate(type);

      /// <inheritdoc/>
      public bool CanBuild(Type type) => _builder.CanBuild(type);

      /// <inheritdoc/>
      public ITypeExplanation? Explain(Type type) => _builder.Explain(type);
      #endregion

      /// <inheritdoc/>
      public bool IsRegistered(Type type)
      {
         if (_context.Registrations.Contains(type))
            return true;

         return _context.OuterContext?.Registrations.Contains(type) == true;
      }

      /// <inheritdoc/>
      public IServiceFacade CreateScope(AppendValueMode? defaultMode)
      {
         ServiceFacade scope = new ServiceFacade(defaultMode ?? DefaultRegistrationMode, _context);
         return scope;
      }

      /// <inheritdoc/>
      public void Dispose()
      {
         _registrar.Dispose();
         _builder.Dispose();
         _requester.Dispose();
         _context.Dispose();
      }
      #endregion
   }
}