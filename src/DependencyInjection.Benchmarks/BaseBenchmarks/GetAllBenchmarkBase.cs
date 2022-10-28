using BenchmarkDotNet.Attributes;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TNO.DependencyInjection;
using TNO.DependencyInjection.Abstractions;

namespace DependencyInjection.Benchmarks.BaseBenchmarks
{
   public abstract class GetAllBenchmarkBase
   {
      #region Fields
      private readonly ServiceFacade _serviceFacade = new ServiceFacade(RegistrationMode.Append); 
      private Type? _interfaceType;
      #endregion

      #region Properties
      public abstract int Amount { get; set; }
      #endregion

      #region Methods
      [GlobalSetup]
      public void Setup()
      {
         string name = "DynamicBenchmarkClasses";
         AssemblyName assemblyName = new AssemblyName(name);

         AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
         ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name);

         _interfaceType = moduleBuilder
               .DefineType("DynamicBenchmarkInterface", TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract)
               .CreateType()
               ?? throw new Exception("Couldn't create interface.");

         for (int i = 1; i <= Amount; i++)
         {

            Type classType = moduleBuilder
               .DefineType($"DynamicBenchmarkClass{i}", TypeAttributes.Class | TypeAttributes.Public, null, new[] { _interfaceType })
               .CreateType()
               ?? throw new Exception("Couldn't create class.");

            Register(_serviceFacade, classType, _interfaceType);
         }
      }

      [GlobalCleanup]
      public void Cleanup()
      {
         _interfaceType = null;
      }

      protected abstract void Register(ServiceFacade facade, Type classType, Type interfaceType);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private IEnumerable<object> GetEnumerable()
      {
         Debug.Assert(_interfaceType is not null);

         IEnumerable<object> enumerable = _serviceFacade.GetAll(_interfaceType);

         return enumerable;
      }

      [Benchmark]
      public object Array() => GetEnumerable().ToArray();

      [Benchmark]
      public object List() => new List<object>(GetEnumerable());
      #endregion
   }
}