﻿using System;
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

        try
        {
            if (source is TSource[] array)
                return array[index];

        }
        catch { }

        return source.ElementAt(index);
    }

    public static T Average<T>(this IEnumerable<T> source) where T : INumber<T> => source.Sum() / T.CreateChecked(source.GetLengthOrCount());

    public static async Task<T> AverageAsync<T>(this IEnumerable<T> source) where T : INumber<T> => await source.SumAsync().ConfigureAwait(false) / T.CreateChecked(source.GetLengthOrCount());

    public static int GetLengthOrCount<TSource>(this IEnumerable<TSource> source)
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

    public static bool LessThan<TSource>(this IEnumerable<TSource> source, int exclusive = 1)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (TryGetLengthOrCount(source, out int? count))
            return count < exclusive;

        count = 1;
        foreach (var _ in source)
        {
            if (count >= exclusive)
                return false;

            count++;
        }

        return true;
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

    public static bool TryGetLengthOrCount<TSource>(this IEnumerable<TSource> source, out int? length)
    {
        ArgumentNullException.ThrowIfNull(source);

        try
        {
            if (source is TSource[] array)
            {
                length = array.Length;
                return true;
            }

        }
        catch { }

        try
        {
            if (source is ICollection<TSource> collection)
            {
                length = collection.Count;
                return true;
            }
        }
        catch { }
        
        length = null;
        return false;
    }
}