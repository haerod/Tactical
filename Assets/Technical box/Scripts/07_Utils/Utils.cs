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
    
    /// <summary>
    /// Returns the given sprite asset.
    /// </summary>
    /// <param name="_spriteName"></param>
    /// <returns></returns>
    public static string SpriteAsset(string _spriteName) => $"<sprite name=\"{_spriteName}\">";
    
    /// <summary>
    /// Returns the percent corresponding to the value in the given range.
    /// Ex: in a range from 2 to 6 => if value is 2 returns 0 ; 4 returns 0.5, 6 returns 1.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_min"></param>
    /// <param name="_max"></param>
    /// <returns></returns>
    public static float PercentInRange(float _value, float _min, float _max) => (_value -_min)/(_max - _min);
    
    /// <summary>
    /// Changes the corners value of a STRETCHED rect transform with a percentage, between 0 (min) and 1 (max).
    /// Ex: percentX = 0.5 ; percentY = 1 => shrinks UI of 50% on X axis.
    /// </summary>
    /// <param name="_rectTransform"></param>
    /// <param name="_percentX"></param>
    /// <param name="_percentY"></param>
    public static void ScaleStretchedUIInPercent(RectTransform _rectTransform, float _percentX, float _percentY)
    {
        float _width = _rectTransform.rect.width;
        float _height = _rectTransform.rect.height;
        Vector2 _stretchSize = new (_width/2 * (1-_percentX), _height/2 * (1-_percentY));;
        _rectTransform.offsetMin = _stretchSize;
        _rectTransform.offsetMax = -_stretchSize;
    }
}
