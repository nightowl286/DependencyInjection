namespace TNO.DependencyInjection.Components.Registration
{
   internal sealed class PerRequestRegistration : RegistrationBase
   {
      #region Properties
      public Type Type { get; }
      public Dictionary<Type, Func<object>> Optimisations { get; } = new Dictionary<Type, Func<object>>();
      #endregion
      public PerRequestRegistration(Type type) => Type = type;

      #region Methods
      public override void Dispose() { }
      public override void RemoveOptimisations() => Optimisations.Clear();
      #endregion
   }
}
