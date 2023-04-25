namespace TNO.DependencyInjection.Abstractions.Components;

/// <summary>
/// Represents the result of trying to unlock the <see cref="IServiceRegistrar"/>.
/// </summary>
public enum UnlockResult : byte
{
   /// <summary>The registrar was already unlocked.</summary>
   AlreadyUnlocked,

   /// <summary>Couldn't unlock the registrar with the provided key.</summary>
   IncorrectKey,

   /// <summary>Unlocking the registrar was successful.</summary>
   Unlocked,
}
