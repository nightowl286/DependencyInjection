using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Explanations;
using TNO.DependencyInjection.Components;

namespace TNO.DependencyInjection
{
   public sealed class ServiceFacade : IServiceFacade
   {
      #region Fields
      private readonly ServiceContext _context;
      private readonly ServiceRegistrar _registrar;
      private readonly ServiceBuilder _builder;
      private readonly ServiceRequester _requester;
      #endregion

      #region Properties
      public RegistrationMode DefaultRegistrationMode => _registrar.DefaultRegistrationMode;
      #endregion
      public ServiceFacade(RegistrationMode defaultMode = RegistrationMode.ReplaceAll) : this(defaultMode, null) { }
      private ServiceFacade(RegistrationMode defaultMode, ServiceContext? outerContext)
      {
         _context = new ServiceContext(this, outerContext);
         _registrar = new ServiceRegistrar(_context, defaultMode);
         _requester = new ServiceRequester(_context);
         _builder = new ServiceBuilder(_context);
      }

      #region Methods
      #region Registrar
      public IServiceRegistrar PerRequest(Type serviceType, Type concreteType, RegistrationMode? mode = null) => _registrar.PerRequest(serviceType, concreteType, mode);
      public IServiceRegistrar Singleton(Type serviceType, Type concreteType, RegistrationMode? mode = null) => _registrar.Singleton(serviceType, concreteType, mode);
      public IServiceRegistrar Instance(Type serviceType, object instance, RegistrationMode? mode = null) => _registrar.Instance(serviceType, instance, mode);
      public IServiceRegistrar RegisterSelf()
      {
         Instance(typeof(IServiceRequester), _requester, RegistrationMode.ReplaceAll);
         Instance(typeof(IServiceProvider), _requester, RegistrationMode.ReplaceAll);
         Instance(typeof(IServiceBuilder), _builder, RegistrationMode.ReplaceAll);
         Instance(typeof(IServiceRegistrar), _registrar, RegistrationMode.ReplaceAll);
         Instance(typeof(IServiceFacade), this, RegistrationMode.ReplaceAll);

         return _registrar;
      }
      #endregion

      #region Requester
      public object Get(Type type) => _requester.Get(type);
      public object? GetOptional(Type type) => _requester.GetOptional(type);
      public IEnumerable<object> GetAll(Type type) => _requester.GetAll(type);
      #endregion

      #region Builder
      public object Build(Type type) => _builder.Build(type);
      public bool CanBuild(Type type) => _builder.CanBuild(type);
      public ITypeExplanation? Explain(Type type) => _builder.Explain(type);
      #endregion

      public bool IsRegistered(Type type)
      {
         if (_context.Registrations.Contains(type))
            return true;

         return _context.OuterContext?.Registrations.Contains(type) == true;
      }
      public IServiceFacade CreateScope(RegistrationMode? defaultMode)
      {
         ServiceFacade scope = new ServiceFacade(defaultMode ?? DefaultRegistrationMode, _context);
         return scope;
      }
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