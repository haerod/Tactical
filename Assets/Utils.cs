using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
    // Returns true if list is void or empty
    public static bool IsVoidList<T>(List<T> list)
    {
        if (list == null || list.Count == 0) return true;

        return false;
    }
}
