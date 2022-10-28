using System.Reflection;
using System.Reflection.Emit;

namespace DependencyInjection.Benchmarks
{
   public class TypeCreator
   {
      #region Properties
      public List<Type> Types { get; } = new List<Type>();
      public List<Type> Interfaces { get; } = new List<Type>();
      public int Amount => Types.Count;
      #endregion

      #region Methods

      public void Create(int amount)
      {
         string name = "DynamicBenchmarkClasses";
         AssemblyName assemblyName = new AssemblyName(name);

         AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
         ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name);

         Cleanup();
         for (int i = 1; i <= amount; i++)
         {
            Type interfaceType = moduleBuilder
               .DefineType($"DynamicBenchmarkInterface{i}", TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract)
               .CreateType()
               ?? throw new Exception("Couldn't create interface.");

            Interfaces.Add(interfaceType);

            Type classType = moduleBuilder
               .DefineType($"DynamicBenchmarkClass{i}", TypeAttributes.Class | TypeAttributes.Public, null, new[] { interfaceType })
               .CreateType()
               ?? throw new Exception("Couldn't create class.");

            Types.Add(classType);
         }
      }

      public void Cleanup()
      {
         Types.Clear();
         Interfaces.Clear();
      }
      #endregion
   }
}
