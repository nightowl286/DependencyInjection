using TNO.Common.Extensions;

namespace TNO.DependencyInjection.Components.Registration;

internal sealed class InstanceRegistration : RegistrationBase
{
   #region Properties
   public object Instance { get; }
   #endregion
   public InstanceRegistration(object instance) => Instance = instance;

   #region Methods
   public override void Dispose() => Instance.TryDispose();
   #endregion
}