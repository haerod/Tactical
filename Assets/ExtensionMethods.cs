using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static T Next<T>(this List<T> list, int index)
    {
        if (list.Count == 0) return default;
        if (index + 1 < list.Count) return list[index + 1];
        else return list[0];
    }

    public static T Previous<T>(this List<T> list, int index)
    {
        if (list.Count == 0) return default;
        if (index - 1 >= 0) return list[index - 1];
        else return list[list.Count - 1];
    }

    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogWarning("List is empty !");
            return default;
        }

        return list[UnityEngine.Random.Range(0, list.Count)];
    }

}
