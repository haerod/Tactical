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

    /// <summary>
    /// Returns a cover info representative of the cover situation of the character.
    /// Returns null if it's no cover around.
    /// Returns the most protective cover if it's not in enemies' view / there are not enemies / it's uncovered.
    /// Returns the most protective cover if it's covered.
    /// </summary>
    /// <returns></returns>
    public CoverInfo GetCoverState()
    {
        List<GameObject> coversAround = _board.GetAdjacentCoversAt(c.coordinates, GetCoveringTypes());

        if (coversAround.Count == 0)
            return null; // No cover around
        
        List<C_Look> enemiesInView = c.look.EnemiesInView()
            .Select(testedEnemy => testedEnemy.look)
            .Where(testedViewer => testedViewer.HasSightOn(c.tile))
            .ToList();

        if (enemiesInView.Count == 0)
            return new CoverInfo // No enemy around OR no enemy viewing the character
                (null, c.coordinates, null, true, GetMostProtectiveCoverTypeIn(coversAround));
        
        List<CoverInfo> coverInfos = new List<CoverInfo>();

        foreach (C_Look testedViewer in enemiesInView)
            coverInfos.Add(GetCoverStateFrom(c.coordinates, coversAround, testedViewer));

        if(coverInfos.Any(testedInfo => !testedInfo.GetIsCovered()))
            return new CoverInfo // Uncovered but a cover closed
                (null, c.coordinates, null, false, GetMostProtectiveCoverTypeIn(coversAround));
        
        return coverInfos // Covered
            .OrderByDescending(testedInfo => testedInfo.GetIsCovered())
            .ThenBy(testedInfo => testedInfo.GetCoverType().GetCoverProtectionPercent())
            .FirstOrDefault();
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the cover info at a coordinate, with a cover and a viewer.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="cover"></param>
    /// <param name="viewer"></param>
    /// <returns></returns>
    private CoverInfo GetCoverInfoFrom(Coordinates coveredCoordinates, GameObject cover, C_Look viewer)
    {
        Cover coverComponent = cover.GetComponent<Cover>();
        Tile coverComponentTile = _board.GetTileAtCoordinates(new Coordinates((int) cover.transform.position.x, (int) cover.transform.position.z));
        CoverType coverType = GetCoveringTypeOf(coverComponent ? coverComponent.type : coverComponentTile.type);
        
        CoverInfo coverInfo = new CoverInfo(
            cover,
            coveredCoordinates,
            coverComponent ? coverComponent.GetOtherSideCoordinates(coveredCoordinates) : new Coordinates((int) cover.transform.position.x, (int) cover.transform.position.z),
            IsCoverProtectingFrom(coveredCoordinates, cover, viewer),
            coverType);
        
        return new CoverInfo(
            cover,
            coveredCoordinates,
            coverComponent ? coverComponent.GetOtherSideCoordinates(coveredCoordinates) : new Coordinates((int) cover.transform.position.x, (int) cover.transform.position.z),
            IsCoverProtectingFrom(coveredCoordinates, cover, viewer),
            coverType);
    }
    
    /// <summary>
    /// Returns the most protecting cover in a list.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="coverList"></param>
    /// <param name="viewer"></param>
    /// <returns></returns>
    private CoverInfo GetCoverStateFrom(Coordinates coveredCoordinates, List<GameObject> coverList, C_Look viewer) => coverList
        .Select(testedCover => GetCoverInfoFrom(coveredCoordinates, testedCover, viewer))
        .OrderByDescending(testedInfo => testedInfo.GetIsCovered())
        .ThenBy(testedInfo => testedInfo.GetCoverType().GetCoverProtectionPercent())
        .FirstOrDefault();
    
    /// <summary>
    /// Returns true if the cover is protecting from a viewer, else returns false.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="cover"></param>
    /// <param name="viewer"></param>
    /// <returns></returns>
    private bool IsCoverProtectingFrom(Coordinates coveredCoordinates, GameObject cover, C_Look viewer)
    {
        Cover coverComponent = cover.GetComponent<Cover>();
        Tile coverComponentTile = _board.GetTileAtCoordinates(new Coordinates((int) cover.transform.position.x, (int) cover.transform.position.z));
        Vector2 coverPosition = coverComponent ? 
            new Vector2(coverComponent.coverPosition.x, coverComponent.coverPosition.y) : // Cover on edges
            new Vector2(coverComponentTile.coordinates.x,coverComponentTile.coordinates.y); // Cover on tile
        Vector2 characterPosition = coveredCoordinates.ToVector2();
        Vector2 viewerPosition = new Vector2(viewer.transform.position.x, viewer.transform.position.z);
                
        Vector2 coverForward = -(coverPosition-characterPosition);
        float angle = Vector2.Angle(coverForward, coverPosition - viewerPosition);
        return angle <= coveringAngle / 2;
    }

    /// <summary>
    /// Returns the cover type with the better protection in a list.
    /// </summary>
    /// <param name="covers"></param>
    /// <returns></returns>
    private CoverType GetMostProtectiveCoverTypeIn(List<GameObject> covers) => covers
        .Select(testedCover => testedCover.GetComponent<Cover>() ? GetCoveringTypeOf(testedCover.GetComponent<Cover>().type) : GetCoveringTypeOf(testedCover.GetComponent<Tile>().type))
        .OrderBy(testedCoverType => testedCoverType.GetCoverProtectionPercent())
        .FirstOrDefault();
    
    /// <summary>
    /// Returns the covering types of the character.
    /// </summary>
    /// <returns></returns>
    private List<TileType> GetCoveringTypes() => coveringTypes
        .SelectMany(currentTypeOfCover => currentTypeOfCover.GetCoveringTileTypes())
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
    [SerializeField] private List<TileType> coveringTileTypes;
    
    /// <summary>
    /// Returns the tile types which cover.
    /// </summary>
    /// <returns></returns>
    public List<TileType> GetCoveringTileTypes() => coveringTileTypes;
    
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
    public bool Contains(TileType tileType) => coveringTileTypes.Contains(tileType);
}

