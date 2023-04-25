using System.Collections.Concurrent;

namespace TNO.DependencyInjection.ThreadTests;

[TestClass]
[DoNotParallelize]
[TestCategory(Category.ThreadSafety)]
public abstract class ThreadSafetyTestBase
{
   #region Fields
   private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
   private readonly ConcurrentBag<Exception> _caughtExceptions = new ConcurrentBag<Exception>();
   #endregion

   #region Properties
   protected virtual uint ThreadCount { get; } = 16;
   protected virtual uint Iterations { get; } = 100;
   protected virtual uint Runs { get; } = 100;
   #endregion

   #region Methods
   protected virtual void RunCleanup() { }
   protected abstract void SetupRun();
   protected abstract void ThreadCode();
   protected abstract void AssertRun();

   [TestMethod]
   [DoNotParallelize]
   public virtual void RunTest()
   {
      for (int i = 0; i < Runs; i++)
      {
         _resetEvent.Reset();
         RunCleanup();
         SingleRun();
      }
   }
   protected void SingleRun()
   {
      _caughtExceptions.Clear();
      SetupRun();

      Thread[] threads = CreateThreads();
      _resetEvent.Set();
      WaitForThreads(threads);

      AssertNoExceptions();
      AssertRun();
   }
   #endregion

   #region Helpers
   private void AssertNoExceptions()
   {
      Exception ex;
      if (_caughtExceptions.Count == 1)
         ex = _caughtExceptions.First();
      else if (_caughtExceptions.Count > 1)
         ex = new AggregateException(_caughtExceptions);
      else
         return;

      throw ex;
   }
   private void ThreadLoop()
   {
      _resetEvent.Wait();

      try
      {
         for (int i = 0; i < Iterations; i++)
            ThreadCode();
      }
      catch (Exception ex)
      {
         _caughtExceptions.Add(ex);
      }
   }
   protected static void WaitForThreads(Thread[] threads)
   {
      foreach (Thread thread in threads)
         thread.Join();
   }
   private Thread[] CreateThreads()
   {
      Thread[] threads = new Thread[ThreadCount];
      for (int i = 0; i < ThreadCount; i++)
      {
         Thread thread = new Thread(ThreadLoop);
         thread.Start();
         threads[i] = thread;
      }

      return threads;
   }
   #endregion
}

