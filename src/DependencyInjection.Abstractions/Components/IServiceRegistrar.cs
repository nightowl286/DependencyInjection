using System;

namespace TNO.DependencyInjection.Abstractions.Components
{
   public interface IServiceRegistrar : IServiceScope, IDisposable
   {
      #region Properties
      RegistrationMode DefaultRegistrationMode { get; }
      #endregion

      #region Methods
      IServiceRegistrar PerRequest(Type serviceType, Type concreteType, RegistrationMode? mode = null);
      IServiceRegistrar Singleton(Type serviceType, Type concreteType, RegistrationMode? mode = null);
      IServiceRegistrar Instance(Type serviceType, object instance, RegistrationMode? mode = null);
      IServiceRegistrar RegisterSelf();
      #endregion
   }
}
