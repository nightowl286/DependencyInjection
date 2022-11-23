using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TNO.DependencyInjection.Abstractions.Components
{
   /// <summary>
   /// Contains extension methods for the <see cref="IServiceRequester"/> type.
   /// </summary>
   public static class ServiceRequesterExtensions
   {
      /// <summary>Requests an instance that was registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the service to request.</typeparam>
      /// <param name="requester">The requester instance to use.</param>
      /// <returns>An instance of the registered service/</returns>
      public static T Get<T>(this IServiceRequester requester) where T : notnull
         => (T)requester.Get(typeof(T));

      /// <summary>Attempts to request an instance that was registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the service to request.</typeparam>
      /// <param name="requester">The requester instance to use.</param>
      /// <returns>
      /// An instance of the registered service, or <see langword="null"/>
      /// if the type <typeparamref name="T"/> has not been registered.
      /// </returns>
      public static T? GetOptional<T>(this IServiceRequester requester) where T : notnull
         => (T?)requester.GetOptional(typeof(T));

      /// <summary>Tries to get an instance registered for the given <paramref name="type"/>.</summary>
      /// <param name="requester">The requester instance to use.</param>
      /// <param name="type">The type to try and request.</param>
      /// <param name="instance">
      /// An instance of the given <paramref name="type"/>, or 
      /// <see langword="null"/> if an instance couldn't be obtained.
      /// </param>
      /// <returns><see langword="true"/> if an instance was obtained, <see langword="false"/> otherwise.</returns>
      public static bool TryGet(this IServiceRequester requester, Type type, [NotNullWhen(true)] out object? instance)
      {
         instance = requester.GetOptional(type);
         return instance is not null;
      }

      /// <summary>Tries to get an instance registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type to try and request.</typeparam>
      /// <param name="requester">The requester instance to use.</param>
      /// <param name="instance">
      /// An instance of the type <typeparamref name="T"/> or 
      /// <see langword="null"/> if an instance couldn't be obtained.
      /// </param>
      /// <returns><see langword="true"/> if an instance was obtained, <see langword="false"/> otherwise.</returns>
      public static bool TryGet<T>(this IServiceRequester requester, [NotNullWhen(true)] out T? instance)
         where T : notnull
      {
         instance = (T?)requester.GetOptional(typeof(T));
         return instance is not null;
      }

      /// <summary>Requests all the instances registered for the type <typeparamref name="T"/>.</summary>
      /// <typeparam name="T">The type of the services to request.</typeparam>
      /// <param name="requester">The requester instance.</param>
      /// <returns>A collection of the registered instances.</returns>
      public static IEnumerable<T> GetAll<T>(this IServiceRequester requester) where T : notnull
      {
         IEnumerable<object> all = requester.GetAll(typeof(T));
         foreach (object obj in all)
            yield return (T)obj;
      }
   }
}
