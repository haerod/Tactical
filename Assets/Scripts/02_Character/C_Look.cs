using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;
using UnityEngine.TextCore.Text;

public class C_Look : MonoBehaviour
{
    [SerializeField] private int range = 5;
    [SerializeField] private List<TileType> visualObstacles;
    public enum VisionType { SingleVision, GroupVision}
    public VisionType visionType = VisionType.GroupVision;
    
    [Header("REFERENCES")]
    
    [SerializeField] private C__Character c;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the view range of the character.
    /// </summary>
    /// <returns></returns>
    public int GetRange() => range;
    
    /// <summary>
    /// Returns all tiles in view depending on the rules.
    /// </summary>
    /// <returns></returns>
    public List<Tile> VisibleTiles()
    {
        List<Tile> toReturn = new List<Tile>();

        switch (visionType)
        {
            case VisionType.SingleVision:
                return GetVisibleTiles()
                    .ToList();
            case VisionType.GroupVision:
                _characters
                    .GetAlliesOf(c)
                    .ForEach(chara => toReturn.AddRange(chara.look.GetVisibleTiles()));

                toReturn = toReturn
                    .Distinct()
                    .ToList();
                break;
        }

        return toReturn;
    }
    
    /// <summary>
    /// Returns the characters visible in fog of war.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> CharactersVisibleInFog() => _characters
        .GetUnitsList()
            .Where(chara =>
            {
                switch (_rules.visibleInFogOfWar)
                {
                    case M_Rules.VisibleInFogOfWar.Everybody:
                        return true;
                    case M_Rules.VisibleInFogOfWar.Allies:
                        return VisibleTiles().Contains(chara.tile) || c.team.IsAllyOf(_characters.current);
                    case M_Rules.VisibleInFogOfWar.InView:
                        return VisibleTiles().Contains(chara.tile);
                    default:
                        return false;
                }
            })
            .ToList();
    
    /// <summary>
    /// Returns the enemies visible in fog of war.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> EnemiesVisibleInFog() => CharactersVisibleInFog()
        .Where(testedCharacter => c.team.IsEnemyOf(testedCharacter))
        .ToList();
    
    /// <summary>
    /// Return true if the target character is visible.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CanSee(C__Character target) =>
        CharactersVisibleInFog().Contains(target) && HasSightOn(target.tile);
    
    /// <summary>
    /// Returns the tiles in the line of sight of the character on a tile.
    /// </summary>
    /// <param name="targetCoordinates"></param>
    /// <returns></returns>
    public List<Tile> GetTilesOfLineOfSightOn(Coordinates targetCoordinates) =>  
        LineOfSight.GetLineOfSight(c.coordinates, targetCoordinates)
            .Select(coordinates => _board.GetTileAtCoordinates(coordinates))
            .ToList();
    
    /// <summary>
    /// Returns the closest enemy on sight.
    /// </summary>
    /// <returns></returns>
    public C__Character ClosestEnemyOnSight() => _characters.GetUnitsList()
            .Where(o => o != c) // remove emitter
            .Where(o => o.team.IsEnemyOf(c)) // get only enemies
            .Where(o => HasSightOn(o.tile)) // get all enemies on sight
            .OrderBy(o => GetTilesOfLineOfSightOn(o.tile.coordinates).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns true if the character has obstacles on its line of sight.
    /// </summary>
    /// <param name="lineOfSight"></param>
    /// <returns></returns>
    private bool AreObstaclesOn(List<Coordinates> lineOfSight) => lineOfSight
        .Select(coordinates => _board.GetTileAtCoordinates(coordinates))
        .Where(coordinates => coordinates)
        .Any(t => visualObstacles.Contains(t.type));

    /// <summary>
    /// Returns all tiles in view of THIS character.
    /// </summary>
    /// <returns></returns>
    private List<Tile> GetVisibleTiles()
    {
        List<Tile> toReturn =
            _board.GetTilesAround(c.tile, range, false)
                .Where(HasSightOn)
                .Where(o => !visualObstacles.Contains(o.type))
                .ToList();

        toReturn.Add(c.tile);

        return toReturn;
    }
    
    /// <summary>
    /// Returns true if the character has the tile on sight, not including Fog of war.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool HasSightOn(Tile tile)
    {
        List<Coordinates> los = LineOfSight.GetLineOfSight(c.coordinates, tile.coordinates);

        if(los == null)
            return true; // Nothing on LoS
        if (los.Count + 1 > range)
            return false; // Out of view range
        if (AreObstaclesOn(los))
            return false; // Obstacles

        return true; // Has sight on target
    }
    
}
