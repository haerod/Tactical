using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;
using static M__Managers;

public class C_Cover : MonoBehaviour
{
    [SerializeField] private List<CoveringElement> coveringTypes;
    [SerializeField] private float coveringAngle = 180;
    
    [Header("REFERENCES")]
    [SerializeField] private C__Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the cover infos of all walkable tiles in a given range around coordinates (0 = given coordinates). 
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<CoverInfo> GetAllCoverInfosInRangeAt(Coordinates coordinates, int range)
    {
        List<CoverInfo> infosToReturn = new List<CoverInfo>();
        List<C_Look> viewersInView = c.look
            .EnemiesInView()
            .Select(character => character.look)
            .ToList();

        if (viewersInView.Count == 0)
            return infosToReturn; // Nobody in view
        
        List<Coordinates> walkableCoordinatesInSquare = _board
            .GetFullSquareCoordinatesWithRadius(coordinates, range)
            .Where(checkedCoordinates => c.move.CanWalkAt(checkedCoordinates))
            .ToList();

        if (walkableCoordinatesInSquare.Count == 0)
            return infosToReturn; // No walkable tiles around
        
        foreach (Coordinates coordinatesToCheck in walkableCoordinatesInSquare)
        {
            List<GameObject> adjacentCoversList = _board.GetAdjacentCoversAt(coordinatesToCheck, GetCoveringTypes());
            
            if (adjacentCoversList.Count == 0)
            {
                infosToReturn.Add(new CoverInfo(null, coordinatesToCheck, null, false, null));
                continue; // No covers around
            }

            foreach (GameObject adjacentCover in adjacentCoversList)
            {
                CoverInfo betterInfo = null;
                
                foreach (C_Look testedViewer in viewersInView)
                {
                    if (betterInfo == null)
                    {
                        betterInfo = GetCoverInfoFrom(coordinatesToCheck, adjacentCover, testedViewer);
                        continue; // First info
                    }
                    
                    if(!betterInfo.GetIsCovered())
                        continue; // Is not a covered position
                    
                    CoverInfo testedInfo = GetCoverInfoFrom(coordinatesToCheck, adjacentCover, testedViewer);
                    
                    if(!testedInfo.GetIsCovered())
                        betterInfo = testedInfo;
                }

                infosToReturn.Add(betterInfo);
            }
        }
        
        return infosToReturn
            .ToList();
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the cover info at a coordinate.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="cover"></param>
    /// <param name="viewer"></param>
    /// <returns></returns>
    private CoverInfo GetCoverInfoFrom(Coordinates coveredCoordinates, GameObject cover, C_Look viewer)
    {
        Cover coverComponent = cover.GetComponent<Cover>();
        Tile coverComponentTile = _board.GetTileAtCoordinates(new Coordinates((int) cover.transform.position.x, (int) cover.transform.position.z));
        Vector2 coverPosition = coverComponent ? 
            new Vector2(coverComponent.coverPosition.x, coverComponent.coverPosition.y) : // Cover on edges
            new Vector2(coverComponentTile.coordinates.x,coverComponentTile.coordinates.y); // Cover on tile
        Vector2 characterPosition = coveredCoordinates.ToVector2();
        Vector2 viewerPosition = new Vector2(viewer.transform.position.x, viewer.transform.position.z);
        CoverType coverType = GetCoveringTypeOf(coverComponent ? coverComponent.type : coverComponentTile.type);
                
        Vector2 coverForward = -(coverPosition-characterPosition);
        float angle = Vector2.Angle(coverForward, coverPosition - viewerPosition);
        
        return new CoverInfo(
            cover,
            coveredCoordinates,
            coverComponent ? coverComponent.GetOtherSideCoordinates(coveredCoordinates) : new Coordinates((int) cover.transform.position.x, (int) cover.transform.position.z),
            angle <= coveringAngle / 2,
            coverType);
    }

    /// <summary>
    /// Returns the covering types of the character.
    /// </summary>
    /// <returns></returns>
    private List<TileType> GetCoveringTypes() => coveringTypes
        .SelectMany(currentTypeOfCover => currentTypeOfCover.GetTileTypes())
        .ToList();
    
    /// <summary>
    /// Returns the covering type corresponding at a type of tile.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    private CoverType GetCoveringTypeOf(TileType tileType)
    {
        foreach (CoveringElement testedTypeOfCover in coveringTypes)
        {
            if(testedTypeOfCover.Contains(tileType))
                return testedTypeOfCover.GetCoverType();
        }
        
        return null;
    }
    
}

[Serializable]
public class CoverInfo
{
    [SerializeField] private GameObject cover;
    [SerializeField] private Coordinates coveredCoordinates;
    [SerializeField] private Coordinates coverFeedbackCoordinates;
    [SerializeField] private bool isCovered;
    [SerializeField] private CoverType coverType;
    
    public CoverInfo(GameObject cover, Coordinates coveredPosition, Coordinates coverFeedbackPosition, bool isCovered, CoverType coverType)
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
    public GameObject GetCover() => cover;
    
    /// <summary>
    /// Returns the type of cover.
    /// </summary>
    /// <returns></returns>
    public CoverType GetCoverType() => coverType;
}

[Serializable]
public class CoveringElement
{
    [SerializeField] private CoverType coverType;
    [SerializeField] private List<TileType> tileTypes;
    
    /// <summary>
    /// Returns the tile types list of this element.
    /// </summary>
    /// <returns></returns>
    public List<TileType> GetTileTypes() => tileTypes;
    
    /// <summary>
    /// Returns the cover type of this element.
    /// </summary>
    /// <returns></returns>
    public CoverType GetCoverType() => coverType;
    
    /// <summary>
    /// Returns true if this tile type is a cover.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public bool Contains(TileType tileType) => tileTypes.Contains(tileType);
}

