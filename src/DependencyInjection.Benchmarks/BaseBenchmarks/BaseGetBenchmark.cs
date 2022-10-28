using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.BaseBenchmarks
{
   public abstract class BaseGetBenchmark : BaseTypeCreatorBenchmark
   {
      #region Fields
      private readonly ServiceFacade _serviceFacade = new ServiceFacade();
      private Type? _interfaceToRequest = null;
      private Type? _typeToRequest = null;
      #endregion

      #region Properties
      public ServiceFacade Facade => _serviceFacade;
      public Type InterfaceToRequest => _interfaceToRequest ?? throw new NullReferenceException();
      public Type TypeToRequest => _typeToRequest ?? throw new NullReferenceException();
      #endregion

      public override void Setup()
      {
         base.Setup();

         _interfaceToRequest = Interfaces[0];
         _typeToRequest = Types[0];

         for (int i = 0; i < Amount; i++)
         {
            Type classType = Types[i];
            Type interfaceType = Interfaces[i];

            Register(_serviceFacade, classType, interfaceType);
         }
      }

      public abstract void Register(ServiceFacade facade, Type classType, Type interfaceType);

      #region Benchmarks

      [Benchmark]
      public virtual object ByInterface() => Facade.Get(InterfaceToRequest);

      [Benchmark]
      public virtual object ByType() => Facade.Get(TypeToRequest);
      #endregion
   }
}