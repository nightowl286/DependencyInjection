using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.Benchmarks.Get;

public class GetAllSingletonBenchmark : GetAllBenchmarkBase
{
   #region Properties
   [Params(1, 5, 10, 25, 50, 100, 250, 500, 1000)]
   public override int Amount { get; set; }
   #endregion

   protected override void Register(ServiceFacade facade, Type classType, Type interfaceType) => facade.Singleton(interfaceType, classType);
}