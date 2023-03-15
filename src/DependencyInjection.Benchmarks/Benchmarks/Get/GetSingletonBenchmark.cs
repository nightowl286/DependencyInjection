using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection.Abstractions.Components;

namespace DependencyInjection.Benchmarks.Benchmarks.Get;

public class GetSingletonBenchmark : BaseGetBenchmark
{
   #region Properties
   [Params(1, 10, 25, 50, 100, 250, 500, 1000)]
   public override int Amount { get; set; }
   #endregion

   public override void Register(IServiceScope serviceScope, Type classType, Type interfaceType)
   {
      serviceScope.Registrar.Singleton(classType, classType);
      serviceScope.Registrar.Singleton(interfaceType, classType);
   }
}