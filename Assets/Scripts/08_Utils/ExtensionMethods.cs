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
    /// Return the next element of the list; after the element.
    /// Return the first one if the index is the last one.
    /// Return default if the list count equals .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T Next<T>(this List<T> list, T element)
    {
        int index = list.IndexOf(element);
        if (list.Count == 0) return default;
        if (index + 1 < list.Count) return list[index + 1];
        else return list[0];
    }

    /// <summary>
    /// Return the previous element of the list; before the index.
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
    /// Return the previous element of the list; before the element.
    /// Return the last one if the index is the last one.
    /// Return default if the list count equals .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T Previous<T>(this List<T> list, T element)
    {
        int index = list.IndexOf(element);
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
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log($"Item {i}: {list[i]}");
        }
    }

    /// <summary>
    /// Add only if it doensn't exist in the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="newItem"></param>
    public static void AddIfNew<T>(this List<T> list, T newItem)
    {
        if (!list.Contains(newItem))
            list.Add(newItem);
    }

    /// <summary>
    /// Add if the condition is true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <param name="condition"></param>
    public static void AddIf<T>(this List<T> list, T item, bool condition)
    {
        if (condition)
            list.Add(item);
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

    // ======================================================================
    // ARRAY[] (one dimension)
    // ======================================================================

    /// <summary>
    /// Print all the elements of a two dimensional array in the console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Print<T>(this T[] array, bool withIndex = false)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            if (withIndex)
                Debug.Log($"Index {i}: {array[i]}");
            else
                Debug.Log(array[i]);
        }
    }

    /// <summary>
    /// Return true if the item [index0] is out of the array bounds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool IsInBounds<T>(this T[] array, int index)
    {
        if (index < 0) return false;
        if (index >= array.Length) return false;
        return true;
    }

    // ======================================================================
    // ARRAY[,] (two dimensions)
    // ======================================================================

    /// <summary>
    /// Print all the elements of a two dimensional array in the console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Print<T>(this T[,] array, bool withIndex = false)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                if (withIndex)
                    Debug.Log($"{array[i, j]} ({i},{j})");
                else
                    Debug.Log(array[i, j]);
            }
        }
    }

    /// <summary>
    /// Print the first and second lenghtes of a two dimensional array in the console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void PrintLengthes<T>(this T[,] array)
    {
        Debug.Log($"{array}: {array.GetLength(0)},{array.GetLength(1)}");
    }

    /// <summary>
    /// Return true if the item [index0, index1] is out of the array bounds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index0"></param>
    /// <param name="index1"></param>
    /// <returns></returns>
    public static bool IsInBounds<T>(this T[,] array, int index0, int index1)
    {
        if (index0 < 0) return false;
        if (index1 < 0) return false;
        if (index0 >= array.GetLength(0)) return false;
        if (index1 >= array.GetLength(1)) return false;
        return true;
    }
}
