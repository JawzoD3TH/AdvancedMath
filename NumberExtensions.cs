using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AdvancedMath;

public static class NumberExtensions
{
    public static TSource AtIndex<TSource>(this IEnumerable<TSource> source, int index)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        if (source is IList<TSource> list)
            return list[index];

        return source.ElementAt(index);
    }

    public static T Average<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(source);

        var sum = T.Zero;
        var count = 0;
        foreach (var item in source)
        {
            sum += item;
            count++;
        }

        if (count == 0)
            throw new InvalidOperationException("The sequence contains no elements.");

        return sum / T.CreateChecked(count);
    }

    public static Task<T> AverageAsync<T>(this IEnumerable<T> source) where T : INumber<T> =>
        Task.FromResult(source.Average());

    public static int GetLengthOrCount<TSource>(this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.TryGetNonEnumeratedCount(out var count) ? count : source.Count();
    }

    public static bool LessThan<TSource>(this IEnumerable<TSource> source, int exclusive = 1)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (exclusive <= 0)
            return false;

        if (source.TryGetNonEnumeratedCount(out var count))
            return count < exclusive;

        using var enumerator = source.GetEnumerator();
        for (var countSeen = 0; countSeen < exclusive; countSeen++)
        {
            if (!enumerator.MoveNext())
                return true;
        }

        return false;
    }

    public static T Max<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(source);

        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("The sequence contains no elements.");

        var max = enumerator.Current;
        while (enumerator.MoveNext())
        {
            if (enumerator.Current > max)
                max = enumerator.Current;
        }

        return max;
    }

    public static Task<T> MaxAsync<T>(this IEnumerable<T> source) where T : INumber<T> =>
        Task.FromResult(source.Max());

    public static T Sum<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(source);

        var sum = T.Zero;
        foreach (var item in source)
            sum += item;

        return sum;
    }

    public static Task<T> SumAsync<T>(this IEnumerable<T> source) where T : INumber<T> =>
        Task.FromResult(source.Sum());

    public static bool TryGetLengthOrCount<TSource>(this IEnumerable<TSource> source, out int? length)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.TryGetNonEnumeratedCount(out var count))
        {
            length = count;
            return true;
        }

        length = null;
        return false;
    }
}
