using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
    public static bool IsVoidList<T>(List<T> list)
    {
        if (list == null || list.Count == 0) return true;

        return false;
    }
}
