using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
    
    /// <summary>
    /// Rolls a die with X faces and returns the result.
    /// </summary>
    /// <param name="diceFaces"></param>
    /// <returns></returns>
    public static int DiceRoll(int diceFaces) => Random.Range(1, diceFaces+1);

    /// <summary>
    /// Returns the string with the pattern by the replacement value.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    public static string ReplacePatternInString(string content, string pattern, string replace) =>
        Regex.Replace(content,$@"\b{pattern}\b", replace);
    
    /// <summary>
    /// Returns the string with the given color.
    /// </summary>
    /// <param name="_content"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColoredText(string _content, Color color) =>
        $"<color=#{color.ToString()}>{_content}</color>";
    
    /// <summary>
    /// Returns the string with the given color.
    /// </summary>
    /// <param name="_content"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string SizedText(string _content, int size) => $"<size={size.ToString()}>{_content}</size>";
}
