using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New cover type", menuName = "Basic Unity Tactical Tool/Cover", order = 2)]
public class CoverType : ScriptableObject
{
    [SerializeField] private Sprite coverFeedback;
    [SerializeField] private int coverProtectionPercent;
    [SerializeField] private List<TileType> coveringTileTypes;

    /// <summary>
    /// Returns covering tile types.
    /// </summary>
    /// <returns></returns>
    public List<TileType> GetCoveringTileTypes() => coveringTileTypes;
    
    /// <summary>
    /// Returns true if this cover type contains the tile type.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public bool Contains(TileType tileType) => coveringTileTypes.Contains(tileType);
    
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