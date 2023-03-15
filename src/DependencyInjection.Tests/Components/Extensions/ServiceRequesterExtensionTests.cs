using Moq;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Tests.Components.Extensions;

[TestClass]
[TestCategory(Category.Requester)]
[TestCategory(Category.Extensions)]
[TestCategory(Category.DependencyInjection)]
public class ServiceRequesterExtensionTests
{
   #region Fields
   private readonly Mock<IServiceRequester> _requesterMock;
   private readonly IServiceRequester _requester;
   #endregion
   public ServiceRequesterExtensionTests()
   {
      _requesterMock = new Mock<IServiceRequester>();
      _requester = _requesterMock.Object;
   }

   #region Test Methods
   [TestMethod]
   public void Get_WithGenericType_RedirectsWithCorrectType()
   {
      // Arrange
      Type type = typeof(Class);

      // Act
      ServiceRequesterExtensions.Get<Class>(_requester);

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
      Class? instance = ServiceRequesterExtensions.GetOptional<Class>(requesterMock.Object);

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
      Class? instance = ServiceRequesterExtensions.GetOptional<Class>(_requester);

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
      bool success = ServiceRequesterExtensions.TryGet(_requester, type, out object? instance);

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
      bool success = ServiceRequesterExtensions.TryGet(requesterMock.Object, type, out object? instance);

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
      bool success = ServiceRequesterExtensions.TryGet(_requester, out Class? instance);

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
      bool success = ServiceRequesterExtensions.TryGet(requesterMock.Object, out Class? instance);

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
      IEnumerable<Class> instances = ServiceRequesterExtensions.GetAll<Class>(requesterMock.Object);

      // Pre-Assert
      Class[] array = instances.ToArray();

      // Assert
      requesterMock.VerifyOnce(r => r.GetAll(type));
      Assert.IsTrue(array.Length == 1);
      Assert.AreSame(expectedInstance, array[0]);
   }
   #endregion
}