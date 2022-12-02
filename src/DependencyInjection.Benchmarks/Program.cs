using BenchmarkDotNet.Running;
using DependencyInjection.Benchmarks.Benchmarks.Get;

BenchmarkRunner.Run<GetPerRequestBenchmark>();

Console.ReadLine();