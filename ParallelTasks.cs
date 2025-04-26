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

    public static int AmountOfThreads() => (Environment.ProcessorCount < 3) ? 1 : Environment.ProcessorCount - 1;

    public static bool CanParallelProcess() => Environment.ProcessorCount > 3;

    public static ParallelOptions ParallelSettingsWithEarlyBreak(CancellationToken token) => new()
    {
        MaxDegreeOfParallelism = AmountOfThreads(),
        CancellationToken = token
    };
}