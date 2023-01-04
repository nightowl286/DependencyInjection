using System;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.Abstractions;

/// <summary>
/// Denotes that the implementing type bundles up all the components of a dependency injection system.
/// </summary>
public interface IServiceFacade : IServiceRegistrar, IServiceRequester, IServiceBuilder, IDisposable { }