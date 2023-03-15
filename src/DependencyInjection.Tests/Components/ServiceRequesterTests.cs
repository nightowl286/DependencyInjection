using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Components;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Tests.Components;

[TestClass]
[TestCategory(Category.Requester)]
[TestCategory(Category.DependencyInjection)]
[TestCategory(Category.DependencyInjectionComponent)]
public class ServiceRequesterTests
{
   #region Fields
   private readonly ServiceScope _scope;
   private readonly ServiceRequester _sut;
   #endregion

   #region Dynamic Data Properties
   public static IEnumerable<object[]> GetAllRegistrationModes => DynamicDataProviders.GetAllRegistrationModes();
   #endregion
   public ServiceRequesterTests()
   {
      _scope = new ServiceScope(null, AppendValueMode.ReplaceAll);
      _sut = new ServiceRequester(_scope);
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
      _scope.Registrations.Add(type, new InstanceRegistration(expectedInstance));

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

      ServiceScope outerScope = new ServiceScope(null, AppendValueMode.ReplaceAll);
      outerScope.Registrations.Add(type, new InstanceRegistration(expectedInstance));

      ServiceRequester sut = new ServiceRequester(outerScope);

      // Act
      object instance = sut.Get(type);

      // Assert
      Assert.AreSame(expectedInstance, instance);
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
   public void Get_WithSingletonRegistration_ReturnsInstance()
   {
      // Arrange
      Type type = typeof(Class);
      Class expectedInstance = new Class();
      _scope.Registrations.Add(type, new SingletonRegistration(type) { Instance = expectedInstance });

      // Act
      object instance = _sut.Get(type);

      // Assert
      Assert.AreSame(expectedInstance, instance);
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

      _scope.Registrations.Add(genericType, registration);

      // Act
      object instance = _sut.Get(type);

      // Assert
      Assert.AreSame(expectedInstance, instance);
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

      _scope.Registrations.Add(type, new InstanceRegistration(expectedInstance));

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
      _scope.Registrations.Add(genericType, registration);

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

      ServiceScope outerScope = new ServiceScope(null, AppendValueMode.ReplaceAll);
      outerScope.Registrations.Add(type, new InstanceRegistration(expectedInstance));

      ServiceRequester sut = new ServiceRequester(outerScope);

      // Act
      object? instance = sut.GetOptional(type);

      // Assert
      Assert.AreSame(expectedInstance, instance);
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

      _scope.Registrations.Add(interfaceType, new InstanceRegistration(instance1), AppendValueMode.Append);
      _scope.Registrations.Add(interfaceType, new InstanceRegistration(instance2), AppendValueMode.Append);

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
   #endregion
}