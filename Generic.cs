using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static AdvancedMath.AdvancedMath;
using static AdvancedMath.ParallelTasks;

namespace AdvancedMath;

public static class Generic
{
    public static T CoefficientOfVariation<T>(IEnumerable<T> listOfNumbers) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers:
                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in decimalListOfNumbers));
            case double[] doubleListOfNumbers:
                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in doubleListOfNumbers));
            default:
            {
                var converted = ToDoubleArray(listOfNumbers);
                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in converted));
            }
        }
    }

    public static T CoefficientOfVariation<T>(T mean, T standardDeviation) where T : INumber<T>
    {
        if (mean == T.Zero)
        {
            if (standardDeviation == T.Zero)
                return T.Zero;

            throw new DivideByZeroException(
                "The coefficient of variation is undefined when the mean is zero.");
        }

        return standardDeviation / mean;
    }

    // Kept for source compatibility. The value is not part of the coefficient of
    // variation definition; callers that need a per-value standardized score should
    // use ZScore(mean, standardDeviation, value).
    public static T CoefficientOfVariation<T>(T mean, T standardDeviation, T value) where T : INumber<T>
    {
        _ = value;
        return CoefficientOfVariation(mean, standardDeviation);
    }

    public static T ZScore<T>(T mean, T standardDeviation, T value) where T : INumber<T> =>
        standardDeviation == T.Zero ? T.Zero : (value - mean) / standardDeviation;

    public static Task<T> CoefficientOfVariationAsync<T>(IEnumerable<T> listOfNumbers) where T : INumber<T> =>
        Task.FromResult(CoefficientOfVariation(listOfNumbers));

    public static bool NullOrLessThanOrEqualToZero<T>(T? value) where T : INumber<T> =>
        value is null || value <= T.Zero;

    public static bool ManyNullOrLessThanOrEqualToZero<T>(params T[]? values) where T : INumber<T>
    {
        if (values is null || values.Length == 0)
            return true;

        if (CanParallelProcess() && IsLargeArray(values.Length))
        {
            var found = 0;
            Parallel.For(0, values.Length, ParallelSettings, (i, state) =>
            {
                if (Volatile.Read(ref found) != 0)
                {
                    state.Stop();
                    return;
                }

                if (NullOrLessThanOrEqualToZero(values[i]))
                {
                    Interlocked.Exchange(ref found, 1);
                    state.Stop();
                }
            });

            return Volatile.Read(ref found) != 0;
        }

        foreach (var value in values)
        {
            if (NullOrLessThanOrEqualToZero(value))
                return true;
        }

        return false;
    }

    public static T Power<T>(T value, int power) where T : INumber<T> =>
        value switch
        {
            decimal decimalValue => T.CreateChecked(AdvancedMath.Power(in decimalValue, power)),
            double doubleValue => T.CreateChecked(Math.Pow(doubleValue, power)),
            _ => T.CreateChecked(Math.Pow(double.CreateTruncating(value), power))
        };

    public static Task<T> PowerAsync<T>(T value, int power) where T : INumber<T> =>
        Task.FromResult(Power(value, power));

    public static T SquareRoot<T>(T value) where T : INumber<T> =>
        value switch
        {
            decimal decimalValue => T.CreateChecked(AdvancedMath.SquareRoot(in decimalValue)),
            double doubleValue => T.CreateChecked(Math.Sqrt(doubleValue)),
            _ => T.CreateChecked(Math.Sqrt(double.CreateTruncating(value)))
        };

    public static Task<T> SquareRootAsync<T>(T value) where T : INumber<T> =>
        Task.FromResult(SquareRoot(value));

    public static T StandardDeviation<T>(IEnumerable<T> listOfNumbers) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers:
                return T.CreateChecked(AdvancedMath.StandardDeviation(in decimalListOfNumbers));
            case double[] doubleListOfNumbers:
                return T.CreateChecked(AdvancedMath.StandardDeviation(in doubleListOfNumbers));
            default:
            {
                var converted = ToDoubleArray(listOfNumbers);
                return T.CreateChecked(AdvancedMath.StandardDeviation(in converted));
            }
        }
    }

    public static Task<T> StandardDeviationAsync<T>(IEnumerable<T> listOfNumbers) where T : INumber<T> =>
        Task.FromResult(StandardDeviation(listOfNumbers));

    public static T ZScore<T>(IEnumerable<T> listOfNumbers, T fallbackZScoreValue) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers when fallbackZScoreValue is decimal decimalFallback:
                return T.CreateChecked(AdvancedMath.ZScore(decimalListOfNumbers, in decimalFallback));
            case double[] doubleListOfNumbers when fallbackZScoreValue is double doubleFallback:
                return T.CreateChecked(AdvancedMath.ZScore(doubleListOfNumbers, in doubleFallback));
            default:
            {
                var converted = ToDoubleArray(listOfNumbers);
                return T.CreateChecked(AdvancedMath.ZScore(
                    converted, double.CreateTruncating(fallbackZScoreValue)));
            }
        }
    }

    public static Task<T> ZScoreAsync<T>(IEnumerable<T> listOfNumbers, T fallbackZScoreValue) where T : INumber<T> =>
        Task.FromResult(ZScore(listOfNumbers, fallbackZScoreValue));

    private static double[] ToDoubleArray<T>(IEnumerable<T> source) where T : INumber<T>
    {
        // Snapshot once. Repeated ElementAt calls on a non-indexed enumerable turn
        // conversion into O(n^2) and can enumerate a stateful source concurrently.
        var snapshot = source.ToArray();
        var result = new double[snapshot.Length];

        if (CanParallelProcess() && IsLargeArray(snapshot.Length))
        {
            Parallel.For(0, snapshot.Length, ParallelSettings,
                i => result[i] = double.CreateTruncating(snapshot[i]));
        }
        else
        {
            for (var i = 0; i < snapshot.Length; i++)
                result[i] = double.CreateTruncating(snapshot[i]);
        }

        return result;
    }
}
