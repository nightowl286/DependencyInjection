using CommunityToolkit.Diagnostics;
using TNO.Common.Extensions;

namespace TNO.DependencyInjection.Components
{
   internal static class RegistrarUtility
   {
      #region Methods
      public static void CheckTypeImplementation(Type serviceType, Type concreteType, bool mustImplement)
      {
         if (!concreteType.CanCreateInstance(true))
            ThrowHelper.ThrowArgumentException(nameof(concreteType), $"The given concrete type ({concreteType}) cannot be used to create an instance.");

         if ((serviceType.IsGenericTypeDefinition && !concreteType.IsGenericTypeDefinition) && mustImplement)
            ThrowHelper.ThrowArgumentException(nameof(concreteType), $"The given concrete type ({concreteType}) must be a generic definition if the given service type ({serviceType}) is a generic definition.");

         if (!ImplementsType(concreteType, serviceType) && mustImplement)
            ThrowHelper.ThrowArgumentException(nameof(concreteType), $"The given concrete type ({concreteType}) does not implement the service type ({serviceType}).");
      }
      public static bool ImplementsType(Type concrete, Type service) => concrete.IsAssignableTo(service) || concrete.ImplementsOpenInterface(service);
      #endregion
   }
}
