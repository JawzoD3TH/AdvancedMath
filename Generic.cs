﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

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
                var count = listOfNumbers.GetLength();
                var newDoubleListOfNumbers = new double[count];
                for (var i = 0; i < count; i++)
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
                var count = listOfNumbers.GetLength();
                var newDoubleListOfNumbers = new double[count];
                await Task.Run(async () =>
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

    public static bool LessThanOrEqualToZero<T>(T value) where T : INumber<T> => value <= T.Zero;

    public static bool ManyLessThanOrEqualToZero<T>(params T[] values) where T : INumber<T>
    {
        if (values.Length < 1)
            return true;

        for (var i = 0; i < values.Length; i++)
        {
            if (LessThanOrEqualToZero(values[i]))
                return true;
        }

        return false;
    }

    public static async Task<bool> ManyLessThanOrEqualToZeroAsync<T>(params T[] values) where T : INumber<T>
    {
        if (values.Length < 1)
            return true;

        bool result = false;

        await Task.Run(async () =>
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (LessThanOrEqualToZero(values[i]))
                {
                    result = true;
                    break;
                }

                await Task.Yield();
            }
        }).ConfigureAwait(false);

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
                var count = listOfNumbers.GetLength();
                var newDoubleListOfNumbers = new double[count];
                for (var i = 0; i < count; i++)
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
                var count = listOfNumbers.GetLength();
                var newDoubleListOfNumbers = new double[count];
                await Task.Run(async () =>
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
                return T.CreateChecked(AdvancedMath.ZScore(in decimalListOfNumbers, in decimalZScore));

            case double[] doubleListOfNumbers when fallbackZScoreValue is double doubleZScore:
                return T.CreateChecked(AdvancedMath.ZScore(in doubleListOfNumbers, in doubleZScore));

            default:
                var count = listOfNumbers.GetLength();
                var newDoubleListOfNumbers = new double[count];
                for (var i = 0; i < count; i++)
                    newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));

                return T.CreateChecked(AdvancedMath.ZScore(in newDoubleListOfNumbers, double.CreateTruncating(fallbackZScoreValue)));
        }
    }

    public static async Task<T> ZScoreAsync<T>(IEnumerable<T> listOfNumbers, T fallbackZScoreValue) where T : INumber<T>
    {
        switch (listOfNumbers)
        {
            case decimal[] decimalListOfNumbers when fallbackZScoreValue is decimal decimalZScore:
                return T.CreateChecked(await AdvancedMath.ZScoreAsync(decimalListOfNumbers, decimalZScore).ConfigureAwait(false));

            case double[] doubleListOfNumbers when fallbackZScoreValue is double doubleZScore:
                return T.CreateChecked(AdvancedMath.ZScore(in doubleListOfNumbers, in doubleZScore));

            default:
                var count = listOfNumbers.GetLength();
                var newDoubleListOfNumbers = new double[count];
                await Task.Run(async () =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        newDoubleListOfNumbers[i] = double.CreateTruncating(listOfNumbers.AtIndex(i));
                        await Task.Yield();
                    }
                }).ConfigureAwait(false);

                return T.CreateChecked(AdvancedMath.ZScore(in newDoubleListOfNumbers, double.CreateTruncating(fallbackZScoreValue)));
        }
    }
}