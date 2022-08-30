using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Explanations;
using TNO.DependencyInjection.Components;

namespace TNO.DependencyInjection.Tests.Components
{
   [TestClass]
   [TestCategory(Category.Dependency_Injection)]
   [TestCategory(Category.Dependency_Injection_Component)]
   public class ServiceBuilderTests // most tests will require new dynamic data
   {
      #region Fields
      private readonly IServiceFacade _facade;
      private readonly ServiceBuilder _sut;
      #endregion

      #region Properties
      public static IEnumerable<object[]> GetNonBuildableTypes => DynamicDataProviders.GetNonBuildableTypes();
      public static IEnumerable<object[]> GetBuildableTypes => DynamicDataProviders.GetBuildableTypes();
      #endregion
      public ServiceBuilderTests()
      {
         _facade = new ServiceFacade();
         ServiceContext context = new ServiceContext(_facade, null);

         _sut = new ServiceBuilder(context);
      }

      #region Tests
      #region Build
      [DynamicData(
         nameof(GetBuildableTypes),
         DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
         DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
      [TestMethod]
      public void Build_WithValidType_ReturnsInstanceOfCorrectType(Type type) 
      {
         // Arrange
         _facade.PerRequest<Class>();

         // Act
         object instance = _sut.Build(type);

         // Assert
         Assert.IsInstanceOfType(instance, type);
      }
      #endregion

      #region Can Build
      [DynamicData(
         nameof(GetBuildableTypes),
         DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
         DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
      [TestMethod]
      public void CanBuild_WithValidType_ReturnsTrue(Type type)
      {
         // Arrange
         _facade.PerRequest<Class>();

         // Act
         bool canBuild = _sut.CanBuild(type);

         // Assert
         Assert.IsTrue(canBuild);
      }

      [DynamicData(
         nameof(GetNonBuildableTypes),
         DynamicDataDisplayName = nameof(DynamicDataProviders.GetTypeNames),
         DynamicDataDisplayNameDeclaringType = typeof(DynamicDataProviders))]
      [TestMethod]
      public void CanBuild_WithInvalidType_ReturnsFalse(Type type)
      {
         // Act
         bool canBuild = _sut.CanBuild(type);

         // Assert
         Assert.IsFalse(canBuild);
      }
      #endregion

      #region Explain
      [DataRow(typeof(ClassWithEnumerableParameterType))]
      [DataRow(typeof(ClassWithValidNullableParameterType))]
      [TestMethod]
      public void Explain_WithValidType_ReturnsNull(Type type)
      {
         // Act
         ITypeExplanation? explanation = _sut.Explain(type);

         // Assert
         Assert.IsNull(explanation);
      }

      [TestMethod]
      public void Explain_WithValidRegisteredType_ReturnsNull() 
      {
         // Arrange
         Type type = typeof(ClassWithValidParameterType);
         _facade.PerRequest<Class>();

         // Act
         ITypeExplanation? explanation = _sut.Explain(type);

         // Assert
         Assert.IsNull(explanation);
      }

      [TestMethod]
      public void Explain_WithStructParameterType_CorrectExplanation()
      {
         // Assert
         Type type = typeof(ClassWithStructParameterType);
         Type expectedParameterType = typeof(Struct);

         // Act
         ITypeExplanation? explanation = _sut.Explain(type);

         // Assert
         Assert.IsNotNull(explanation);
         Assert.IsTrue(explanation.ConstructorExplanations.Count == 1);
         IConstructorExplanation constructor = explanation.ConstructorExplanations.First();

         Assert.IsTrue(constructor.ParameterExplanations.Count == 1);
         IParameterExplanation parameter = constructor.ParameterExplanations.First();

         Assert.IsNotNull(parameter.ParameterTypeExplanation);
         IParameterTypeExplanation parameterType = parameter.ParameterTypeExplanation;

         Type paramType = parameterType.Type;
         Assert.AreSame(expectedParameterType, paramType);
      }

      [TestMethod]
      public void Explain_WithMissingParameterType_CorrectExplanation()
      {
         // Assert
         Type type = typeof(ClassWithValidParameterType);
         Type expectedParameterType = typeof(Class);

         // Act
         ITypeExplanation? explanation = _sut.Explain(type);

         // Assert
         Assert.IsNotNull(explanation);
         Assert.IsTrue(explanation.ConstructorExplanations.Count == 1);
         IConstructorExplanation constructor = explanation.ConstructorExplanations.First();

         Assert.IsTrue(constructor.ParameterExplanations.Count == 1);
         IParameterExplanation parameter = constructor.ParameterExplanations.First();

         Assert.IsNotNull(parameter.ParameterTypeExplanation);
         IParameterTypeExplanation parameterType = parameter.ParameterTypeExplanation;

         Type paramType = parameterType.Type;
         Assert.AreSame(expectedParameterType, paramType);
      }

      [TestMethod]
      public void Explain_WithOutParameter_CorrectExplanation()
      {
         // Assert
         Type type = typeof(ClassWithOutParameterType);

         // Act
         ITypeExplanation? explanation = _sut.Explain(type);

         // Assert
         Assert.IsNotNull(explanation);
         Assert.IsTrue(explanation.ConstructorExplanations.Count == 1);
         IConstructorExplanation constructor = explanation.ConstructorExplanations.First();

         Assert.IsTrue(constructor.ParameterExplanations.Count == 1);
         IParameterExplanation parameter = constructor.ParameterExplanations.First();

         Assert.IsTrue(parameter.Parameter.IsOut);
      }

      [TestMethod]
      public void Explain_NoPublicConstructor_CorrectExplanation()
      {
         // Assert
         Type type = typeof(ClassWithNoPublicConstructor);

         // Act
         ITypeExplanation? explanation = _sut.Explain(type);

         // Assert
         Assert.IsNotNull(explanation);
         Assert.IsTrue(explanation.ConstructorExplanations.Count == 0);
      }
      #endregion
      #endregion
   }
}
