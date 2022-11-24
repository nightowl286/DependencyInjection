using System;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Abstractions.Components
{
   /// <summary>
   /// Denotes a builder that is capable of injecting 
   /// registered services into constructors.
   /// </summary>
   public interface IServiceBuilder : IDisposable
   {
      #region Methods
      /// <summary>
      /// Generates an explanation as to why an instance of the 
      /// given <paramref name="type"/> cannot be built.
      /// </summary>
      /// <param name="type">The type to try and explain.</param>
      /// <returns>
      /// An explanation as to why the given <paramref name="type"/> cannot 
      /// be built, or <see langword="null"/> if there are no problems.
      /// </returns>
      ITypeExplanation? Explain(Type type);

      /// <summary>Build an instance of the given <paramref name="type"/>.</summary>
      /// <param name="type">The type to build.</param>
      /// <returns>An instance of the given <paramref name="type"/>.</returns>
      object Build(Type type);

      /// <summary>
      /// Generates a <see cref="Func{TResult}"/> delegate that can be invoke 
      /// to build an instance of the given <paramref name="type"/>.
      /// </summary>
      /// <param name="type">The type that the delegate will build.</param>
      /// <returns>
      /// A delegate that can be used to build an instance 
      /// of the given <paramref name="type"/>.
      /// </returns>
      Func<object> BuildDelegate(Type type);

      /// <summary>Check whether the given <paramref name="type"/> can be built.</summary>
      /// <param name="type">The type to check.</param>
      /// <returns>
      /// <see langword="true"/> if the given <paramref name="type"/> 
      /// can be built, <see langword="false"/> otherwise.
      /// </returns>
      bool CanBuild(Type type);
      #endregion
   }
}
