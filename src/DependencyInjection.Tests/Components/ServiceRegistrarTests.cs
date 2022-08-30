﻿using Moq;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Components;
using TNO.DependencyInjection.Components.Registration;
using TNO.Tests.Moq.Extensions;

namespace TNO.DependencyInjection.Tests.Components
{
   [TestClass]
   [TestCategory(Category.Dependency_Injection)]
   [TestCategory(Category.Dependency_Injection_Component)]
   public class ServiceRegistrarTests
   {
      #region Fields
      private readonly Mock<IServiceFacade> _facadeMock;
      private readonly ServiceRegistrar _sut;
      private readonly ServiceContext _context;
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
         _facadeMock = new Mock<IServiceFacade>();

         _context = new ServiceContext(_facadeMock.Object, null);
         _sut = new ServiceRegistrar(_context);
      }

      #region Tests
      #region Instance
      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      [TestCategory(nameof(IServiceRegistrar.Instance))]
      public void Instance_ValidType_WithRegistrationMode_AddsCorrectRegistration(RegistrationMode registrationMode)
      {
         // Arrange
         object instance = new object();
         Type type = typeof(object);

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_context.Registrations.TryGet(type, out _));

         // Act
         _sut.Instance(type, instance, registrationMode);

         // Pre-Assert
         bool registered = _context.Registrations.TryGet(type, out RegistrationBase? registration);

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
      #endregion

      #region Per Request
      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      [TestCategory(nameof(IServiceRegistrar.PerRequest))]
      public void PerRequest_ValidType_WithRegistrationMode_AddsCorrectRegistration(RegistrationMode registrationMode)
      {
         // Arrange
         Type type = typeof(object);

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_context.Registrations.TryGet(type, out _));

         // Act
         _sut.PerRequest(type, type, registrationMode);

         // Pre-Assert
         bool registered = _context.Registrations.TryGet(type, out RegistrationBase? registration);

         // Assert
         Assert.IsTrue(registered);
         Assert.IsInstanceOfType(registration, typeof(PerRequestRegistration));
      }

      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      [TestCategory(nameof(IServiceRegistrar.PerRequest))]
      public void PerRequest_ValidGenericType_WithRegistrationMode_AddsCorrectRegistration(RegistrationMode registrationMode)
      {
         // Arrange
         Type type = typeof(GenericClass<>);

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_context.Registrations.TryGet(type, out _));

         // Act
         _sut.PerRequest(type, type, registrationMode);

         // Pre-Assert
         bool registered = _context.Registrations.TryGet(type, out RegistrationBase? registration);

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
      #endregion

      #region Singleton
      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      [TestCategory(nameof(IServiceRegistrar.Singleton))]
      public void Singleton_ValidType_WithRegistrationMode_AddsCorrectRegistration(RegistrationMode registrationMode)
      {
         // Arrange
         Type type = typeof(object);

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_context.Registrations.TryGet(type, out _));

         // Act
         _sut.Singleton(type, type, registrationMode);

         // Pre-Assert
         bool registered = _context.Registrations.TryGet(type, out RegistrationBase? registration);

         // Assert
         Assert.IsTrue(registered);
         Assert.IsInstanceOfType(registration, typeof(SingletonRegistration));
      }

      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      [TestCategory(nameof(IServiceRegistrar.Singleton))]
      public void Singleton_ValidGenericType_AddsCorrectRegistration(RegistrationMode registrationMode)
      {
         // Arrange
         Type type = typeof(GenericClass<>);

         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(_context.Registrations.TryGet(type, out _));

         // Act
         _sut.Singleton(type, type, registrationMode);

         // Pre-Assert
         bool registered = _context.Registrations.TryGet(type, out RegistrationBase? registration);

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
      #endregion

      #region Redirects
      [TestMethod]
      public void IsRegistered_RedirectsCallToFacade()
      {
         // Arrange
         Type type = typeof(object);

         // Act
         _ = _sut.IsRegistered(type);

         // Assert
         _facadeMock.VerifyOnce(f => f.IsRegistered(type));
      }

      [TestMethod]
      public void RegisterSelf_RedirectsCallToFacade()
      {
         // Act
         _sut.RegisterSelf();

         // Assert
         _facadeMock.VerifyOnce(f => f.RegisterSelf());
      }

      [DynamicData(nameof(GetAllRegistrationModes))]
      [TestMethod]
      public void CreateScope_WithCustomRegistrationMode_RedirectsCallToFacade(RegistrationMode registrationMode)
      {
         // Pre-Act Assert
         Assert.That.IsInconclusiveIf(registrationMode == _sut.DefaultRegistrationMode, $"The registration mode ({registrationMode}) cannot be tested as it is the default.");

         // Act
         _ = _sut.CreateScope(registrationMode);

         // Assert
         _facadeMock.VerifyOnce(f => f.CreateScope(registrationMode));
      }

      [TestMethod]
      public void CreateScope_WithDefaultRegistrationMode_RedirectsCallToFacade()
      {
         // Arrange
         RegistrationMode registrationMode = _sut.DefaultRegistrationMode;

         // Act
         _ = _sut.CreateScope();

         // Assert
         _facadeMock.VerifyOnce(f => f.CreateScope(registrationMode));
      }
      #endregion
      #endregion
   }
}