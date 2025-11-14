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
    public static bool IsVoidList<T>(List<T> list) => list == null || list.Count == 0;

    /// <summary>
    /// Round to the closest value + 0.5 (ex : 0.5, 1.5, 2.5, ...).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float RoundToHalf(float value) => Mathf.Round(value * 2) / 2;
	
    /// <summary>
    /// Return a random boolean.
    /// </summary>
    /// <returns></returns>
    public static bool RandomBool() => Random.Range(0, 2) == 0;
    
    /// <summary>
	/// Returns renderer's bounds of an object, with all its children. 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();
        Renderer[ ] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            //Find first enabled renderer to start encapsulate from it
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    bounds = renderer.bounds;
                    break;
                }
            }
            //Encapsulate for all renderers
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return bounds;
    }
}
