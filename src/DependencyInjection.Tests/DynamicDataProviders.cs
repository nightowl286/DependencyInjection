using System.Diagnostics;
using System.Reflection;
using TNO.DependencyInjection.Abstractions;

namespace TNO.DependencyInjection.Tests
{
   internal class DynamicDataProviders
   {
      #region Dynamic data
      public static IEnumerable<object[]> GetAllRegistrationModes()
      {
         foreach (AppendValueMode mode in Enum.GetValues<AppendValueMode>())
            yield return new object[] { mode };
      }

      public static IEnumerable<object?[]> GetAllRegistrationModesAndNull()
      {
         foreach (object[] mode in GetAllRegistrationModes())
            yield return mode;

         yield return new object?[] { null };
      }

      public static string GetTypeNames(MethodInfo method, object[] values)
      {
         Debug.Assert(values.All(v => v is Type));
         IEnumerable<string> names = values.Cast<Type>().Select(t => t.Name);

         string allNames = string.Join(", ", names);


         return $"{method.Name}({allNames})";
      }

      public static IEnumerable<object[]> GetValidCreatableCombinations()
      {
         yield return Array<Class, Class>();
         yield return Array<IInterfaceForClass, ClassWithInterface>();
      }

      public static IEnumerable<object[]> GetValidCombinations()
      {
         foreach (object[] combination in GetValidCreatableCombinations())
            yield return combination;

         yield return new[] { typeof(GenericClassWithInterface<>), typeof(GenericClassWithInterface<>) };
         yield return new[] { typeof(IGenericInterfaceForClass<>), typeof(GenericClassWithInterface<>) };
      }

      public static IEnumerable<object[]> GetInvalidCreateableCombinations()
      {
         yield return Array<Struct, Class>();
         yield return new[] { typeof(GenericStruct<>), typeof(Class) };
         yield return Array<IInterface, Class>();
         yield return new[] { typeof(IGenericInterface<>), typeof(Class) };
         yield return Array<IInterface, GenericClass<object>>();
      }
      public static IEnumerable<object[]> GetInvalidTypeCombinations()
      {
         foreach (object[] combination in GetInvalidCreateableCombinations())
            yield return combination;

         yield return new[] { typeof(IGenericInterface<>), typeof(IGenericInterface<>) };
         yield return new[] { typeof(IGenericInterface<>), typeof(IGenericInterface<object>) };
         yield return new[] { typeof(GenericStruct<>), typeof(Struct) };
         yield return Array<Class, IInterface>();
         yield return Array<Struct, IInterface>();
      }

      public static IEnumerable<object[]> GetNonBuildableTypes()
      {
         yield return Array<ClassWithOutParameterType>();
         yield return Array<ClassWithStructParameterType>();
         yield return Array<ClassWithNoPublicConstructor>();
         yield return Array<ClassWithValidParameterType>(); // parameter type missing
      }

      public static IEnumerable<object[]> GetBuildableTypes()
      {
         yield return Array<ClassWithEnumerableParameterType>();
         yield return Array<ClassWithValidNullableParameterType>();
         yield return Array<ClassWithValidParameterType>();
      }

      private static object[] Array<T>() => new[] { typeof(T) };
      private static object[] Array<T1, T2>() => new[] { typeof(T1), typeof(T2) };
      #endregion
   }
}
