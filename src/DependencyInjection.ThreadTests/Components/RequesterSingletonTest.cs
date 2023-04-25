using System.Diagnostics;
using TNO.DependencyInjection.Abstractions.Components;

namespace TNO.DependencyInjection.ThreadTests.Components;

[TestClass]
public class RequesterSingletonTest : ThreadSafetyTestBase
{
   #region Fields
   private IServiceRequester? _requester;
   #endregion

   #region Methods
   protected override void SetupRun()
   {
      IServiceScope scope = new ServiceFacade().CreateNew();
      scope.Registrar.Singleton<ITestService, TestService>();

      _requester = scope.Requester;
   }
   protected override void ThreadCode()
   {
      Debug.Assert(_requester is not null, "Requester was not set.");

      _requester.Get<ITestService>();
   }
   protected override void AssertRun() => Assert.That.AreEqual(1, TestService.Instances);
   protected override void RunCleanup()
   {
      TestService.Instances = 0;
   }
   #endregion

   #region Subclasses
   private interface ITestService { }
   private class TestService : ITestService
   {
      #region Properties
      public static int Instances { get; set; }
      #endregion
      public TestService() => Instances++;
   }
   #endregion
}
