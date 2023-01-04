namespace TNO.DependencyInjection.Components.Registration;

internal abstract class RegistrationBase : IDisposable
{
   #region Methods
   public abstract void Dispose();
   public virtual void RemoveOptimisations() { }
   #endregion
}