using Moq;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Tests
{
   [TestCategory(Category.Dependency_Injection)]
   [TestCategory(Category.Extensions)]
   [TestClass]
   public class DependencyInjectionExtensionTests
   {
      #region Fields
      private readonly Mock<IServiceRegistrar> _registrarMock;
      private readonly IServiceRegistrar _registrar;

      private readonly Mock<IServiceRequester> _requesterMock;
      private readonly IServiceRequester _requester;

      private readonly Mock<IServiceBuilder> _builderMock;
      private readonly IServiceBuilder _builder;
      #endregion

      #region Properties
      public static IEnumerable<object?[]> GetAllRegistrationModesAndNull => DynamicDataProviders.GetAllRegistrationModesAndNull();
      #endregion
      public DependencyInjectionExtensionTests()
      {
         _registrarMock = new Mock<IServiceRegistrar>();
         _registrar = _registrarMock.Object;

         _requesterMock = new Mock<IServiceRequester>();
         _requester = _requesterMock.Object;

         _builderMock = new Mock<IServiceBuilder>();
         _builder = _builderMock.Object;
      }

      #region Service Registrar
      #region Instance
      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void Instance_WithGenericService_RedirectsWithCorrectTypes(RegistrationMode registrationMode)
      {
         // Arrange
         Type serviceType = typeof(IInterfaceForClass);
         ClassWithInterface instance = new ClassWithInterface();

         // Act
         DependencyInjectionExtensions.Instance<IInterfaceForClass>(_registrar, instance, registrationMode);

         // Assert
         _registrarMock.VerifyOnce(f => f.Instance(serviceType, instance, registrationMode));
      }

      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void Instance_WithInstance_RedirectsWithCorrectTypes(RegistrationMode registrationMode)
      {
         // Arrange
         Type concreteType = typeof(ClassWithInterface);
         object instance = new ClassWithInterface();

         // Act
         DependencyInjectionExtensions.Instance(_registrar, instance, registrationMode);

         // Assert
         _registrarMock.VerifyOnce(f => f.Instance(concreteType, instance, registrationMode));
      }
      #endregion

      #region Per Request
      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void PerRequest_WithGenericServiceAndConcrete_RedirectsWithCorrectTypes(RegistrationMode registrationMode)
      {
         // Arrange
         Type serviceType = typeof(IInterfaceForClass);
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.PerRequest<IInterfaceForClass, ClassWithInterface>(_registrar, registrationMode);

         // Assert
         _registrarMock.VerifyOnce(f => f.PerRequest(serviceType, concreteType, registrationMode));
      }

      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void PerRequest_WithGenericConcrete_RedirectsWithCorrectTypes(RegistrationMode registrationMode)
      {
         // Arrange
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.PerRequest<ClassWithInterface>(_registrar, registrationMode);

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
         DependencyInjectionExtensions.PerRequestIfMissing(_registrar, serviceType, concreteType);

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
         DependencyInjectionExtensions.PerRequestIfMissing(registrar, serviceType, concreteType);

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
         DependencyInjectionExtensions.PerRequestIfMissing<IInterfaceForClass, ClassWithInterface>(_registrar);

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
         DependencyInjectionExtensions.PerRequestIfMissing<IInterfaceForClass, ClassWithInterface>(registrar);

         // Assert
         _registrarMock.VerifyNever(f => f.PerRequest(serviceType, concreteType, null));
      }

      [TestMethod]
      public void PerRequestIfMissing_WithConcrete_Successful()
      {
         // Assert
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.PerRequestIfMissing(_registrar, concreteType);

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
         DependencyInjectionExtensions.PerRequestIfMissing(registrar, concreteType);

         // Assert
         _registrarMock.VerifyNever(f => f.PerRequest(concreteType, concreteType, null));
      }

      [TestMethod]
      public void PerRequestIfMissing_WithGenericConcrete_Successful()
      {
         // Assert
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.PerRequestIfMissing<ClassWithInterface>(_registrar);

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
         DependencyInjectionExtensions.PerRequestIfMissing<ClassWithInterface>(registrar);

         // Assert
         _registrarMock.VerifyNever(f => f.PerRequest(concreteType, concreteType, null));
      }
      #endregion

      #region Singleton
      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void Singleton_WithGenericServiceAndConcrete_RedirectsWithCorrectTypes(RegistrationMode? registrationMode)
      {
         // Arrange
         Type serviceType = typeof(IInterfaceForClass);
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.Singleton<IInterfaceForClass, ClassWithInterface>(_registrar, registrationMode);

         // Assert
         _registrarMock.VerifyOnce(f => f.Singleton(serviceType, concreteType, registrationMode));
      }

      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void Singleton_WithGenericConcrete_RedirectsWithCorrectTypes(RegistrationMode? registrationMode)
      {
         // Arrange
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.Singleton<ClassWithInterface>(_registrar, registrationMode);

         // Assert
         _registrarMock.VerifyOnce(f => f.Singleton(concreteType, concreteType, registrationMode));
      }

      [DynamicData(nameof(GetAllRegistrationModesAndNull))]
      [TestMethod]
      public void Singleton_WithConcrete_RedirectsWithCorrectTypes(RegistrationMode? registrationMode)
      {
         // Arrange
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.Singleton(_registrar, concreteType, registrationMode);

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
         DependencyInjectionExtensions.SingletonIfMissing(_registrar, serviceType, concreteType);

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
         DependencyInjectionExtensions.SingletonIfMissing(registrar, serviceType, concreteType);

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
         DependencyInjectionExtensions.SingletonIfMissing<IInterfaceForClass, ClassWithInterface>(_registrar);

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
         DependencyInjectionExtensions.SingletonIfMissing<IInterfaceForClass, ClassWithInterface>(registrar);

         // Assert
         _registrarMock.VerifyNever(f => f.Singleton(serviceType, concreteType, null));
      }

      [TestMethod]
      public void SingletonIfMissing_WithConcrete_Successful()
      {
         // Assert
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.SingletonIfMissing(_registrar, concreteType);

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
         DependencyInjectionExtensions.SingletonIfMissing(registrar, concreteType);

         // Assert
         _registrarMock.VerifyNever(f => f.Singleton(concreteType, concreteType, null));
      }

      [TestMethod]
      public void SingletonIfMissing_WithGenericConcrete_Successful()
      {
         // Assert
         Type concreteType = typeof(ClassWithInterface);

         // Act
         DependencyInjectionExtensions.SingletonIfMissing<ClassWithInterface>(_registrar);

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
         DependencyInjectionExtensions.SingletonIfMissing<ClassWithInterface>(registrar);

         // Assert
         _registrarMock.VerifyNever(f => f.Singleton(concreteType, concreteType, null));
      }
      #endregion
      #endregion

      #region Service Requester
      [TestMethod]
      public void Get_WithGenericType_RedirectsWithCorrectType()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         DependencyInjectionExtensions.Get<Class>(_requester);

         // Assert
         _requesterMock.VerifyOnce(r => r.Get(type));
      }

      [TestMethod]
      public void GetOptional_WithGenericType_RedirectsWithCorrectType()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();
         Mock<IServiceRequester> requesterMock = new Mock<IServiceRequester>();
         requesterMock.Setup(r => r.GetOptional(type)).Returns(expectedInstance);

         // Act
         Class? instance =  DependencyInjectionExtensions.GetOptional<Class>(requesterMock.Object);

         // Assert
         requesterMock.VerifyOnce(r => r.GetOptional(type));
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void GetOptional_WithMissingGenericType_ReturnsNull()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         Class? instance = DependencyInjectionExtensions.GetOptional<Class>(_requester);

         // Assert
         _requesterMock.VerifyOnce(r => r.GetOptional(type));
         Assert.IsNull(instance);
      }

      [TestMethod]
      public void TryGet_WithMissingType_RedirectsWithCorrectTypeAndReturnsFalseAndNull()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         bool success = DependencyInjectionExtensions.TryGet(_requester, type, out object? instance);

         // Assert
         _requesterMock.VerifyOnce(r => r.GetOptional(type));
         Assert.IsFalse(success);
         Assert.IsNull(instance);
      }

      [TestMethod]
      public void TryGet_WithValidType_RedirectsWithCorrectTypeAndReturnsTrueAndValidInstance()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceRequester> requesterMock = new Mock<IServiceRequester>();
         requesterMock.Setup(r => r.GetOptional(type)).Returns(expectedInstance);

         // Act
         bool success = DependencyInjectionExtensions.TryGet(requesterMock.Object, type, out object? instance);

         // Assert
         requesterMock.VerifyOnce(r => r.GetOptional(type));
         Assert.IsTrue(success);
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void TryGet_WithMissingGenericType_RedirectsWithCorrectTypeAndReturnsFalseAndNull()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         bool success = DependencyInjectionExtensions.TryGet(_requester, out Class? instance);

         // Assert
         _requesterMock.VerifyOnce(r => r.GetOptional(type));
         Assert.IsFalse(success);
         Assert.IsNull(instance);
      }

      [TestMethod]
      public void TryGet_WithValidGenericType_RedirectsWithCorrectTypeAndReturnsTrueAndValidInstance()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceRequester> requesterMock = new Mock<IServiceRequester>();
         requesterMock.Setup(r => r.GetOptional(type)).Returns(expectedInstance);

         // Act
         bool success = DependencyInjectionExtensions.TryGet(requesterMock.Object, out Class? instance);

         // Assert
         requesterMock.VerifyOnce(r => r.GetOptional(type));
         Assert.IsTrue(success);
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void GetAll_WithValidGenericType_RedirectsWithCorrectType()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceRequester> requesterMock = new Mock<IServiceRequester>();
         requesterMock.Setup(r => r.GetAll(type)).Returns(new[] { expectedInstance });

         // Act
         IEnumerable<Class> instances = DependencyInjectionExtensions.GetAll<Class>(requesterMock.Object);

         // Pre Assert
         Class[] array = instances.ToArray();

         // Assert
         requesterMock.VerifyOnce(r => r.GetAll(type));
         Assert.IsTrue(array.Length == 1);
         Assert.AreSame(expectedInstance, array[0]);
      }
      #endregion

      #region Service Builder
      [TestMethod]
      public void Build_WithValidGenericType_RedirectsWithCorrectType()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         _ = DependencyInjectionExtensions.Build<Class>(_builder);

         // Assert
         _builderMock.VerifyOnce(b => b.Build(type));
      }

      [TestMethod]
      public void TryBuild_WithValidType_RedirectsWithCorrectTypeAndReturnsTrueAndCorrectInstance()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceBuilder> builderMock = new Mock<IServiceBuilder>();
         builderMock.Setup(b => b.CanBuild(type)).Returns(true);
         builderMock.Setup(b => b.Build(type)).Returns(expectedInstance);

         // Act
         bool success = DependencyInjectionExtensions.TryBuild(builderMock.Object, type, out object? instance);

         // Assert
         Assert.IsTrue(success);
         Assert.AreSame(expectedInstance, instance);
         builderMock.VerifyOnce(b => b.Build(type));
      }

      [TestMethod]
      public void TryBuild_WithInvalidType_ReturnsFalseAndNull()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         bool success = DependencyInjectionExtensions.TryBuild(_builder, type, out object? instance);

         // Assert
         Assert.IsFalse(success);
         Assert.IsNull(instance);
         _builderMock.VerifyNever(b => b.Build(type));
      }

      [TestMethod]
      public void TryBuild_WithValidGenericType_RedirectsWithCorrectTypeAndReturnsTrueAndCorrectInstance()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceBuilder> builderMock = new Mock<IServiceBuilder>();
         builderMock.Setup(b => b.CanBuild(type)).Returns(true);
         builderMock.Setup(b => b.Build(type)).Returns(expectedInstance);

         // Act
         bool success = DependencyInjectionExtensions.TryBuild(builderMock.Object, out Class? instance);

         // Assert
         Assert.IsTrue(success);
         Assert.AreSame(expectedInstance, instance);
         builderMock.VerifyOnce(b => b.Build(type));
      }

      [TestMethod]
      public void TryBuild_WithInvalidGenericType_ReturnsFalseAndNull()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         bool success = DependencyInjectionExtensions.TryBuild(_builder, out Class? instance);

         // Assert
         Assert.IsFalse(success);
         Assert.IsNull(instance);
         _builderMock.VerifyNever(b => b.Build(type));
      }
      #endregion
   }
}
