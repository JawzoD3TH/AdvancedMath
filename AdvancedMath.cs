using System;
using System.Threading.Tasks;

namespace AdvancedMath;

public static class AdvancedMath
{
    public const decimal DecimalTwo = 2m;
    public const double DoubleOne = 1d;
    public const double DoubleTwo = 2d;
    public const decimal FastAccuracy = 0.0001m;
    public const decimal PrecisionAccuracy = 0.0000000000000000000000000001m;
    public const int SmallArraySizeLimit = 63;

    /// <summary>
    /// Calculates the sample coefficient of variation, standard deviation divided by
    /// the arithmetic mean. A single value has no measurable variation and returns zero.
    /// </summary>
    public static double CoefficientOfVariation(in double[] listOfNumbers)
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        if (listOfNumbers.Length < 2)
            return 0d;

        var mean = listOfNumbers.Average();
        var standardDeviation = StandardDeviation(in listOfNumbers);

        // CV is undefined for a non-zero standard deviation and a zero mean.
        // IEEE floating point gives the useful representation of that result.
        if (mean == 0d)
            return standardDeviation == 0d ? 0d : double.PositiveInfinity;

        return standardDeviation / mean;
    }

    /// <summary>
    /// Calculates the sample coefficient of variation, standard deviation divided by
    /// the arithmetic mean. A single value has no measurable variation and returns zero.
    /// </summary>
    public static decimal CoefficientOfVariation(in decimal[] listOfNumbers)
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        if (listOfNumbers.Length < 2)
            return decimal.Zero;

        var mean = listOfNumbers.Average();
        var standardDeviation = StandardDeviation(in listOfNumbers);

        if (mean == decimal.Zero)
        {
            if (standardDeviation == decimal.Zero)
                return decimal.Zero;

            throw new DivideByZeroException(
                "The coefficient of variation is undefined when the mean is zero.");
        }

        return standardDeviation / mean;
    }

    public static Task<decimal> CoefficientOfVariationAsync(decimal[] listOfNumbers) =>
        Task.FromResult(CoefficientOfVariation(in listOfNumbers));

    public static bool IsLargeArray(int length) => length > SmallArraySizeLimit;

    /// <summary>
    /// Raises a decimal value to an integer power using exponentiation by squaring.
    /// </summary>
    public static decimal Power(in decimal value, int power)
    {
        // 0^0 is conventionally defined as 1 for integer-power operations.
        if (power == 0)
            return decimal.One;

        if (value == decimal.Zero)
        {
            if (power < 0)
                throw new DivideByZeroException("Zero cannot be raised to a negative power.");

            return decimal.Zero;
        }

        var isNegativePower = power < 0;
        long exponent = power;
        if (isNegativePower)
        {
            // Widen before negating: -int.MinValue cannot be represented by an int.
            exponent = -exponent;
        }

        // Inverting before the loop avoids calculating a large positive power and
        // then taking its reciprocal. It also avoids avoidable intermediate overflow.
        var baseValue = isNegativePower ? decimal.One / value : value;
        var result = decimal.One;

        while (exponent > 0)
        {
            if ((exponent & 1) != 0)
                result *= baseValue;

            exponent >>= 1;
            if (exponent > 0)
                baseValue *= baseValue;
        }

        return result;
    }

    public static Task<decimal> PowerAsync(decimal value, int power) =>
        Task.FromResult(Power(in value, power));

    /// <summary>
    /// Calculates a non-negative square root with Newton's method.
    /// </summary>
    public static decimal SquareRoot(in decimal value, decimal accuracy = FastAccuracy)
    {
        if (value < decimal.Zero)
            throw new ArgumentOutOfRangeException(nameof(value), value, "A square root requires a non-negative value.");

        if (value == decimal.Zero)
            return decimal.Zero;

        if (accuracy <= decimal.Zero)
            accuracy = PrecisionAccuracy;

        // For values below one, one is a much better initial estimate than value / 2.
        var guess = value >= decimal.One ? value / DecimalTwo : decimal.One;

        // Decimal has finite precision. The equality check prevents an endless
        // loop when Newton iteration has reached its rounding fixed point.
        const int maximumIterations = 256;
        for (var iteration = 0; iteration < maximumIterations; iteration++)
        {
            var result = (guess + value / guess) / DecimalTwo;
            if (result == guess || Math.Abs(result - guess) <= accuracy)
                return result;

            guess = result;
        }

        return guess;
    }

    public static Task<decimal> SquareRootAsync(decimal value, decimal accuracy = FastAccuracy) =>
        Task.FromResult(SquareRoot(in value, accuracy));

    /// <summary>
    /// Calculates the sample standard deviation using a numerically stable
    /// Welford recurrence.
    /// </summary>
    public static double StandardDeviation(in double[] listOfNumbers)
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        if (listOfNumbers.Length < 2)
            return 0d;

        double mean = 0d;
        double sumOfSquaredDifferences = 0d;

        for (var i = 0; i < listOfNumbers.Length; i++)
        {
            var value = listOfNumbers[i];
            var delta = value - mean;
            mean += delta / (i + 1);
            var differenceFromNewMean = value - mean;
            sumOfSquaredDifferences += delta * differenceFromNewMean;
        }

        // A tiny negative value can only be caused by floating-point rounding.
        if (sumOfSquaredDifferences < 0d)
            sumOfSquaredDifferences = 0d;

        return Math.Sqrt(sumOfSquaredDifferences / (listOfNumbers.Length - 1));
    }

    /// <summary>
    /// Calculates the sample standard deviation using a numerically stable
    /// Welford recurrence.
    /// </summary>
    public static decimal StandardDeviation(in decimal[] listOfNumbers)
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        if (listOfNumbers.Length < 2)
            return decimal.Zero;

        decimal mean = decimal.Zero;
        decimal sumOfSquaredDifferences = decimal.Zero;

        for (var i = 0; i < listOfNumbers.Length; i++)
        {
            var value = listOfNumbers[i];
            var delta = value - mean;
            mean += delta / (i + 1);
            var differenceFromNewMean = value - mean;
            sumOfSquaredDifferences += delta * differenceFromNewMean;
        }

        if (sumOfSquaredDifferences < decimal.Zero)
            sumOfSquaredDifferences = decimal.Zero;

        var variance = sumOfSquaredDifferences / (listOfNumbers.Length - 1);
        return SquareRoot(in variance, PrecisionAccuracy);
    }

    public static Task<decimal> StandardDeviationAsync(decimal[] listOfNumbers) =>
        Task.FromResult(StandardDeviation(in listOfNumbers));

    /// <summary>
    /// Returns the mean of the computed z-scores, preserving the existing scalar
    /// API. For a list standardized around its own mean this value is expected to
    /// be approximately zero by definition.
    /// </summary>
    public static decimal ZScore(decimal[] listOfNumbers, in decimal fallbackZScoreValue)
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        try
        {
            if (listOfNumbers.Length < 2)
                return fallbackZScoreValue;

            var mean = listOfNumbers.Average();
            var standardDeviation = StandardDeviation(in listOfNumbers);

            if (standardDeviation == decimal.Zero)
                return decimal.Zero;

            var sumOfZScores = decimal.Zero;
            foreach (var value in listOfNumbers)
                sumOfZScores += (value - mean) / standardDeviation;

            return sumOfZScores / listOfNumbers.Length;
        }
        catch (ArithmeticException)
        {
            return fallbackZScoreValue;
        }
    }

    public static double ZScore(double[] listOfNumbers, in double fallbackZScoreValue)
    {
        ArgumentNullException.ThrowIfNull(listOfNumbers);

        try
        {
            if (listOfNumbers.Length < 2)
                return fallbackZScoreValue;

            var mean = listOfNumbers.Average();
            var standardDeviation = StandardDeviation(in listOfNumbers);

            if (standardDeviation == 0d)
                return 0d;

            if (double.IsNaN(standardDeviation) || double.IsInfinity(standardDeviation))
                return fallbackZScoreValue;

            var sumOfZScores = 0d;
            foreach (var value in listOfNumbers)
                sumOfZScores += (value - mean) / standardDeviation;

            var result = sumOfZScores / listOfNumbers.Length;
            return double.IsNaN(result) || double.IsInfinity(result)
                ? fallbackZScoreValue
                : result;
        }
        catch (ArithmeticException)
        {
            return fallbackZScoreValue;
        }
    }

    public static Task<decimal> ZScoreAsync(decimal[] listOfNumbers, decimal fallbackZScoreValue) =>
        Task.FromResult(ZScore(listOfNumbers, in fallbackZScoreValue));
}
