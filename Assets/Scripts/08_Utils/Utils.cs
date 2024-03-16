using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
    /// <summary>
    /// Returns true if list is void or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static bool IsVoidList<T>(List<T> list)
    {
        if (list == null || list.Count == 0) return true;

        return false;
    }
}
