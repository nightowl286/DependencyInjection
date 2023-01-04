using System;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Abstractions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceFacade"/> type.
/// </summary>
public static class ServiceFacadeExtensions
{
   #region Facade
   /// <summary>
   /// Try to obtain an instance registered with the given <paramref name="type"/>, 
   /// if that fails, build an instance of the given <paramref name="type"/>.
   /// </summary>
   /// <param name="facade">The facade instance to use.</param>
   /// <param name="type">The type to request, or build.</param>
   /// <returns>An instance of the given <paramref name="type"/>.</returns>
   public static object GetOrBuild(this IServiceFacade facade, Type type)
   {
      if (ServiceRequesterExtensions.TryGet(facade, type, out object? instance))
         return instance;

      return facade.Build(type);
   }

   /// <summary>
   /// Try to obtain an instance registered with the type <typeparamref name="T"/>,
   /// if that fails, build an instance of type <typeparamref name="T"/>.
   /// </summary>
   /// <typeparam name="T">The type to request, or build.</typeparam>
   /// <param name="facade">The facade instance to use.</param>
   /// <returns>An instance of type <typeparamref name="T"/>.</returns>
   public static T GetOrBuild<T>(this IServiceFacade facade) where T : notnull
      => (T)GetOrBuild(facade, typeof(T));
   #endregion
}