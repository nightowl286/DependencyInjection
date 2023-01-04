using Moq;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Tests.Components.Extensions;

[TestClass]
[TestCategory(Category.Registrar)]
[TestCategory(Category.Extensions)]
[TestCategory(Category.Dependency_Injection)]
public class ServiceRegistrarExtensionTests
{
   #region Fields
   private readonly Mock<IServiceRegistrar> _registrarMock;
   private readonly IServiceRegistrar _registrar;
   #endregion

   #region Properties
   public static IEnumerable<object?[]> GetAllRegistrationModesAndNull => DynamicDataProviders.GetAllRegistrationModesAndNull();
   #endregion
   public ServiceRegistrarExtensionTests()
   {
      _registrarMock = new Mock<IServiceRegistrar>();
      _registrar = _registrarMock.Object;
   }

   #region Test Methods
   #region Instance
   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void Instance_WithGenericService_RedirectsWithCorrectTypes(AppendValueMode registrationMode)
   {
      // Arrange
      Type serviceType = typeof(IInterfaceForClass);
      ClassWithInterface instance = new ClassWithInterface();

      // Act
      ServiceRegistrarExtensions.Instance<IInterfaceForClass>(_registrar, instance, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.Instance(serviceType, instance, registrationMode));
   }

   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void Instance_WithInstance_RedirectsWithCorrectTypes(AppendValueMode registrationMode)
   {
      // Arrange
      Type concreteType = typeof(ClassWithInterface);
      object instance = new ClassWithInterface();

      // Act
      ServiceRegistrarExtensions.Instance(_registrar, instance, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.Instance(concreteType, instance, registrationMode));
   }
   #endregion

   #region Per Request
   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void PerRequest_WithGenericServiceAndConcrete_RedirectsWithCorrectTypes(AppendValueMode registrationMode)
   {
      // Arrange
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.PerRequest<IInterfaceForClass, ClassWithInterface>(_registrar, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.PerRequest(serviceType, concreteType, registrationMode));
   }

   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void PerRequest_WithGenericConcrete_RedirectsWithCorrectTypes(AppendValueMode registrationMode)
   {
      // Arrange
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.PerRequest<ClassWithInterface>(_registrar, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.PerRequest(concreteType, concreteType, registrationMode));
   }
   #endregion

   #region Per Request If Missing
   [TestMethod]
   public void PerRequestIfMissing_WithServiceAndConcrete_Successful()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing(_registrar, serviceType, concreteType);

      // Assert
      _registrarMock.VerifyOnce(f => f.PerRequest(serviceType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithServiceAndConcrete_NoAction()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(serviceType) == true);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing(registrar, serviceType, concreteType);

      // Assert
      _registrarMock.VerifyNever(f => f.PerRequest(serviceType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithGenericServiceAndConcrete_Successful()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing<IInterfaceForClass, ClassWithInterface>(_registrar);

      // Assert
      _registrarMock.VerifyOnce(f => f.PerRequest(serviceType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithGenericServiceAndConcrete_NoAction()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(serviceType) == true);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing<IInterfaceForClass, ClassWithInterface>(registrar);

      // Assert
      _registrarMock.VerifyNever(f => f.PerRequest(serviceType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithConcrete_Successful()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing(_registrar, concreteType);

      // Assert
      _registrarMock.VerifyOnce(f => f.PerRequest(concreteType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithConcrete_NoAction()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(concreteType) == true);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing(registrar, concreteType);

      // Assert
      _registrarMock.VerifyNever(f => f.PerRequest(concreteType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithGenericConcrete_Successful()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing<ClassWithInterface>(_registrar);

      // Assert
      _registrarMock.VerifyOnce(f => f.PerRequest(concreteType, concreteType, null));
   }

   [TestMethod]
   public void PerRequestIfMissing_WithGenericConcrete_NoAction()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(concreteType) == true);

      // Act
      ServiceRegistrarExtensions.PerRequestIfMissing<ClassWithInterface>(registrar);

      // Assert
      _registrarMock.VerifyNever(f => f.PerRequest(concreteType, concreteType, null));
   }
   #endregion

   #region Singleton
   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void Singleton_WithGenericServiceAndConcrete_RedirectsWithCorrectTypes(AppendValueMode? registrationMode)
   {
      // Arrange
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.Singleton<IInterfaceForClass, ClassWithInterface>(_registrar, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(serviceType, concreteType, registrationMode));
   }

   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void Singleton_WithGenericConcrete_RedirectsWithCorrectTypes(AppendValueMode? registrationMode)
   {
      // Arrange
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.Singleton<ClassWithInterface>(_registrar, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(concreteType, concreteType, registrationMode));
   }

   [DynamicData(nameof(GetAllRegistrationModesAndNull))]
   [TestMethod]
   public void Singleton_WithConcrete_RedirectsWithCorrectTypes(AppendValueMode? registrationMode)
   {
      // Arrange
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.Singleton(_registrar, concreteType, registrationMode);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(concreteType, concreteType, registrationMode));
   }
   #endregion

   #region Singleton If Missing
   [TestMethod]
   public void SingletonIfMissing_WithServiceAndConcrete_Successful()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing(_registrar, serviceType, concreteType);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(serviceType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithServiceAndConcrete_NoAction()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(serviceType) == true);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing(registrar, serviceType, concreteType);

      // Assert
      _registrarMock.VerifyNever(f => f.Singleton(serviceType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithGenericServiceAndConcrete_Successful()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing<IInterfaceForClass, ClassWithInterface>(_registrar);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(serviceType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithGenericServiceAndConcrete_NoAction()
   {
      // Assert
      Type serviceType = typeof(IInterfaceForClass);
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(serviceType) == true);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing<IInterfaceForClass, ClassWithInterface>(registrar);

      // Assert
      _registrarMock.VerifyNever(f => f.Singleton(serviceType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithConcrete_Successful()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing(_registrar, concreteType);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(concreteType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithConcrete_NoAction()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(concreteType) == true);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing(registrar, concreteType);

      // Assert
      _registrarMock.VerifyNever(f => f.Singleton(concreteType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithGenericConcrete_Successful()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing<ClassWithInterface>(_registrar);

      // Assert
      _registrarMock.VerifyOnce(f => f.Singleton(concreteType, concreteType, null));
   }

   [TestMethod]
   public void SingletonIfMissing_WithGenericConcrete_NoAction()
   {
      // Assert
      Type concreteType = typeof(ClassWithInterface);
      IServiceRegistrar registrar = Mock.Of<IServiceRegistrar>(r => r.IsRegistered(concreteType) == true);

      // Act
      ServiceRegistrarExtensions.SingletonIfMissing<ClassWithInterface>(registrar);

      // Assert
      _registrarMock.VerifyNever(f => f.Singleton(concreteType, concreteType, null));
   }
   #endregion
   #endregion
}