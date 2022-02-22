using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    // ======================================================================
    // LISTS
    // ======================================================================

    /// <summary>
    /// Return the next element of the list; after the index.
    /// Return the first one if the index is the last one.
    /// Return default if the list count equals .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T Next<T>(this List<T> list, int index)
    {
        if (list.Count == 0) return default;
        if (index + 1 < list.Count) return list[index + 1];
        else return list[0];
    }

    /// <summary>
    /// Return the previous element of the list; after the index.
    /// Return the last one if the index is the last one.
    /// Return default if the list count equals .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T Previous<T>(this List<T> list, int index)
    {
        if (list.Count == 0) return default;
        if (index - 1 >= 0) return list[index - 1];
        else return list[list.Count - 1];
    }

    /// <summary>
    /// Return a random element of a list.
    /// Return default if the list is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogWarning("List is empty !");
            return default;
        }

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Print all the elements of a list in the console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Print<T>(this List<T> list)
    {
        foreach (var item in list)
        {
            Debug.Log(item);
        }
    }

    // ======================================================================
    // INT
    // ======================================================================

    /// <summary>
    /// Return incremented int until the maximum value (inclusive).
    /// If int is more than it maximum, return 0.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maximumInclusive"></param>
    /// <returns></returns>
    public static int Next(this int value, int maximumInclusive)
    {
        value++;
        if (value > maximumInclusive) value = 0;
        return value;
    }

    /// <summary>
    /// Return decremented int from the maximum value (inclusive) to 0.
    /// If int is less than 0, return maximum.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maximumInclusive"></param>
    /// <returns></returns>
    public static int Previous(this int value, int maximumInclusive)
    {
        value--;
        if (value < 0) value = maximumInclusive;
        return value;
    }
}
