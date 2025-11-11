using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;
using static M__Managers;

public class U_Cover : MonoBehaviour
{
    [SerializeField] private List<CoverType> coverTypes;
    [SerializeField] private float coveringAngle = 180;
    
    [Header("REFERENCES")]
    [SerializeField] private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns true if there is at less a cover around the unit's coordinates.
    /// </summary>
    /// <returns></returns>
    public bool AreCoversAround() => _board.GetAdjacentCoversAt(unit.coordinates, GetCoveringTileTypes()).Count > 0;
    
    /// <summary>
    /// Returns the cover infos of all walkable tiles in a given range around coordinates (0 = given coordinates). 
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<CoverInfo> GetAllCoverInfosInRangeAt(Coordinates coordinates, int range)
    {
        List<CoverInfo> infosToReturn = new();
        List<U__Unit> enemiesInView = unit.look
            .EnemiesVisibleInFog()
            .ToList();

        if (enemiesInView.Count == 0)
            return infosToReturn; // Nobody in view
        
        List<Coordinates> walkableCoordinatesInSquare = _board
            .GetFullSquareCoordinatesWithRadius(coordinates, range)
            .Where(checkedCoordinates => unit.move.CanWalkAt(checkedCoordinates))
            .ToList();
        
        if (walkableCoordinatesInSquare.Count == 0)
            return infosToReturn; // No walkable tiles around
        
        foreach (Coordinates coordinatesToCheck in walkableCoordinatesInSquare)
        {
            List<Cover> adjacentCoversList = _board.GetAdjacentCoversAt(coordinatesToCheck, GetCoveringTileTypes());
            
            if (adjacentCoversList.Count == 0)
            {
                infosToReturn.Add(new CoverInfo(null, coordinatesToCheck, null, false, null));
                continue; // No covers around
            }

            foreach (Cover adjacentCover in adjacentCoversList)
            {
                CoverInfo betterInfo = null;
                
                foreach (U__Unit testedEnemy in enemiesInView)
                {
                    if (betterInfo == null)
                    {
                        betterInfo = GetCoverInfoFrom(coordinatesToCheck, adjacentCover, testedEnemy);
                        continue; // First info
                    }
                    
                    if(!betterInfo.GetIsCovered())
                        continue; // Is not a covered position
                    
                    CoverInfo testedInfo = GetCoverInfoFrom(coordinatesToCheck, adjacentCover, testedEnemy);
                    
                    if(!testedInfo.GetIsCovered())
                        betterInfo = testedInfo;
                }

                infosToReturn.Add(betterInfo);
            }
        }
        
        return infosToReturn;
    }

    /// <summary>
    /// Returns a cover info representative of the cover situation of the unit.
    /// Returns null if it's no cover around.
    /// Returns the most protective cover if it's not in enemies' view / there are not enemies / it's uncovered.
    /// Returns the most protective cover if it's covered.
    /// </summary>
    /// <returns></returns>
    public CoverInfo GetCoverState()
    {
        List<Cover> coversAround = _board.GetAdjacentCoversAt(unit.coordinates, GetCoveringTileTypes());

        if (coversAround.Count == 0)
            return null; // No cover around
        
        List<U__Unit> enemiesInView = unit.look.EnemiesVisibleInFog()
            .Where(testedUnit => testedUnit.look.CanSee(unit))
            .ToList();

        if (enemiesInView.Count == 0)
            return new CoverInfo // No enemy around OR no enemy viewing the character
                (null, unit.coordinates, null, true, GetMostProtectiveCoverTypeIn(coversAround));
        
        List<CoverInfo> coverInfos = new();

        foreach (U__Unit testedUnit in enemiesInView)
            coverInfos.Add(GetCoverStateFrom(unit.coordinates, coversAround, testedUnit));

        if(coverInfos.Any(testedInfo => !testedInfo.GetIsCovered()))
            return new CoverInfo // Uncovered but a cover closed
                (null, unit.coordinates, null, false, GetMostProtectiveCoverTypeIn(coversAround));
        
        return coverInfos // Covered
            .OrderByDescending(testedInfo => testedInfo.GetIsCovered())
            .ThenBy(testedInfo => testedInfo.GetCoverType().GetCoverProtectionPercent())
            .FirstOrDefault();
    }

    /// <summary>
    /// Returns the Cover info of the unit when another targets it.
    /// </summary>
    /// <param name="aimingUnit"></param>
    /// <returns></returns>
    public CoverInfo GetCoverStateFrom(U__Unit aimingUnit)
    {
        List<Cover> coversAround = _board.GetAdjacentCoversAt(unit.coordinates, GetCoveringTileTypes());
        return GetCoverStateFrom(unit.coordinates, coversAround, aimingUnit);
    }
    
    /// <summary>
    /// Returns the value of protection of the covering position from an unit.
    /// </summary>
    /// <param name="aimingUnit"></param>
    /// <returns></returns>
    public int GetCoverProtectionValueFrom(U__Unit aimingUnit)
    {
        List<Cover> adjacentCoversList = _board.GetAdjacentCoversAt(unit.coordinates, GetCoveringTileTypes());

        if (adjacentCoversList.Count == 0)
            return 0; // No cover around

        CoverInfo betterCover = GetCoverStateFrom(unit.coordinates, adjacentCoversList, aimingUnit);

        if (!betterCover.GetIsCovered())
            return 0; // Nothing covers
        
        return GetCoverStateFrom(unit.coordinates, adjacentCoversList, aimingUnit)
            .GetCoverType()
            .GetCoverProtectionPercent();
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the covering types of the unit.
    /// </summary>
    /// <returns></returns>
    private List<TileType> GetCoveringTileTypes() => coverTypes
        .SelectMany(currentTypeOfCover => currentTypeOfCover.GetCoveringTileTypes())
        .ToList();
    
    /// <summary>
    /// Returns the cover info at a coordinate, with a cover and an aiming unit.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="cover"></param>
    /// <param name="aimingUnit"></param>
    /// <returns></returns>
    private CoverInfo GetCoverInfoFrom(Coordinates coveredCoordinates, Cover cover, U__Unit aimingUnit) => new(
            cover,
            coveredCoordinates,
            cover.GetEdgeElement()
                ? cover.GetEdgeElement().GetOtherSideCoordinates(coveredCoordinates)
                : cover.GetTile().coordinates,
            IsCoverProtectingFrom(coveredCoordinates, cover, aimingUnit),
            GetCoveringTypeOf(cover.GetCoveringTileType()));

    /// <summary>
    /// Returns the most protecting cover in a list.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="coverList"></param>
    /// <param name="aimingUnit"></param>
    /// <returns></returns>
    private CoverInfo GetCoverStateFrom(Coordinates coveredCoordinates, List<Cover> coverList, U__Unit aimingUnit) =>
        coverList
            .Select(testedCover => GetCoverInfoFrom(coveredCoordinates, testedCover, aimingUnit))
            .OrderByDescending(testedInfo => testedInfo.GetIsCovered())
            .ThenBy(testedInfo => testedInfo.GetCoverType().GetCoverProtectionPercent())
            .FirstOrDefault();

    /// <summary>
    /// Returns true if the cover is protecting from an aiming unit, else returns false.
    /// </summary>
    /// <param name="coveredCoordinates"></param>
    /// <param name="cover"></param>
    /// <param name="aimingUnit"></param>
    /// <returns></returns>
    private bool IsCoverProtectingFrom(Coordinates coveredCoordinates, Cover cover, U__Unit aimingUnit)
    {
        Vector2 coverPosition = cover.GetWorldCoordinatesAsVector2();
        Vector2 coveredPosition = coveredCoordinates.ToVector2();
        Vector2 viewerPosition = aimingUnit.coordinates.ToVector2();
        Vector2 coverForward = -(coverPosition-coveredPosition);
        float angle = Vector2.Angle(coverForward, coverPosition - viewerPosition);
        return angle <= coveringAngle / 2;
    }

    /// <summary>
    /// Returns the cover type with the better protection in a list.
    /// </summary>
    /// <param name="covers"></param>
    /// <returns></returns>
    private CoverType GetMostProtectiveCoverTypeIn(List<Cover> covers) => covers
        .Select(testedCover => GetCoveringTypeOf(testedCover.GetCoveringTileType()))
        .OrderBy(testedCoverType => testedCoverType.GetCoverProtectionPercent())
        .FirstOrDefault();

    /// <summary>
    /// Returns the covering type corresponding at a type of tile.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    private CoverType GetCoveringTypeOf(TileType tileType) => coverTypes
        .FirstOrDefault(testedCoverType => testedCoverType.Contains(tileType));
}

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

