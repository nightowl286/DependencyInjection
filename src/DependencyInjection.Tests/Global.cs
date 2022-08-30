global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using TNO.Tests.Common.Extensions;

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
}