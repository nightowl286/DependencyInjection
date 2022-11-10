using Moq;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Components;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Tests.Components
{
   [TestClass]
   [TestCategory(Category.Dependency_Injection)]
   [TestCategory(Category.Dependency_Injection_Component)]
   public class ServiceRequesterTests
   {
      #region Fields
      private readonly Mock<IServiceFacade> _facadeMock;
      private readonly ServiceContext _context;
      private readonly ServiceRequester _sut;
      #endregion

      #region Dynamic Data Properties
      public static IEnumerable<object[]> GetAllRegistrationModes => DynamicDataProviders.GetAllRegistrationModes();
      #endregion
      public ServiceRequesterTests()
      {
         _facadeMock = new Mock<IServiceFacade>();
         _context = new ServiceContext(_facadeMock.Object, null);
         _sut = new ServiceRequester(_context);
      }

      #region Tests
      #region Get
      [TestMethod]
      public void Get_WithInvalidType_ThrowsArgumentException() 
      {
         // Arrange
         Type type = typeof(GenericClass<>);

         // Act
         void act() => _sut.Get(type);

         // Assert
         Assert.ThrowsException<ArgumentException>(act);
      }

      [TestMethod]
      public void Get_WithRegisteredType_ReturnsCorrectInstance() 
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();
         _context.Registrations.Add(type, new InstanceRegistration(expectedInstance));

         // Act
         object instance = _sut.Get(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void Get_WithMissingTypeAndOuterScope_CallsOuterScope()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceFacade> outerScope = new Mock<IServiceFacade>();
         outerScope.Setup(f => f.Get(type)).Returns(expectedInstance);

         ServiceContext outerContext = new ServiceContext(outerScope.Object, null);
         outerContext.Registrations.Add(type, new InstanceRegistration(expectedInstance));

         ServiceContext context = new ServiceContext(Mock.Of<IServiceFacade>(), outerContext);
         ServiceRequester sut = new ServiceRequester(context);

         // Act
         object instance = sut.Get(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
         outerScope.VerifyOnce(f => f.Get(type));
      }

      [TestMethod]
      public void Get_WithMissingType_ThrowsArgumentException()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         void act() => _sut.Get(type);

         // Assert
         Assert.ThrowsException<ArgumentException>(act);
      }

      [TestMethod]
      public void Get_WithPerRequestRegistration_BuildsInstance() 
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceFacade> facadeMock = new Mock<IServiceFacade>();
         facadeMock.Setup(f => f.Build(type)).Returns(expectedInstance);
         ServiceContext context = new ServiceContext(facadeMock.Object, null);

         context.Registrations.Add(type, new PerRequestRegistration(type));

         ServiceRequester sut = new ServiceRequester(context);

         // Act
         object instance = sut.Get(type);

         // Assert
         Assert.IsInstanceOfType(instance, type);
         facadeMock.Verify(f => f.Build(type));
      }

      [TestMethod]
      public void Get_WithSingletonRegistration_ReturnsInstance() 
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();
         _context.Registrations.Add(type, new SingletonRegistration(type) { Instance = expectedInstance });

         // Act
         object instance = _sut.Get(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void Get_WithSingletonRegistration_BuildsInstance() 
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceFacade> facadeMock = new Mock<IServiceFacade>();
         facadeMock.Setup(f => f.Build(type)).Returns(expectedInstance);
         ServiceContext context = new ServiceContext(facadeMock.Object, null);

         SingletonRegistration registration = new SingletonRegistration(type);
         context.Registrations.Add(type, registration);

         ServiceRequester sut = new ServiceRequester(context);

         // Act
         object instance = sut.Get(type);

         // Assert
         Assert.AreSame(instance, expectedInstance);
         Assert.IsNotNull(registration.Instance);
         facadeMock.Verify(f => f.Build(type));
      }

      [TestMethod]
      public void Get_WithGenericSingletonRegistration_ReturnsInstance()
      {
         // Arrange
         Type genericType = typeof(GenericClass<>);
         Type type = typeof(GenericClass<Class>);
         GenericClass<Class> expectedInstance = new GenericClass<Class>();

         GenericSingletonRegistration registration = new GenericSingletonRegistration(type);
         registration.Instances.Add(type, expectedInstance);

         _context.Registrations.Add(genericType, registration);

         // Act
         object instance = _sut.Get(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void Get_WithGenericSingletonRegistration_BuildsInstance()
      {
         // Arrange
         Type genericType = typeof(GenericClass<>);
         Type type = typeof(GenericClass<Class>);
         GenericClass<Class> expectedInstance = new GenericClass<Class>();

         Mock<IServiceFacade> facadeMock = new Mock<IServiceFacade>();
         facadeMock.Setup(f => f.Build(type)).Returns(expectedInstance);
         ServiceContext context = new ServiceContext(facadeMock.Object, null);

         GenericSingletonRegistration registration = new GenericSingletonRegistration(type);
         context.Registrations.Add(genericType, registration);

         ServiceRequester sut = new ServiceRequester(context);

         // Act
         object instance = sut.Get(type);

         // Assert
         Assert.IsNotNull(registration.Instances.TryGet(type, out object? storedInstance));
         Assert.AreSame(instance, storedInstance);
         facadeMock.Verify(f => f.Build(type));
      }
      #endregion


      #region Get Optional
      [TestMethod]
      public void GetOptional_InvalidType_ThrowsArgumentException()
      {
         // Arrange
         Type type = typeof(GenericClass<>);

         // Act
         void act() => _sut.GetOptional(type);


         // Assert
         Assert.ThrowsException<ArgumentException>(act);
      }

      [TestMethod]
      public void GetOptional_WithMissingType_ReturnsNull() 
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         object? instance = _sut.GetOptional(type);

         // Assert
         Assert.IsNull(instance);
      }

      [TestMethod]
      public void GetOptional_WithRegisteredType_ReturnsCorrectInstance()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         _context.Registrations.Add(type, new InstanceRegistration(expectedInstance));

         // Act
         object? instance = _sut.GetOptional(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void GetOptional_WithRegisteredGenericType_ReturnsCorrectInstance()
      {
         // Arrange
         Type genericType = typeof(GenericClass<>);
         Type type = typeof(GenericClass<Class>);
         GenericClass<Class> expectedInstance = new GenericClass<Class>();

         GenericSingletonRegistration registration = new GenericSingletonRegistration(genericType);
         registration.Instances.Add(type, expectedInstance);
         _context.Registrations.Add(genericType, registration);

         // Act
         object? instance = _sut.GetOptional(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
      }

      [TestMethod]
      public void GetOptional_MissingWithOuterContext_RedirectsToOuterContext()
      {
         // Arrange
         Type type = typeof(Class);
         Class expectedInstance = new Class();

         Mock<IServiceFacade> outerFacade = new Mock<IServiceFacade>();
         outerFacade.Setup(f => f.GetOptional(type)).Returns(expectedInstance);
         ServiceContext outerContext = new ServiceContext(outerFacade.Object, null);

         ServiceContext context = new ServiceContext(Mock.Of<IServiceFacade>(), outerContext);
         ServiceRequester sut = new ServiceRequester(context);

         // Act
         object? instance = sut.GetOptional(type);

         // Assert
         Assert.AreSame(expectedInstance, instance);
         outerFacade.VerifyOnce(f => f.GetOptional(type));
      }

      #endregion

      #region Get All
      [TestMethod]
      public void GetAll_InvalidType_ThrowsArgumentException()
      {
         // Arrange
         Type type = typeof(GenericClass<>);

         // Act
         void act() => _sut.GetAll(type);


         // Assert
         Assert.ThrowsException<ArgumentException>(act);
      }

      [TestMethod]
      public void GetAll_WithMissingType_ReturnsEmptyEnumerable()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         IEnumerable<object> instances = _sut.GetAll(type);

         // Assert
         CollectionAssert.That.IsEmpty(instances);
      }

      [TestMethod]
      public void GetAll_WithRegisteredTypes_ReturnsAllInstances()
      {
         // Arrange
         Type interfaceType = typeof(IInterfaceForClass);
         ClassWithInterface instance1 = new ClassWithInterface();
         AnotherClassWithInterface instance2 = new AnotherClassWithInterface();

         _context.Registrations.Add(interfaceType, new InstanceRegistration(instance1), RegistrationMode.Append);
         _context.Registrations.Add(interfaceType, new InstanceRegistration(instance2), RegistrationMode.Append);

         // Act
         IEnumerable<object> instances = _sut.GetAll(interfaceType);

         // Pre-Assert
         object[] array = instances.ToArray();

         // Assert
         Assert.IsTrue(array.Length == 2);
         Assert.AreSame(instance1, array[0]);
         Assert.AreSame(instance2, array[1]);
      }
      #endregion

      #region Redirects
      [TestMethod]
      public void IsRegistered_RedirectsCallToFacade()
      {
         // Arrange
         Type type = typeof(Class);

         // Act
         _ = _sut.IsRegistered(type);

         // Assert
         _facadeMock.VerifyOnce(f => f.IsRegistered(type));
      }

      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      public void CreateScope_WithCustomRegistrationMode_RedirectsCallToFacade(RegistrationMode registrationMode)
      {
         // Act
         _ = _sut.CreateScope(registrationMode);

         // Assert
         _facadeMock.VerifyOnce(f => f.CreateScope(registrationMode));
      }
      #endregion
      #endregion
   }
}
