using System;
using System.Linq;
using System.Threading.Tasks;
using static AdvancedMath.ParallelTasks;

namespace AdvancedMath;

public static class AdvancedMath
{
    public const decimal DecimalTwo = 2m;
    public const double DoubleOne = 1d;
    public const double DoubleTwo = 2d;
    public const decimal FastAccuracy = 0.0001m;
    public const decimal PrecisionAccuracy = 0.0000000000000000000000000001m;
    public const int SmallArraySizeLimit = 63;

    public static double CoefficientOfVariation(in double[] listOfNumbers)
    {
        if (listOfNumbers.LessThan(2))
            return DoubleOne;

        var result = StandardDeviation(in listOfNumbers) / listOfNumbers.Average();
        return Generic.LessThanOrEqualToZero(result) ? DoubleOne : result;
    }

    public static decimal CoefficientOfVariation(in decimal[] listOfNumbers)
    {
        if (listOfNumbers.LessThan(2))
            return decimal.One;

        var result = StandardDeviation(in listOfNumbers) / listOfNumbers.Average();
        return Generic.LessThanOrEqualToZero(result) ? decimal.One : result;
    }

    public static async Task<decimal> CoefficientOfVariationAsync(decimal[] listOfNumbers)
    {
        if (listOfNumbers.LessThan(2))
            return decimal.One;

        var result = await StandardDeviationAsync(listOfNumbers).ConfigureAwait(false) / await listOfNumbers.AverageAsync().ConfigureAwait(false);
        return Generic.LessThanOrEqualToZero(result) ? decimal.One : result;
    }

    public static bool IsLargeArray(int length) => length > SmallArraySizeLimit;

    public static decimal Power(in decimal value, int power)
    {
        if (value == decimal.Zero)
            return decimal.Zero;

        switch (power)
        {
            case 0:
                return decimal.One;

            case 1:
                return value;
        }

        var isNegativePower = power < 0;
        if (isNegativePower)
            power = -power;

        var result = decimal.One;
        var baseValue = value;

        while (power > 0)
        {
            if ((power & 1) == 1)
                result *= baseValue;

            baseValue *= baseValue;
            power >>= 1;
        }

        return isNegativePower ? decimal.One / result : result;
    }

    public static async Task<decimal> PowerAsync(decimal value, int power)
    {
        if (value == decimal.Zero)
            return decimal.Zero;

        switch (power)
        {
            case 0:
                return decimal.One;

            case 1:
                return value;
        }

        var isNegativePower = power < 0;
        if (isNegativePower)
            power = -power;

        var result = decimal.One;
        var baseValue = value;

        return await Task.Run(async () =>
        {
            while (power > 0)
            {
                if ((power & 1) == 1)
                    result *= baseValue;

                baseValue *= baseValue;
                power >>= 1;

                await Task.Yield();
            }

            return isNegativePower ? decimal.One / result : result;
        }).ConfigureAwait(false);
    }

    public static decimal SquareRoot(in decimal value, decimal accuracy = FastAccuracy)
    {
        if (Generic.LessThanOrEqualToZero(value))
            return decimal.Zero;

        if (accuracy < PrecisionAccuracy)
            accuracy = PrecisionAccuracy;

        var guess = value / DecimalTwo;
        var result = (guess + value / guess) / DecimalTwo;

        while (Math.Abs(guess - result) > accuracy)
        {
            guess = result;
            result = (guess + value / guess) / DecimalTwo;
        }

        return result;
    }

    public static async Task<decimal> SquareRootAsync(decimal value, decimal accuracy = FastAccuracy)
    {
        if (Generic.LessThanOrEqualToZero(value))
            return decimal.Zero;

        if (accuracy < PrecisionAccuracy)
            accuracy = PrecisionAccuracy;

        var guess = value / DecimalTwo;
        var result = (guess + value / guess) / DecimalTwo;

        return await Task.Run(async () =>
        {
            while (Math.Abs(guess - result) > accuracy)
            {
                guess = result;
                result = (guess + value / guess) / DecimalTwo;

                await Task.Yield();
            }

            return result;
        }).ConfigureAwait(false);
    }

    public static double StandardDeviation(in double[] listOfNumbers)
    {
        if (listOfNumbers.LessThan(2))
            return double.NegativeZero;

        var mean = listOfNumbers.Average();

        try
        {
            var sumOfSquares = listOfNumbers.Sum(x => (x - mean) * (x - mean));
            var variance = sumOfSquares / (listOfNumbers.Length - 1);

            return Math.Sqrt(variance);
        }
        catch { return Math.Sqrt(listOfNumbers.Sum(x => Math.Pow(x - mean, 2)) / listOfNumbers.Length - 1); }
    }

    public static decimal StandardDeviation(in decimal[] listOfNumbers)
    {
        if (listOfNumbers.LessThan(2))
            return decimal.Zero;

        var mean = listOfNumbers.Average();

        try
        {
            var sumOfSquares = listOfNumbers.Sum(x => (x - mean) * (x - mean));
            var variance = sumOfSquares / (listOfNumbers.Length - 1);

            return SquareRoot(in variance);
        }
        catch { return SquareRoot(listOfNumbers.Sum(x => Power(x - mean, 2)) / (listOfNumbers.Length - 1)); }
    }

    public static async Task<decimal> StandardDeviationAsync(decimal[] listOfNumbers)
    {
        if (listOfNumbers.LessThan(2))
            return decimal.Zero;

        var mean = await listOfNumbers.AverageAsync().ConfigureAwait(false);

        try
        {
            var sumOfSquares = listOfNumbers.Sum(x => (x - mean) * (x - mean));
            var variance = sumOfSquares / (listOfNumbers.Length - 1);

            return await SquareRootAsync(variance).ConfigureAwait(false);
        }
        catch { return await SquareRootAsync(listOfNumbers.Sum(x => Power(x - mean, 2)) / (listOfNumbers.Length - 1)).ConfigureAwait(false); }
    }

    public static decimal ZScore(decimal[] listOfNumbers, in decimal fallbackZScoreValue)
    {
        try
        {
            var mean = listOfNumbers.Average();
            var standardDeviation = StandardDeviation(in listOfNumbers);

            if (standardDeviation == decimal.Zero)
                return decimal.Zero;

            var zScores = new decimal[listOfNumbers.Length];

            if (CanParallelProcess() && IsLargeArray(listOfNumbers.Length))
                Parallel.For(0, listOfNumbers.Length, ParallelSettings, i => zScores[i] = (listOfNumbers[i] - mean) / standardDeviation);
            else for (var i = 0; i < listOfNumbers.Length; i++)
            {
                zScores[i] = (listOfNumbers[i] - mean) / standardDeviation;

                //Ensure positive score:
                /* if (zScores[i].LessThan(0))
                    zScores[i] = zScores[i] * decimal.MinusOne; */
            }

            return zScores.Average();
        }
        catch { return fallbackZScoreValue; }
    }

    public static double ZScore(double[] listOfNumbers, in double fallbackZScoreValue)
    {
        try
        {
            var mean = listOfNumbers.Average();
            var standardDeviation = StandardDeviation(in listOfNumbers);

            if (standardDeviation == double.NegativeZero)
                return double.NegativeZero;

            var zScores = new double[listOfNumbers.Length];
            if (CanParallelProcess() && IsLargeArray(listOfNumbers.Length))
                Parallel.For(0, listOfNumbers.Length, ParallelSettings, i => zScores[i] = (listOfNumbers[i] - mean) / standardDeviation);
            else for (var i = 0; i < listOfNumbers.Length; i++)
            {
                zScores[i] = (listOfNumbers[i] - mean) / standardDeviation;

                //Ensure positive score:
                /* if (zScores[i].LessThan(0))
                    zScores[i] = zScores[i] * decimal.MinusOne; */
                }

            return zScores.Average();
        }
        catch { return fallbackZScoreValue; }
    }

    public static async Task<decimal> ZScoreAsync(decimal[] listOfNumbers, decimal fallbackZScoreValue)
    {
        try
        {
            var mean = await listOfNumbers.AverageAsync().ConfigureAwait(false);
            var standardDeviation = await StandardDeviationAsync(listOfNumbers).ConfigureAwait(false);

            if (standardDeviation == decimal.Zero)
                return decimal.Zero;

            var zScores = new decimal[listOfNumbers.Length];
            if (CanParallelProcess() && IsLargeArray(listOfNumbers.Length))
            {
                await Parallel.ForAsync(0, listOfNumbers.Length, ParallelSettings, async (i, _) =>
                {
                    zScores[i] = (listOfNumbers[i] - mean) / standardDeviation;

                    //Ensure positive score:
                    /* if (zScores[i].LessThan(0))
                        zScores[i] = zScores[i] * decimal.MinusOne; */

                    await Task.Yield();
                }).ConfigureAwait(false);
            }
            else await Task.Run(async () =>
                {
                    for (var i = 0; i < listOfNumbers.Length; i++)
                    {
                        zScores[i] = (listOfNumbers[i] - mean) / standardDeviation;
                        await Task.Yield();
                    }
                }).ConfigureAwait(false);

            return await zScores.AverageAsync().ConfigureAwait(false);
        }
        catch { return fallbackZScoreValue; }
    }
}