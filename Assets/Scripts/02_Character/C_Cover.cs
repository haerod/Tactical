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
    [SerializeField] private List<TypeOfCover> coveringTypes;
    [SerializeField] private float coveringAngle = 180;
    
    [Header("REFERENCES")]
    [SerializeField] private C__Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

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
    
    public List<TileType> GetCoveringTypes() => coveringTypes
        .SelectMany(currentTypeOfCover => currentTypeOfCover.GetTileTypes())
        .ToList();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
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

    private CoverType GetCoveringTypeOf(TileType tileType)
    {
        foreach (TypeOfCover testedTypeOfCover in coveringTypes)
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

    public bool IsSameInfoThan(CoverInfo other) => other.cover == cover && other.coveredCoordinates == coveredCoordinates;
    
    public bool GetIsCovered() => isCovered;
    
    public Coordinates GetCoveredCoordinates() => coveredCoordinates;
    
    public Coordinates GetCoverFeedbackCoordinates() => coverFeedbackCoordinates;
    
    public GameObject GetCover() => cover;
    
    public CoverType GetCoverType() => coverType;
}

[Serializable]
public class TypeOfCover
{
    [SerializeField] private CoverType coverType;
    [SerializeField] private List<TileType> tileTypes;
    
    public bool Contains(TileType tileType) => tileTypes.Contains(tileType);
    
    public List<TileType> GetTileTypes() => tileTypes;
    
    public CoverType GetCoverType() => coverType;
}

