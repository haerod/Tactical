using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class U_Look : MonoBehaviour
{
    [SerializeField] private int range = 5;
    [SerializeField] private List<TileType> visualObstacles;
    public enum VisionType { SingleVision, GroupVision}
    public VisionType visionType = VisionType.GroupVision;
    
    [Header("REFERENCES")]
    
    [SerializeField] private U__Unit unit;

    public List<Tile> visibleTiles => GetVisibleTiles().ToList();
    
    private List<Tile> currentVisibleTiles = new();
    private bool anythingChangedOnBoard = true;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        unit.move.OnMovementStart += Move_OnMovementStart;
        unit.move.OnUnitEnterTile += Move_OnUnitEnterTile;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the view range of the unit.
    /// </summary>
    /// <returns></returns>
    public int GetRange() => range;
    
    /// <summary>
    /// Returns the units visible in fog of war.
    /// </summary>
    /// <returns></returns>
    public List<U__Unit> CharactersVisibleInFog() => _units
        .GetUnitsList()
            .Where(chara =>
            {
                switch (_rules.visibleInFogOfWar)
                {
                    case M_Rules.VisibleInFogOfWar.Everybody:
                        return true;
                    case M_Rules.VisibleInFogOfWar.Allies:
                        return visibleTiles.Contains(chara.tile) || unit.team.IsAllyOf(_units.current);
                    case M_Rules.VisibleInFogOfWar.InView:
                        return visibleTiles.Contains(chara.tile);
                    default:
                        return false;
                }
            })
            .ToList();
    
    /// <summary>
    /// Returns the enemies visible in fog of war.
    /// </summary>
    /// <returns></returns>
    public List<U__Unit> EnemiesVisibleInFog() => CharactersVisibleInFog()
        .Where(testedCharacter => unit.team.IsEnemyOf(testedCharacter))
        .ToList();
    
    /// <summary>
    /// Return true if the target unit is visible.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CanSee(U__Unit target) => CharactersVisibleInFog().Contains(target);
    
    /// <summary>
    /// Returns the tiles in the line of sight of the unit on a tile.
    /// </summary>
    /// <param name="targetCoordinates"></param>
    /// <returns></returns>
    public List<Tile> GetTilesOfLineOfSightOn(Coordinates targetCoordinates) =>  
        LineOfSight.GetLineOfSight(unit.coordinates, targetCoordinates)
            .Select(coordinates => _board.GetTileAtCoordinates(coordinates))
            .ToList();
    
    /// <summary>
    /// Returns the closest enemy on sight.
    /// </summary>
    /// <returns></returns>
    public U__Unit ClosestEnemyOnSight() => _units.GetUnitsList()
            .Where(o => o != unit) // remove emitter
            .Where(o => o.team.IsEnemyOf(unit)) // get only enemies
            .Where(o => HasSightOn(o.tile)) // get all enemies on sight
            .OrderBy(o => GetTilesOfLineOfSightOn(o.tile.coordinates).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the visible tiles of the unit, and calculate it if something change.
    /// </summary>
    /// <returns></returns>
    private List<Tile> GetVisibleTiles()
    {
        if (!anythingChangedOnBoard) 
            return currentVisibleTiles.ToList(); // Nothing change
        
        anythingChangedOnBoard = false;
        
        switch (visionType)
        {
            case VisionType.SingleVision:
                return CalculateVisibleTiles()
                    .ToList();
            case VisionType.GroupVision:
                _units
                    .GetAlliesOf(unit)
                    .ForEach(chara => currentVisibleTiles.AddRange(chara.look.CalculateVisibleTiles()));

                currentVisibleTiles = currentVisibleTiles
                    .Distinct()
                    .ToList();
                break;
        }
        
        return currentVisibleTiles.ToList();
    }
    
    /// <summary>
    /// Returns all tiles in view of THIS unit.
    /// </summary>
    /// <returns></returns>
    private List<Tile> CalculateVisibleTiles()
    {
        List<Tile> toReturn =
            _board.GetTilesAround(unit.tile, range, false)
                .Where(HasSightOn)
                .ToList();

        toReturn.Add(unit.tile);

        return toReturn;
    }
    
    /// <summary>
    /// Returns true if the unit has the tile on sight, not including Fog of war.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool HasSightOn(Tile tile)
    {
        List<Coordinates> los = LineOfSight.GetLineOfSight(unit.coordinates, tile.coordinates);

        if(los == null)
            return true; // Nothing on LoS
        if (los.Count + 1 > range)
            return false; // Out of view range
        
        bool areObstacleOnLineOfSight = los
            .Select(coordinates => _board.GetTileAtCoordinates(coordinates))
            .Where(coordinates => coordinates)
            .Any(t => visualObstacles.Contains(t.type));;
        
        return !areObstacleOnLineOfSight; // Obstacles
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Move_OnUnitEnterTile(object sender, Tile enteringTile)
    {
        anythingChangedOnBoard = true;
    }

    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        anythingChangedOnBoard = true;
    }
}
