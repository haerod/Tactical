using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;

public class C_Look : MonoBehaviour
{
    [SerializeField] private int range = 5;
    [SerializeField] private List<TileType> visualObstacles = null;
    public enum VisionType { SingleVision, GroupVision}
    public VisionType visionType = VisionType.GroupVision;

    [Header("REFERENCES")]
    [SerializeField] private C__Character c = null;

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

        if (visionType == VisionType.SingleVision)
            return GetVisibleTiles()
                .ToList();

        else if(visionType == VisionType.GroupVision)
        {
            _characters
                .GetTeamMembers(c)
                .ForEach(chara => toReturn.AddRange(chara.look.GetVisibleTiles()));

            toReturn = toReturn
                .Distinct()
                .ToList();
        }

        return toReturn;
    }

    /// <summary>
    /// Returns the other characters visible by this character, depending on the rules.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> CharactersInView()
    {
        return _characters.GetCharacterList()
            .Where(chara =>
            {
                if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Everybody)
                    return true;
                else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Allies)
                    return VisibleTiles().Contains(chara.tile) || c.team == _characters.current.team;
                else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.InView)
                    return VisibleTiles().Contains(c.tile);
                else
                    Debug.LogError("No rule, please add one here.");

                return false;
            })
            .ToList();
    }
    
    /// <summary>
    /// Returns all the enemies in view.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> EnemiesInView() => CharactersInView()
        .Where(civ => civ.team != c.team)
        .ToList();

    /// <summary>
    /// Returns true if the character has the tile on sight.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool HasSightOn(Tile tile)
    {
        List<Tile> los = GetTilesOfLineOfSightOn(tile);

        if (AreObstaclesOn(los))
            return false; // Obstacles
        if (los.Count + 1 > range)
            return false; // Out of view range

        return true; // Has sight on target
    }
    
    /// <summary>
    /// Returns the tiles in the line of sight of the character on a tile.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    public List<Tile> GetTilesOfLineOfSightOn(Tile targetTile) => GetCoordinatesOfLineOfSightOn(targetTile)
            .Select(coordinates => _board.GetTileAtCoordinates(coordinates))
            .ToList();

    /// <summary>
    /// Returns the coordinates in the line of sight of the character on a tile.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    public List<Coordinates> GetCoordinatesOfLineOfSightOn(Tile targetTile)=> Pathfinding.LineOfSight(c.tile, targetTile)
        .ToList();
    
    /// <summary>
    /// Returns the coordinates in the line of sight of the character on a tile.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    public List<Coordinates> GetCoordinatesOfFullLineOfSightOn(Tile targetTile)=> Pathfinding.LineOfSight(c.tile, targetTile, Pathfinding.TileInclusion.WithStartAndEnd)
        .ToList();
    
    /// <summary>
    /// Returns the closest enemy on sight.
    /// </summary>
    /// <returns></returns>
    public C__Character ClosestEnemyOnSight() => _characters.GetCharacterList()
            .Where(o => o != c) // remove emitter
            .Where(o => o.Team() != c.Team()) // remove allies
            .Where(o => HasSightOn(o.tile)) // get all enemies on sight
            .OrderBy(o => GetTilesOfLineOfSightOn(o.tile).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns true if the character has obstacles on its line of sight.
    /// </summary>
    /// <param name="lineOfSight"></param>
    /// <returns></returns>
    private bool AreObstaclesOn(List<Tile> lineOfSight)
    {
        if (Utils.IsVoidList(lineOfSight)) return false;

        foreach (Tile t in lineOfSight)
        {
            if (!t) continue; // Nothing
            if (visualObstacles.Contains(t.type)) return true; // Line of sight blocker

            // Character
            C__Character chara = t.Character();
            if (!chara) continue; // There is no character

            if (_rules.canSeeAndShotThrough == M_Rules.SeeAnShotThrough.Nobody)
            {
                if (!chara.health.IsDead()) return true; // Other character
            }
            else if (_rules.canSeeAndShotThrough == M_Rules.SeeAnShotThrough.AlliesOnly)
            {
                // Are obstacle if enemy && alive
                if ((chara.infos.team != c.infos.team) && (!chara.health.IsDead())) return true; // Enemy
            }
        }

        return false; // No obstacle
    }

    /// <summary>
    /// Returns all tiles in view of THIS character.
    /// </summary>
    /// <returns></returns>
    private List<Tile> GetVisibleTiles()
    {
        List<Tile> toReturn =
            _board.GetTilesAround(c.tile, range, false)
                .Where(o => HasSightOn(o))
                .Where(o => !visualObstacles.Contains(o.type))
                .ToList();

        toReturn.Add(c.tile);

        return toReturn;
    }
}
