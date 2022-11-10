using System.Diagnostics.CodeAnalysis;

namespace TNO.DependencyInjection.Tests
{
   public class Class { }
   public class GenericClass<T> { }

   public interface IInterface { }
   public interface IGenericInterface<T> { }

   public struct Struct { }
   public struct GenericStruct<T> { }

   public class ClassWithInterface : IInterfaceForClass { }
   public class GenericClassWithInterface<T> : IGenericInterfaceForClass<T> { }
   public class AnotherClassWithInterface : IInterfaceForClass { }

   public interface IInterfaceForClass { }
   public interface IGenericInterfaceForClass<T> { }

   #region Building
   public class ClassWithOutParameterType
   {
      public ClassWithOutParameterType(out Class value) => value = new Class();
   }
   public class ClassWithStructParameterType
   {
      public ClassWithStructParameterType(Struct _) { }
   }
   public class ClassWithValidParameterType
   {
      public ClassWithValidParameterType(Class _) { }
   }
   public class ClassWithNoPublicConstructor
   {
      private ClassWithNoPublicConstructor() { }
   }
   public class ClassWithValidNullableParameterType
   {
      public ClassWithValidNullableParameterType([AllowNull] Class _) { }
   }
   public class ClassWithEnumerableParameterType
   {
      public ClassWithEnumerableParameterType(Class[] _) { }
   }
   #endregion
}
