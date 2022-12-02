using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.Benchmarks.Get
{
   [Config(typeof(AntiVirusFriendlyConfig))]
   public class GetPerRequestBenchmark : BaseGetBenchmark
   {
      #region Properties
      [Params(1, 10, 15, 25, 50, 75, 100, 150)]
      public override int Amount { get; set; }
      public override bool ClassRequiredPrevious => true;
      #endregion

      public override void Register(ServiceFacade facade, Type classType, Type interfaceType)
      {
         facade.PerRequest(classType, classType);
         facade.PerRequest(interfaceType, classType);
      }
   }
}
