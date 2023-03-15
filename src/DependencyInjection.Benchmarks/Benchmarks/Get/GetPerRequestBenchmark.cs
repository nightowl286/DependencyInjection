using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection.Abstractions.Components;

namespace DependencyInjection.Benchmarks.Benchmarks.Get;

[Config(typeof(AntiVirusFriendlyConfig))]
public class GetPerRequestBenchmark : BaseGetBenchmark
{
   #region Properties
   [Params(1, 10, 15, 25, 50, 75, 100, 150)]
   public override int Amount { get; set; }
   public override bool ClassRequiredPrevious => true;
   #endregion

   public override void Register(IServiceScope scope, Type classType, Type interfaceType)
   {
      scope.Registrar.PerRequest(classType, classType);
      scope.Registrar.PerRequest(interfaceType, classType);
   }
}