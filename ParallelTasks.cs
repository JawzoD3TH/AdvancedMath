using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedMath;

public static class ParallelTasks
{
    public static readonly ParallelOptions ParallelSettings = new()
    {
        MaxDegreeOfParallelism = AmountOfThreads()
    };

    public static bool CanParallelProcess() => Environment.ProcessorCount > 3;

    public static ParallelOptions ParallelSettingsWithEarlyBreak(CancellationTokenSource cancellationTokenSource) => new()
    {
        MaxDegreeOfParallelism = AmountOfThreads(),
        CancellationToken = cancellationTokenSource.Token
    };

    private static int AmountOfThreads() => (Environment.ProcessorCount < 3) ? 1 : Environment.ProcessorCount - 1;
}