using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AdvancedMath;

public static class NumberExtensions
{
    public static T AtIndex<T>(this IEnumerable<T> source, int index)
    {
        ArgumentNullException.ThrowIfNull(source);

        try
        {
            if (source is T[] array)
                return array[index];

        }
        catch { }
        
        return source.ElementAt(index);
    }

    public static T Average<T>(this IEnumerable<T> source) where T : INumber<T> => source.Sum() / T.CreateChecked(source.GetLength());

    public static async Task<T> AverageAsync<T>(this IEnumerable<T> source) where T : INumber<T> => await source.SumAsync().ConfigureAwait(false) / T.CreateChecked(source.GetLength());

    public static int GetLength<TSource>(this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        try
        {
            if (source is TSource[] array)
                return array.Length;

        }
        catch { }

        return source.Count();
    }

    public static T Max<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        var max = source.First();

        //Do not Parallel Process
        foreach (var item in source)
        {
            if (item > max)
                max = item;

        }

        return max;
    }

    public static async Task<T> MaxAsync<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        var max = source.First();

        await Task.Run(async () =>
        {
            foreach (var item in source)
            {
                if (item > max)
                    max = item;

                await Task.Yield();
            }
        }).ConfigureAwait(false);

        return max;
    }

    public static T Sum<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        var sum = T.Zero;
        foreach (var item in source)
            sum += item;

        return sum;
    }

    public static async Task<T> SumAsync<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        var sum = T.Zero;
        await Task.Run(async () =>
        {
            foreach (var item in source)
            {
                sum += item;
                await Task.Yield();
            }
        }).ConfigureAwait(false);

        return sum;
    }
}