using TNO.Common.Extensions;

namespace TNO.DependencyInjection.Components.Registration;

internal sealed class SingletonRegistration : RegistrationBase
{
   #region Properties
   public ReaderWriterLockSlim Lock { get; }
   public Type Type { get; }
   public object? Instance { get; set; }
   #endregion
   public SingletonRegistration(Type type)
   {
      Type = type;
      Lock = new ReaderWriterLockSlim();
   }

   #region Methods
   public override void Dispose() => Instance?.TryDispose();
   #endregion
}