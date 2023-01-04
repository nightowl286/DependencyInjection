using System.Reflection;
using System.Reflection.Emit;

namespace DependencyInjection.Benchmarks;

public class TypeCreator
{
   #region Properties
   public List<Type> Types { get; } = new List<Type>();
   public List<Type> Interfaces { get; } = new List<Type>();
   public int Amount => Types.Count;
   #endregion

   #region Methods

   public void Create(int amount, bool requirePrevious)
   {
      string name = "DynamicBenchmarkClasses";
      AssemblyName assemblyName = new AssemblyName(name);

      AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
      ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name);

      Cleanup();

      ConstructorInfo objCtor = Type.GetType("System.Object")!.GetConstructor(Array.Empty<Type>())!;
      for (int i = 1; i <= amount; i++)
      {
         Type interfaceType = moduleBuilder
            .DefineType($"DynamicBenchmarkInterface{i}", TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract)
            .CreateType()
            ?? throw new Exception("Couldn't create interface.");

         Interfaces.Add(interfaceType);

         TypeBuilder classTypeBuilder = moduleBuilder
            .DefineType($"DynamicBenchmarkClass{i}", TypeAttributes.Class | TypeAttributes.Public, null, new[] { interfaceType });

         if (requirePrevious && i > 1)
         {
            Type previous = Types[i - 2];

            ConstructorBuilder constructorBuilder = classTypeBuilder
               .DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new[] { previous });

            ILGenerator gen = constructorBuilder.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);

            gen.Emit(OpCodes.Call, objCtor);

            gen.Emit(OpCodes.Ret);
         }

         Type classType = classTypeBuilder.CreateType()
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