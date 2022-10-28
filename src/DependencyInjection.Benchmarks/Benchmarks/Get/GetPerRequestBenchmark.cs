using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.Benchmarks.Get
{
   public class GetPreRequestBenchmark : BaseGetBenchmark
   {
      #region Properties
      [Params(1, 10, 25, 50, 100, 250, 500, 1000)]
      public override int Amount { get; set; }
      #endregion

      public override void Register(ServiceFacade facade, Type classType, Type interfaceType)
      {
         facade.PerRequest(classType, classType);
         facade.PerRequest(interfaceType, classType);
      }
   }
}
