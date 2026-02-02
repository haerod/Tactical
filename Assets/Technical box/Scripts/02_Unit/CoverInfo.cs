using System;
using UnityEngine;

[Serializable]
public class CoverInfo
{
    [SerializeField] private Cover cover;
    [SerializeField] private Coordinates coveredCoordinates;
    [SerializeField] private Coordinates coverFeedbackCoordinates;
    [SerializeField] private bool isCovered;
    [SerializeField] private CoverType coverType;
    
    public CoverInfo(Cover cover, Coordinates coveredPosition, Coordinates coverFeedbackPosition, bool isCovered, CoverType coverType)
    {
        this.cover = cover;
        this.coveredCoordinates = coveredPosition;
        this.coverFeedbackCoordinates = coverFeedbackPosition;
        this.isCovered = isCovered;
        this.coverType = coverType;
    }
    
    /// <summary>
    /// Returns if cover protects.
    /// </summary>
    /// <returns></returns>
    public bool GetIsCovered() => isCovered;
    
    /// <summary>
    /// Returns the covered position's coordinates.
    /// </summary>
    /// <returns></returns>
    public Coordinates GetCoveredCoordinates() => coveredCoordinates;
    
    /// <summary>
    /// Returns the coordinates where to place the cover feedback.
    /// </summary>
    /// <returns></returns>
    public Coordinates GetCoverFeedbackCoordinates() => coverFeedbackCoordinates;
    
    /// <summary>
    /// Returns the cover game object.
    /// </summary>
    /// <returns></returns>
    public Cover GetCover() => cover;
    
    /// <summary>
    /// Returns the type of cover.
    /// </summary>
    /// <returns></returns>
    public CoverType GetCoverType() => coverType;

    public override string ToString()
    {
        return isCovered
            ? $"Cover {cover.gameObject.name} of type {coverType.name} covers {coveredCoordinates}"
            : $"Cover {cover.gameObject.name} of type {coverType.name} don't cover {coveredCoordinates}";
    }
}