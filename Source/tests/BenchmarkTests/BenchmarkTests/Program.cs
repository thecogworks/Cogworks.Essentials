using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchmarkTests
{
    [MemoryDiagnoser]
    internal class Program
    {
        /// <summary>
        /// CLI Run: dotnet run -c Release -f netcoreapp3.1 --runtimes net48 net50 netcoreapp31 --filter * --join --warmupCount 1 --minIterationCount 9 --maxIterationCount 10 --iterationCount 5 --strategy ColdStart
        /// </summary>
        /// <param name="args">--runtimes net48 net50 netcoreapp31 --filter * --join --warmupCount 1 --minIterationCount 9 --maxIterationCount 10 --iterationCount 5 --strategy ColdStart</param>
        internal static void Main(string[] args)
            => BenchmarkSwitcher
                .FromAssemblies(new[] { typeof(Program).Assembly })
                .Run(args);
    }
}
