using BenchmarkDotNet.Running;
using DependencyInjection.Benchmarks.Benchmarks.Get;

BenchmarkRunner.Run<GetAllSingletonBenchmark>();

Console.ReadLine();