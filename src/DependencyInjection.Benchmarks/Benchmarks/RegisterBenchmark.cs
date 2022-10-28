using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.Benchmarks
{
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
         ServiceFacade facade = new ServiceFacade();

         foreach (Type type in Types)
         {
            facade.Singleton(type, type);
         }
      }

      [Benchmark]
      public void SingletonWithInterface()
      {
         ServiceFacade facade = new ServiceFacade();

         for (int i = 0; i < Amount; i++)
         {
            Type classType = Types[i];
            Type interfaceType = Interfaces[i];

            facade.Singleton(interfaceType, classType);
         }
      }
      #endregion
   }
}
