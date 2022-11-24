using BenchmarkDotNet.Running;
using DependencyInjection.Benchmarks.Benchmarks;

BenchmarkRunner.Run<BuildBenchmark>();

Console.ReadLine();