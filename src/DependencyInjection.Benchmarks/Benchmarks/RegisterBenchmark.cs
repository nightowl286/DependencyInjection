using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection.Abstractions;
using TNO.DependencyInjection.Components;

namespace DependencyInjection.Benchmarks.Benchmarks;

public class RegisterBenchmark : BaseTypeCreatorBenchmark
{
   #region Properties
   [Params(1, 25, 50, 100)]
   public override int Amount { get; set; }
   #endregion

   #region Benchmarks
   [Benchmark]
   public void Singleton()
   {
      ServiceScope scope = new ServiceScope(null, AppendValueMode.ReplaceAll);

      foreach (Type type in Types)
      {
         scope.Registrar.Singleton(type, type);
      }
   }

   [Benchmark]
   public void SingletonWithInterface()
   {
      ServiceScope scope = new ServiceScope(null, AppendValueMode.ReplaceAll);

      for (int i = 0; i < Amount; i++)
      {
         Type classType = Types[i];
         Type interfaceType = Interfaces[i];

         scope.Registrar.Singleton(interfaceType, classType);
      }
   }
   #endregion
}