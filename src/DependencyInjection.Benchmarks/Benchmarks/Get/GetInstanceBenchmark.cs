using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection.Abstractions.Components;

namespace DependencyInjection.Benchmarks.Benchmarks.Get;

public class GetInstanceBenchmark : BaseGetBenchmark
{
   #region Properties
   [Params(1, 10, 25, 50, 100, 250, 500, 1000)]
   public override int Amount { get; set; }
   #endregion

   public override void Register(IServiceScope scope, Type classType, Type interfaceType)
   {
      object instance = Activator.CreateInstance(classType) ?? throw new Exception($"Couldn't create instance of type {classType}.");

      scope.Registrar.Instance(classType, instance);
      scope.Registrar.Instance(interfaceType, instance);
   }
}