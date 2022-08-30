namespace TNO.DependencyInjection.Components.Registration
{
   internal sealed class PerRequestRegistration : RegistrationBase
   {
      #region Properties
      public Type Type { get; }
      #endregion
      public PerRequestRegistration(Type type) => Type = type;

      #region Methods
      public override void Dispose() { }
      #endregion
   }
}
