using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Components;

namespace TNO.DependencyInjection;

/// <inheritdoc cref="IServiceFacade"/>
public sealed class ServiceFacade : IServiceFacade
{
   #region Properties
   /// <inheritdoc/>
   public AppendValueMode DefaultRegistrationMode { get; }
   #endregion

   #region Constructors
   /// <summary>Creates a new instance of the <see cref="ServiceFacade"/> with the given <paramref name="defaultRegistrationMode"/>.</summary>
   /// <param name="defaultRegistrationMode">The default mode to use when registering new services.</param>
   public ServiceFacade(AppendValueMode defaultRegistrationMode = AppendValueMode.ReplaceAll)
   {
      DefaultRegistrationMode = defaultRegistrationMode;
   }
   #endregion

   #region Methods
   /// <inheritdoc/>
   public IServiceScope CreateNew(AppendValueMode? defaultMode = null)
   {
      AppendValueMode mode = defaultMode ?? DefaultRegistrationMode;

      return new ServiceScope(null, mode);
   }
   #endregion
}