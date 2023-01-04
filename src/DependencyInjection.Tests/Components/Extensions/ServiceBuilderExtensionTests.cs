using Moq;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Tests.Components.Extensions;

[TestClass]
[TestCategory(Category.Builder)]
[TestCategory(Category.Extensions)]
[TestCategory(Category.Dependency_Injection)]
public class ServiceBuilderExtensionTests
{
   #region Fields
   private readonly Mock<IServiceBuilder> _builderMock;
   private readonly IServiceBuilder _builder;
   #endregion
   public ServiceBuilderExtensionTests()
   {
      _builderMock = new Mock<IServiceBuilder>();
      _builder = _builderMock.Object;
   }

   #region Test Methods
   [TestMethod]
   public void Build_WithValidGenericType_RedirectsWithCorrectType()
   {
      // Arrange
      Type type = typeof(Class);

      // Act
      _ = _builder.Build<Class>();

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
      bool success = builderMock.Object.TryBuild(type, out object? instance);

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
      bool success = _builder.TryBuild(type, out object? instance);

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
      bool success = builderMock.Object.TryBuild(out Class? instance);

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
      bool success = _builder.TryBuild(out Class? instance);

      // Assert
      Assert.IsFalse(success);
      Assert.IsNull(instance);
      _builderMock.VerifyNever(b => b.Build(type));
   }
   #endregion
}