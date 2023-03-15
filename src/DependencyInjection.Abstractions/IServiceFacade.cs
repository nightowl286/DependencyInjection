using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Abstractions;

/// <summary>
/// Denotes that the implementing type bundles up all the components of a dependency injection system.
/// </summary>
public interface IServiceFacade
{
   #region Properties
   /// <summary>The default registration mode that will be used when creating new scopes.</summary>
   AppendValueMode DefaultRegistrationMode { get; }
   #endregion

   #region Methods
   /// <summary>Creates a new service scope.</summary>
   /// <param name="defaultRegistrationMode">
   /// The default registration mode in this scope, if <see langword="null"/>
   /// then the <see cref="DefaultRegistrationMode"/> will be used.
   /// </param>
   /// <returns>The newly created service scope.</returns>
   IServiceScope CreateNew(AppendValueMode? defaultRegistrationMode = null);
   #endregion
}