using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.Benchmarks.Get
{
   public class GetInstanceBenchmark : BaseGetBenchmark
   {
      #region Properties
      [Params(1, 10, 25, 50, 100, 250, 500, 1000)]
      public override int Amount { get; set; }
      #endregion

      public override void Register(ServiceFacade facade, Type classType, Type interfaceType)
      {
         object instance = Activator.CreateInstance(classType) ?? throw new Exception($"Couldn't create instance of type {classType}.");

         facade.Instance(classType, instance);
         facade.Instance(interfaceType, instance);
      }
   }
}
