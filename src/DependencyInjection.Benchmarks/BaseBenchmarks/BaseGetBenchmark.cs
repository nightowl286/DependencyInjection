using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using TNO.Common.Locking;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Abstractions.Components;
using TNO.DependencyInjection.Components;

namespace DependencyInjection.Benchmarks.BaseBenchmarks;

public abstract class BaseGetBenchmark : BaseTypeCreatorBenchmark
{
   #region Fields
   private readonly ServiceScope _serviceScope = new ServiceScope(null, AppendValueMode.ReplaceAll);
   private Type? _interfaceToRequest = null;
   private Type? _typeToRequest = null;
   private ReferenceKey? _key;
   #endregion

   #region Properties
   public IServiceScope Scope => _serviceScope;
   public Type InterfaceToRequest => _interfaceToRequest ?? throw new NullReferenceException();
   public Type TypeToRequest => _typeToRequest ?? throw new NullReferenceException();
   #endregion

   public override void Setup()
   {
      base.Setup();

      _interfaceToRequest = Interfaces.Last();
      _typeToRequest = Types.Last();

      for (int i = 0; i < Amount; i++)
      {
         Type classType = Types[i];
         Type interfaceType = Interfaces[i];

         Register(_serviceScope, classType, interfaceType);
      }
   }

   public abstract void Register(IServiceScope serviceScope, Type classType, Type interfaceType);

   [GlobalSetup(Targets = new[] { nameof(ByInterfaceOptimised), nameof(ByTypeOptimised) })]
   public void OptimisedSetup()
   {
      Setup();
      _serviceScope.Registrar.TryLock(out _key);
   }

   [GlobalCleanup(Targets = new[] { nameof(ByInterfaceOptimised), nameof(ByTypeOptimised) })]
   public void OptimisedCleanup()
   {
      Debug.Assert(_key is not null);
      _serviceScope.Registrar.TryUnlock(_key);
   }

   #region Benchmarks

   [Benchmark]
   public virtual object ByInterface() => _serviceScope.Requester.Get(InterfaceToRequest);

   [Benchmark]
   public virtual object ByType() => _serviceScope.Requester.Get(TypeToRequest);

   [Benchmark]
   public virtual object ByInterfaceOptimised() => _serviceScope.Requester.Get(InterfaceToRequest);

   [Benchmark]
   public virtual object ByTypeOptimised() => _serviceScope.Requester.Get(TypeToRequest);
   #endregion
}