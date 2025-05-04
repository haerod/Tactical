using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New cover type", menuName = "Basic Unity Tactical Tool/Cover", order = 2)]
public class CoverType : ScriptableObject
{
    [SerializeField] private Sprite coverFeedback;
    [SerializeField] private int coverProtectionPercent;
    
    /// <summary>
    /// Returns the feedback's cover sprite.
    /// </summary>
    /// <returns></returns>
    public Sprite GetCoverFeedbackSprite() => coverFeedback;
    
    /// <summary>
    /// Returns cover protection's percentage.
    /// </summary>
    /// <returns></returns>
    public int GetCoverProtectionPercent() => coverProtectionPercent;
}