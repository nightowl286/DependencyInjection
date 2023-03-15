namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Contains useful extension methods related to the <see cref="IServiceRegistrationChecker"/>.
/// </summary>
public static class ServiceRegistrationCheckerExtensions
{
   #region Is Registered
   /// <summary>Checks if the type <typeparamref name="T"/> has been registered in the current scope/chain of scopes.</summary>
   /// <typeparam name="T">The type to check</typeparam>
   /// <param name="checker">The checker to use.</param>
   /// <returns>
   /// <see langword="true"/> if the type <typeparamref name="T"/>
   /// has been registered, <see langword="false"/> otherwise.
   /// </returns>
   public static bool IsRegistered<T>(this IServiceRegistrationChecker checker)
      => checker.IsRegistered(typeof(T));
   #endregion
}
