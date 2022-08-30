namespace TNO.DependencyInjection.Components.Registration
{
   internal sealed class GenericSingletonRegistration : RegistrationBase
   {
      #region Properties
      public TypeCollectionStore<object> Instances { get; }
      public Type Type { get; }
      #endregion
      public GenericSingletonRegistration(Type type)
      {
         Type = type;
         Instances = new TypeCollectionStore<object>();
      }

      #region Methods
      public override void Dispose() => Instances.Dispose();
      #endregion
   }
}
