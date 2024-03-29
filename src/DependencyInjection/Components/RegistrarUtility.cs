﻿using TNO.Common.Extensions;

namespace TNO.DependencyInjection.Components;

internal static class RegistrarUtility
{
   #region Methods
   public static void CheckTypeImplementation(Type serviceType, Type concreteType, bool mustImplement)
   {
      if (!concreteType.CanCreateInstance(true))
         throw new ArgumentException($"The given concrete type ({concreteType}) cannot be used to create an instance.", nameof(concreteType));

      if (serviceType.IsGenericTypeDefinition && !concreteType.IsGenericTypeDefinition && mustImplement)
         throw new ArgumentException($"The given concrete type ({concreteType}) must be a generic definition if the given service type ({serviceType}) is a generic definition.", nameof(concreteType));

      if (!ImplementsType(concreteType, serviceType) && mustImplement)
         throw new ArgumentException($"The given concrete type ({concreteType}) does not implement the service type ({serviceType}).", nameof(concreteType));
   }
   public static bool ImplementsType(Type concrete, Type service) => service.IsAssignableFrom(concrete) || concrete.ImplementsOpenInterface(service);
   #endregion
}