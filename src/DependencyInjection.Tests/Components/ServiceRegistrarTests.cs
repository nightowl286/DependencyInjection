using TNO.Common.Locking;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Exceptions;
using TNO.DependencyInjection.Components;
using TNO.DependencyInjection.Components.Registration;

namespace TNO.DependencyInjection.Tests.Components;

[TestClass]
[TestCategory(Category.Registrar)]
[TestCategory(Category.DependencyInjection)]
[TestCategory(Category.DependencyInjectionComponent)]
public class ServiceRegistrarTests
{
   #region Fields
   private readonly ServiceRegistrar _sut;
   private readonly ServiceScope _scope;
   #endregion

   #region Dynamic Data Properties
   public static IEnumerable<object[]> GetAllRegistrationModes => DynamicDataProviders.GetAllRegistrationModes();
   public static IEnumerable<object[]> GetInvalidTypeCombinations => DynamicDataProviders.GetInvalidTypeCombinations();
   public static IEnumerable<object[]> GetValidCombinations => DynamicDataProviders.GetValidCombinations();
   public static IEnumerable<object[]> GetInvalidCreateableCombinations => DynamicDataProviders.GetInvalidCreateableCombinations();
   public static IEnumerable<object[]> GetValidCreatableCombinations => DynamicDataProviders.GetValidCreatableCombinations();
   #endregion

   public ServiceRegistrarTests()
   {
      _scope = new ServiceScope(null, AppendValueMode.ReplaceAll);
      _sut = new ServiceRegistrar(_scope);
   }

   #region Tests
   #region Locking
   [TestMethod]
   public void TryLock_WhileUnlocked_ReturnsTrue()
   {
      // Pre-Act Assert
      Assert.That.IsInconclusiveIf(_sut.IsLocked);

      // Act
      bool result = _sut.TryLock(out ReferenceKey? key);

      // Assert
      Assert.IsTrue(result);
      Assert.IsTrue(_sut.IsLocked);
      Assert.IsNotNull(key);
   }

   [TestMethod]
   public void TryUnlock_WhileLockedWithCorrectKey_ReturnsTrue()
   {
      // Arrange
      ReferenceKey key = LockSut();

      // Act
      bool result = _sut.TryUnlock(key);

      // Assert
      Assert.IsTrue(result);
      Assert.IsFalse(_sut.IsLocked);
   }

   [TestMethod]
   public void TryLock_WhileLocked_ReturnsFalse()
   {
      // Arrange
      _ = LockSut();

      // Act
      bool result = _sut.TryLock(out ReferenceKey? key);

      // Assert
      Assert.IsFalse(result);
      Assert.IsTrue(_sut.IsLocked);
      Assert.IsNull(key);
   }

   [TestMethod]
   public void TryUnlock_WhileUnlocked_ReturnsFalse()
   {
      // Arrange
      ReferenceKey key = LockSut();
      bool arrangeResult = _sut.TryUnlock(key);

      // Arrange Assert
      Assert.That.IsInconclusiveIfNot(arrangeResult);
      Assert.That.IsInconclusiveIf(_sut.IsLocked);

      // Act
      bool result = _sut.TryUnlock(key);

      // Assert
      Assert.IsFalse(result);
      Assert.IsFalse(_sut.IsLocked);
   }

   [TestMethod]
   public void TryLock_AfterLockAndUnlock_ReturnsUniqueKey()
   {
      // Pre-Arrange Assert
      Assert.That.IsInconclusiveIf(_sut.IsLocked);

      // Arrange
      ReferenceKey key1 = LockSut();
      UnlockSut(key1);

      // Act
      bool result = _sut.TryLock(out ReferenceKey? key2);

      // Assert
      Assert.That.IsInconclusiveIfNot(result);
      Assert.That.IsInconclusiveIfNot(_sut.IsLocked);

      Assert.AreNotSame(key1, key2);
   }

   [TestMethod]
   public void TryUnlock_WithWrongKey_ReturnsFalse()
   {
      // Arrange
      ReferenceKey badKey = new ReferenceKey();
      ReferenceKey goodKey = LockSut();

      // Arrange Assert
      Assert.That.IsInconclusiveIf(ReferenceEquals(badKey, goodKey));

      // Act
      bool result = _sut.TryUnlock(badKey);

      // Assert
      Assert.IsFalse(result);
      Assert.IsTrue(_sut.IsLocked);
   }
   #endregion

   #region Instance
   [DynamicData(nameof(GetAllRegistrationModes))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Instance))]
   public void Instance_ValidType_WithRegistrationMode_AddsCorrectRegistration(AppendValueMode registrationMode)
   {
      // Arrange
      object instance = new object();
      Type type = typeof(object);

      // Pre-Act Assert
      Assert.That.IsInconclusiveIf(_scope.Registrations.TryGet(type, out _));

      // Act
      _sut.Instance(type, instance, registrationMode);

      // Pre-Assert
      bool registered = _scope.Registrations.TryGet(type, out RegistrationBase? registration);

      // Assert
      Assert.IsTrue(registered);
      Assert.IsInstanceOfType(registration, typeof(InstanceRegistration));
   }

   [DynamicData(
      nameof(GetInvalidCreateableCombinations),
      DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
      DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Instance))]
   public void Instance_IncorrectType_ThrowsArgumentException(Type serviceType, Type concreteType)
   {
      // Arrange
      object? instance = Activator.CreateInstance(concreteType);
      Assert.That.IsInconclusiveIf(instance is null);

      // Act
      void act() => _sut.Instance(serviceType, instance);

      // Assert
      Assert.That.ThrowsExceptionOfType<ArgumentException>(act);
   }

   [DynamicData(
      nameof(GetValidCreatableCombinations),
      DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
      DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Instance))]
   public void Instance_ValidTypeCombination_Success(Type serviceType, Type concreteType)
   {
      // Arrange
      object? instance = Activator.CreateInstance(concreteType);
      Assert.That.IsInconclusiveIf(instance is null);

      // Act
      void act() => _sut.Instance(serviceType, instance);

      // Assert
      Assert.That.NoExceptionIsThrown(act);
   }

   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Instance))]
   public void Instance_ValidGenericTypeCombination_Success()
   {
      // Arrange
      object? instance = new GenericClassWithInterface<int>();
      Type type = typeof(IGenericInterfaceForClass<int>);

      // Act
      void act() => _sut.Instance(type, instance);

      // Assert
      Assert.That.NoExceptionIsThrown(act);
   }

   [TestMethod]
   public void Instance_WhileLocked_ThrowsRegistrarLockedException()
   {
      // Arrange
      Type serviceType = typeof(object);
      object instance = new object();
      _ = LockSut();

      // Act
      void Act() => _sut.Instance(serviceType, instance);

      // Assert
      Assert.ThrowsException<RegistrarLockedException>(Act);
   }
   #endregion

   #region Per Request
   [DynamicData(nameof(GetAllRegistrationModes))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.PerRequest))]
   public void PerRequest_ValidType_WithRegistrationMode_AddsCorrectRegistration(AppendValueMode registrationMode)
   {
      // Arrange
      Type type = typeof(object);

      // Pre-Act Assert
      Assert.That.IsInconclusiveIf(_scope.Registrations.TryGet(type, out _));

      // Act
      _sut.PerRequest(type, type, registrationMode);

      // Pre-Assert
      bool registered = _scope.Registrations.TryGet(type, out RegistrationBase? registration);

      // Assert
      Assert.IsTrue(registered);
      Assert.IsInstanceOfType(registration, typeof(PerRequestRegistration));
   }

   [DynamicData(nameof(GetAllRegistrationModes))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.PerRequest))]
   public void PerRequest_ValidGenericType_WithRegistrationMode_AddsCorrectRegistration(AppendValueMode registrationMode)
   {
      // Arrange
      Type type = typeof(GenericClass<>);

      // Pre-Act Assert
      Assert.That.IsInconclusiveIf(_scope.Registrations.TryGet(type, out _));

      // Act
      _sut.PerRequest(type, type, registrationMode);

      // Pre-Assert
      bool registered = _scope.Registrations.TryGet(type, out RegistrationBase? registration);

      // Assert
      Assert.IsTrue(registered);
      Assert.IsInstanceOfType(registration, typeof(PerRequestRegistration));
   }

   [DynamicData(
      nameof(GetInvalidTypeCombinations),
      DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
      DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.PerRequest))]
   public void PerRequest_IncorrectType_ThrowsArgumentException(Type serviceType, Type concreteType)
   {
      // Act
      void act() => _sut.PerRequest(serviceType, concreteType);

      // Assert
      Assert.That.ThrowsExceptionOfType<ArgumentException>(act);
   }

   [DynamicData(
      nameof(GetValidCombinations),
      DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
      DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.PerRequest))]
   public void PerRequest_ValidTypeCombination_Success(Type serviceType, Type concreteType)
   {
      // Act
      void act() => _sut.PerRequest(serviceType, concreteType);

      // Assert
      Assert.That.NoExceptionIsThrown(act);
   }

   [TestMethod]
   public void PerRequest_WhileLocked_ThrowsRegistrarLockedException()
   {
      // Arrange
      Type serviceType = typeof(object);
      Type concreteType = typeof(object);
      _ = LockSut();

      // Act
      void Act() => _sut.PerRequest(serviceType, concreteType);

      // Assert
      Assert.ThrowsException<RegistrarLockedException>(Act);
   }
   #endregion

   #region Singleton
   [DynamicData(nameof(GetAllRegistrationModes))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Singleton))]
   public void Singleton_ValidType_WithRegistrationMode_AddsCorrectRegistration(AppendValueMode registrationMode)
   {
      // Arrange
      Type type = typeof(object);

      // Pre-Act Assert
      Assert.That.IsInconclusiveIf(_scope.Registrations.TryGet(type, out _));

      // Act
      _sut.Singleton(type, type, registrationMode);

      // Pre-Assert
      bool registered = _scope.Registrations.TryGet(type, out RegistrationBase? registration);

      // Assert
      Assert.IsTrue(registered);
      Assert.IsInstanceOfType(registration, typeof(SingletonRegistration));
   }

   [DynamicData(nameof(GetAllRegistrationModes))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Singleton))]
   public void Singleton_ValidGenericType_AddsCorrectRegistration(AppendValueMode registrationMode)
   {
      // Arrange
      Type type = typeof(GenericClass<>);

      // Pre-Act Assert
      Assert.That.IsInconclusiveIf(_scope.Registrations.TryGet(type, out _));

      // Act
      _sut.Singleton(type, type, registrationMode);

      // Pre-Assert
      bool registered = _scope.Registrations.TryGet(type, out RegistrationBase? registration);

      // Assert
      Assert.IsTrue(registered);
      Assert.IsInstanceOfType(registration, typeof(GenericSingletonRegistration));
   }

   [DynamicData(
      nameof(GetInvalidTypeCombinations),
      DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
      DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Singleton))]
   public void Singleton_IncorrectType_ThrowsArgumentException(Type serviceType, Type concreteType)
   {
      // Act
      void act() => _sut.Singleton(serviceType, concreteType);

      // Assert
      Assert.That.ThrowsExceptionOfType<ArgumentException>(act);
   }

   [DynamicData(
      nameof(GetValidCombinations),
      DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
      DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
   [TestMethod]
   [TestCategory(nameof(IServiceRegistrar.Singleton))]
   public void Singleton_ValidTypeCombination_Success(Type serviceType, Type concreteType)
   {
      // Act
      void act() => _sut.Singleton(serviceType, concreteType);

      // Assert
      Assert.That.NoExceptionIsThrown(act);
   }

   [TestMethod]
   public void Singleton_WhileLocked_ThrowsRegistrarLockedException()
   {
      // Arrange
      Type serviceType = typeof(object);
      Type concreteType = typeof(object);
      _ = LockSut();

      // Act
      void Act() => _sut.Singleton(serviceType, concreteType);

      // Assert
      Assert.ThrowsException<RegistrarLockedException>(Act);
   }
   #endregion

   [TestMethod]
   public void RegisterComponents_WhileLocked_ThrowsRegistrarLockedException()
   {
      // Arrange
      _ = LockSut();

      // Act
      void Act() => _sut.RegisterComponents();

      // Assert
      Assert.ThrowsException<RegistrarLockedException>(Act);
   }
   #endregion

   #region Helpers
   private ReferenceKey LockSut()
   {
      Assert.That.IsInconclusiveIf(_sut.IsLocked);

      bool result = _sut.TryLock(out ReferenceKey? key);

      Assert.That.IsInconclusiveIf(key is null);
      Assert.That.IsInconclusiveIfNot(result);
      Assert.That.IsInconclusiveIfNot(_sut.IsLocked);

      return key;
   }
   private void UnlockSut(ReferenceKey key)
   {
      Assert.That.IsInconclusiveIfNot(_sut.IsLocked);

      bool result = _sut.TryUnlock(key);

      Assert.That.IsInconclusiveIfNot(result);
      Assert.That.IsInconclusiveIf(_sut.IsLocked);
   }
   #endregion
}