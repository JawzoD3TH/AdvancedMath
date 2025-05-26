using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using static AdvancedMath.AdvancedMath;
using static AdvancedMath.ParallelTasks;

namespace AdvancedMath;

public static class Generic
{
    public static T CoefficientOfVariation<T>(IEnumerable<T> listOfNumbers) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers:
                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in decimalListOfNumbers));

            case double[] doubleListOfNumbers:
                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in doubleListOfNumbers));

            default:
                var count = listOfNumbers.GetLengthOrCount();
                var newDoubleListOfNumbers = new double[count];
                if (CanParallelProcess() && IsLargeArray(count))
                    Parallel.For(0, count, ParallelSettings, (i, _) => newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i)));
                else for (var i = 0; i < count; i++)
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));

                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in newDoubleListOfNumbers));
        }
    }

    public static T CoefficientOfVariation<T>(T mean, T standardDeviation, T value) where T : INumber<T> =>
        standardDeviation == T.Zero ? T.Zero : (value - mean) / standardDeviation;

    public static async Task<T> CoefficientOfVariationAsync<T>(IEnumerable<T> listOfNumbers) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers:
                return T.CreateChecked(await AdvancedMath.CoefficientOfVariationAsync(decimalListOfNumbers).ConfigureAwait(false));

            case double[] doubleListOfNumbers:
                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in doubleListOfNumbers));

            default:
                var count = listOfNumbers.GetLengthOrCount();
                var newDoubleListOfNumbers = new double[count];

                if (CanParallelProcess() && IsLargeArray(count))
                {
                    await Parallel.ForAsync(0, count, ParallelSettings, async (i, _) =>
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }).ConfigureAwait(false);
                }
                else await Task.Run(async () =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }
                }).ConfigureAwait(false);

                return T.CreateChecked(AdvancedMath.CoefficientOfVariation(in newDoubleListOfNumbers));
        }
    }

    public static bool NullOrLessThanOrEqualToZero<T>(T? value) where T : INumber<T> => value is null || value <= T.Zero;

    public static bool ManyNullOrLessThanOrEqualToZero<T>(params T[]? values) where T : INumber<T>
    {
        if (values is null)
            return true;

        if (values.LessThan())
            return true;

        bool result = false;
        if (CanParallelProcess() && IsLargeArray(values.Length))
        {
            Parallel.For(0, values.Length, ParallelSettings, (i, state) =>
            {
                if (NullOrLessThanOrEqualToZero(values[i]))
                {
                    result = true;
                    state.Break(); // Cancel the operation if the condition is met
                }
            });
        }
        else foreach (var value in values)
            {
                if (NullOrLessThanOrEqualToZero(value))
                    return true;
            }

        return result;
    }

    public static T Power<T>(T value, int power) where T : INumber<T> =>
        value switch
        {
            decimal decimalValue => T.CreateChecked(AdvancedMath.Power(in decimalValue, power)),
            double doubleValue => T.CreateChecked(Math.Pow(doubleValue, power)),
            _ => T.CreateChecked(Math.Pow(double.CreateTruncating(value), power))
        };

    public static async Task<T> PowerAsync<T>(T value, int power) where T : INumber<T> =>
        value switch
        {
            decimal decimalValue => T.CreateChecked(await AdvancedMath.PowerAsync(decimalValue, power).ConfigureAwait(false)),
            double doubleValue => T.CreateChecked(Math.Pow(doubleValue, power)),
            _ => T.CreateChecked(Math.Pow(double.CreateTruncating(value), power))
        };

    public static T SquareRoot<T>(T value) where T : INumber<T> =>
        value switch
        {
            decimal decimalValue => T.CreateChecked(AdvancedMath.SquareRoot(in decimalValue)),
            double doubleValue => T.CreateChecked(Math.Sqrt(doubleValue)),
            _ => T.CreateChecked(Math.Sqrt(double.CreateTruncating(value)))
        };

    public static async Task<T> SquareRootAsync<T>(T value) where T : INumber<T> =>
        value switch
        {
            decimal decimalValue => T.CreateChecked(await AdvancedMath.SquareRootAsync(decimalValue).ConfigureAwait(false)),
            double doubleValue => T.CreateChecked(Math.Sqrt(doubleValue)),
            _ => T.CreateChecked(Math.Sqrt(double.CreateTruncating(value)))
        };

    public static T StandardDeviation<T>(IEnumerable<T> listOfNumbers) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers:
                return T.CreateChecked(AdvancedMath.StandardDeviation(in decimalListOfNumbers));

            case double[] doubleListOfNumbers:
                return T.CreateChecked(AdvancedMath.StandardDeviation(in doubleListOfNumbers));

            default:
                var count = listOfNumbers.GetLengthOrCount();
                var newDoubleListOfNumbers = new double[count];
                if (CanParallelProcess() && IsLargeArray(count))
                    Parallel.For(0, count, ParallelSettings, (i, _) => newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i)));
                else for (var i = 0; i < count; i++)
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));

                return T.CreateChecked(AdvancedMath.StandardDeviation(in newDoubleListOfNumbers));
        }
    }

    public static async Task<T> StandardDeviationAsync<T>(IEnumerable<T> listOfNumbers) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers:
                return T.CreateChecked(await AdvancedMath.StandardDeviationAsync(decimalListOfNumbers).ConfigureAwait(false));

            case double[] doubleListOfNumbers:
                return T.CreateChecked(AdvancedMath.StandardDeviation(in doubleListOfNumbers));

            default:
                var count = listOfNumbers.GetLengthOrCount();
                var newDoubleListOfNumbers = new double[count];

                if (CanParallelProcess() && IsLargeArray(count))
                {
                    await Parallel.ForAsync(0, count, ParallelSettings, async (i, _) =>
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }).ConfigureAwait(false);
                }
                else await Task.Run(async () =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }
                }).ConfigureAwait(false);
                return T.CreateChecked(AdvancedMath.StandardDeviation(in newDoubleListOfNumbers));
        }
    }

    public static T ZScore<T>(IEnumerable<T> listOfNumbers, T fallbackZScoreValue) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers when fallbackZScoreValue is decimal decimalZScore:
                return T.CreateChecked(AdvancedMath.ZScore(decimalListOfNumbers, in decimalZScore));

            case double[] doubleListOfNumbers when fallbackZScoreValue is double doubleZScore:
                return T.CreateChecked(AdvancedMath.ZScore(doubleListOfNumbers, in doubleZScore));

            default:
                var count = listOfNumbers.GetLengthOrCount();
                var newDoubleListOfNumbers = new double[count];
                if (CanParallelProcess() && IsLargeArray(count))
                    Parallel.For(0, count, ParallelSettings, (i, _) => newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i)));
                else for (var i = 0; i < count; i++)
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));

                return T.CreateChecked(AdvancedMath.ZScore(newDoubleListOfNumbers, double.CreateTruncating(fallbackZScoreValue)));
        }
    }

    public static async Task<T> ZScoreAsync<T>(IEnumerable<T> listOfNumbers, T fallbackZScoreValue) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers when fallbackZScoreValue is decimal decimalZScore:
                return T.CreateChecked(await AdvancedMath.ZScoreAsync(decimalListOfNumbers, decimalZScore).ConfigureAwait(false));

            case double[] doubleListOfNumbers when fallbackZScoreValue is double doubleZScore:
                return T.CreateChecked(AdvancedMath.ZScore(doubleListOfNumbers, in doubleZScore));

            default:
                var count = listOfNumbers.GetLengthOrCount();
                var newDoubleListOfNumbers = new double[count];

                if (CanParallelProcess() && IsLargeArray(count))
                {
                    await Parallel.ForAsync(0, count, ParallelSettings, async (i, _) =>
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }).ConfigureAwait(false);
                }
                else await Task.Run(async () =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }
                }).ConfigureAwait(false);

                return T.CreateChecked(AdvancedMath.ZScore(newDoubleListOfNumbers, double.CreateTruncating(fallbackZScoreValue)));
        }
    }
}