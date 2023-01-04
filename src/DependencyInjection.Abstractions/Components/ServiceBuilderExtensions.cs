using System;
using System.Diagnostics.CodeAnalysis;
using TNO.DependencyInjection.Abstractions.Explanations;

namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Contains extension methods for the <see cref="IServiceBuilder"/> type.
/// </summary>
public static class ServiceBuilderExtensions
{
   /// <summary>
   /// Generates an explanation as to why an instance of the type
   /// <typeparamref name="T"/> cannot be built.
   /// </summary>
   /// <typeparam name="T">The type to try and explain.</typeparam>
   /// <param name="builder">The builder instance to use.</param>
   /// <returns>
   /// An explanation as to why the type <typeparamref name="T"/> cannot
   /// be built, or <see langword="null"/> if there are no problems.
   /// </returns>
   public static ITypeExplanation? Explain<T>(this IServiceBuilder builder) => builder.Explain(typeof(T));

   /// <summary>Build an instance of the type <typeparamref name="T"/>.</summary>
   /// <typeparam name="T">The type to build.</typeparam>
   /// <param name="builder">The builder instance to use.</param>
   /// <returns>An instance of the type <typeparamref name="T"/>.</returns>
   public static T Build<T>(this IServiceBuilder builder) where T : notnull
      => (T)builder.Build(typeof(T));

   /// <summary>Tries to build an instance of the given <paramref name="type"/>.</summary>
   /// <param name="builder">The builder instance to use.</param>
   /// <param name="type">The type to try and build.</param>
   /// <param name="instance">
   /// The built instance, or <see langword="null"/> if an instance
   /// of the given <paramref name="type"/> could not be built.
   /// </param>
   /// <remarks>If building has failed, you can call <see cref="IServiceBuilder.Explain(Type)"/> to see the reason why.</remarks>
   /// <returns><see langword="true"/> if building was successful, <see langword="false"/> otherwise.</returns>
   public static bool TryBuild(this IServiceBuilder builder, Type type, [NotNullWhen(true)] out object? instance)
   {
      if (builder.CanBuild(type))
      {
         instance = builder.Build(type);
         return true;
      }

      instance = null;
      return false;
   }

   /// <summary>Tries to build an instance of the type <typeparamref name="T"/>.</summary>
   /// <typeparam name="T">The type to try and build.</typeparam>
   /// <param name="builder">The builder instance to use.</param>
   /// <param name="instance">
   /// The built instance, or <see langword="null"/> if an instance 
   /// of the type <typeparamref name="T"/> could not be built.
   /// </param>
   /// <remarks>If building has failed, you can call <see cref="Explain{T}(IServiceBuilder)"/> to see the reason why.</remarks>
   /// <returns><see langword="true"/> if building was successful, <see langword="false"/> otherwise.</returns>
   public static bool TryBuild<T>(this IServiceBuilder builder, [NotNullWhen(true)] out T? instance) where T : notnull
   {
      if (TryBuild(builder, typeof(T), out object? inst))
      {
         instance = (T)inst;
         return true;
      }

      instance = default;
      return false;
   }
}