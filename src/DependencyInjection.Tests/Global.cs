global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using TNO.Tests.Common;
global using TNO.Tests.Moq;

#if DEBUG
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel, Workers = 1)]
#else
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 0)]
#endif

internal static class Category
{
   public const string Dependency_Injection = "Dependency Injection";
   public const string Dependency_Injection_Component = "DI Component";
   public const string Extensions = "Extensions";

   public const string Builder = "Builder";
   public const string Requester = "Requester";
   public const string Registrar = "Registrar";
}