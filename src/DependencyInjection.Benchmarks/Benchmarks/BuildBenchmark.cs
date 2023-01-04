using BenchmarkDotNet.Attributes;
using DependencyInjection.Benchmarks.BaseBenchmarks;
using System.Diagnostics;
using TNO.DependencyInjection;

namespace DependencyInjection.Benchmarks.Benchmarks;

public class BuildBenchmark : BaseTypeCreatorBenchmark
{
   #region Fields
   private readonly ServiceFacade _serviceFacade = new ServiceFacade();
   private Type? _typeToBuild = null;
   private Func<object>? _buildDelegate = null;
   #endregion

   #region Properties
   [Params(1, 5, 10)]
   public override int Amount { get; set; }
   public override bool ClassRequiredPrevious => true;
   #endregion

   public override void Setup()
   {
      base.Setup();


      for (int i = 0; i < Amount; i++)
      {
         Type classType = Types[i];

         _serviceFacade.PerRequest(classType, classType);
      }

      _typeToBuild = Types.Last();
      _buildDelegate = _serviceFacade.BuildDelegate(_typeToBuild);
   }

   #region Benchmarks
   [Benchmark(Baseline = true)]
   public object Build()
   {
      Debug.Assert(_typeToBuild is not null);
      return _serviceFacade.Build(_typeToBuild);
   }

   [Benchmark]
   public Func<object> BuildDelegate()
   {
      Debug.Assert(_typeToBuild is not null);

      return _serviceFacade.BuildDelegate(_typeToBuild);
   }

   [Benchmark]
   public object Delegate()
   {
      Debug.Assert(_typeToBuild is not null);

      return _serviceFacade.BuildDelegate(_typeToBuild).Invoke();
   }

   [Benchmark]
   public object DelegateCached()
   {
      Debug.Assert(_buildDelegate is not null);

      return _buildDelegate.Invoke();
   }
   #endregion
}