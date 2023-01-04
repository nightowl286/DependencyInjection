using Moq;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.Tests.Moq.DependencyInjection.Abstractions;

/// <summary>
/// Contains useful extension methods for the <see cref="Mock"/>&lt;<see cref="IServiceBuilder"/>&gt; class.
/// </summary>
public static class MockServiceBuilderExtensions
{
   #region Methods
   /// <summary>
   /// Sets up the given <paramref name="mock"/>
   /// to return the given <paramref name="explanation"/>
   /// for the given <paramref name="type"/>.
   /// </summary>
   /// <param name="mock">The mock to setup.</param>
   /// <param name="type">The type to return the <paramref name="explanation"/> for.</param>
   /// <param name="explanation">The explanation to return for the given <paramref name="type"/>.</param>
   /// <returns>The <paramref name="mock"/> instance that was given.</returns>
   public static Mock<IServiceBuilder> WithExplanation(this Mock<IServiceBuilder> mock, Type type, ITypeExplanation explanation)
   {
      mock.Setup(b => b.Explain(type)).Returns(explanation);
      return mock;
   }

   /// <summary>
   /// Sets up the given <paramref name="mock"/>
   /// with the given <paramref name="canBuild"/>
   /// result for the given <paramref name="type"/>.
   /// </summary>
   /// <param name="mock">The mock to setup.</param>
   /// <param name="type">The type to return the given <paramref name="canBuild"/> value for.</param>
   /// <param name="canBuild">Whether the given <paramref name="type"/> can be built.</param>
   /// <returns>The <paramref name="mock"/> instance that was given.</returns>
   public static Mock<IServiceBuilder> WithCanBuild(this Mock<IServiceBuilder> mock, Type type, bool canBuild)
   {
      mock.Setup(b => b.CanBuild(type)).Returns(canBuild);
      return mock;
   }

   /// <summary>
   /// Sets up the given <paramref name="mock"/> to return
   /// the given <paramref name="instance"/> when the given
   /// <paramref name="type"/> is requested to be built.
   /// </summary>
   /// <param name="mock">The mock to setup.</param>
   /// <param name="type">The type to return the <paramref name="instance"/> for.</param>
   /// <param name="instance">The instance to return for the given <paramref name="type"/>.</param>
   /// <returns>The <paramref name="mock"/> instance that was given.</returns>
   public static Mock<IServiceBuilder> WithBuild(this Mock<IServiceBuilder> mock, Type type, object instance)
   {
      mock.Setup(b => b.Build(type)).Returns(instance);
      return mock;
   }

   /// <summary>
   /// Sets up the given <paramref name="mock"/>
   /// to return the given <paramref name="buildDelegate"/>
   /// when a build delegate is requested for the given <paramref name="type"/>.
   /// </summary>
   /// <param name="mock">The mock to setup.</param>
   /// <param name="type">The type to return the <paramref name="buildDelegate"/> for.</param>
   /// <param name="buildDelegate">The build delegate to return for the given <paramref name="type"/>.</param>
   /// <returns>The <paramref name="mock"/> instance that was given.</returns>
   public static Mock<IServiceBuilder> WithDelegate(this Mock<IServiceBuilder> mock, Type type, Func<object> buildDelegate)
   {
      mock.Setup(b => b.BuildDelegate(type)).Returns(buildDelegate);
      return mock;
   }

   /// <summary>
   /// Sets up the given <paramref name="mock"/> to return a <see cref="Func{Object}"/>
   /// that will return the given <paramref name="instance"/> whenever a
   /// build delegate is requested for the given <paramref name="type"/>.
   /// </summary>
   /// <param name="mock">The mock to setup.</param>
   /// <param name="type">The type to return the <paramref name="instance"/> for.</param>
   /// <param name="instance">The instance that the build delegate will return for the given <paramref name="type"/>.</param>
   /// <returns>The <paramref name="mock"/> instance that was given.</returns>
   public static Mock<IServiceBuilder> WithDelegateInstance(this Mock<IServiceBuilder> mock, Type type, object instance)
   {
      object Action() => instance;

      mock.Setup(b => b.BuildDelegate(type)).Returns(Action);
      return mock;
   }
   #endregion
}