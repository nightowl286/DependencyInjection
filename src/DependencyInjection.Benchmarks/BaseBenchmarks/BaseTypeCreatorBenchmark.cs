using BenchmarkDotNet.Attributes;

namespace DependencyInjection.Benchmarks.BaseBenchmarks
{
   public abstract class BaseTypeCreatorBenchmark
   {
      #region Fields
      private readonly TypeCreator _creator = new TypeCreator();
      #endregion

      #region Properties
      public virtual bool ClassRequiredPrevious { get; }
      public abstract int Amount { get; set; }
      public List<Type> Types => _creator.Types;
      public List<Type> Interfaces => _creator.Interfaces;
      #endregion

      #region Methods
      [GlobalSetup]
      public virtual void Setup() => _creator.Create(Amount, ClassRequiredPrevious);

      [GlobalCleanup]
      public virtual void Cleanup() => _creator.Cleanup();
      #endregion
   }
}
