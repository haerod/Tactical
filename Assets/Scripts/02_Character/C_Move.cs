using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;
using UnityEditor;

public class C_Move : MonoBehaviour
{
    public int movementRange = 6;
    public List<TileType> walkableTiles;

    [Header("FOG OF WAR")]
    public bool movementInFogOfWarAllowed = false;

    [Header("ANIMATION")]
    [Range(0,10f)]
    [SerializeField] private float speed = 6;
    [Range(0,1f)]
    [SerializeField] private float animSpeed = .5f;

    [Header("COORDINATES (debug)")]
    public int x = 1; // Let it serialized to set it dirty.
    public int y = 1; // Let it serialized to set it dirty.

    [Header("REFERENCES")]

    [SerializeField] private Animator anim = null;
    [SerializeField] private C__Character c  = null;

    private List<Tile> currentPath = null;
    private int index = 0;
    private Vector3 destination;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the move area depending the rules.
    /// </summary>
    public List<Tile> MovementArea()
    {
        if (!c.CanPlay())
            return new List<Tile>();

        // Fog of war disabled
        if (!_rules.enableFogOfWar)
            return _board
                .GetTilesAround(c.tile, movementRange, _rules.useDiagonals)
                .Except(Blockers())
                .Where(t => !t.IsOccupiedByCharacter())
                .ToList();

        // Fog of war && can walk in fog of war
        if (movementInFogOfWarAllowed)
            return _board
                .GetTilesAround(c.tile, movementRange, _rules.useDiagonals)
                .Except(Blockers())
                .Where(t => !t.IsOccupiedByCharacter() || (t.IsOccupiedByCharacter() && !c.look.HasSightOn(t)))
                .ToList();

        // Fog of war && can not walk in fog of war
        return _board
            .GetTilesAround(c.tile, movementRange, _rules.useDiagonals)
            .Except(Blockers())
            .Intersect(c.look.VisibleTiles())
            .Where(t => !t.IsOccupiedByCharacter())
            .ToList();
    }

    /// <summary>
    /// Return the characters which block the movement (depending the rules).
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetTraversableCharacterTiles()
    {
        List<Tile> toReturn = new List<Tile>();

        if (_rules.enableFogOfWar)
            toReturn.AddRange(_characters.GetCharacterList()
                .Where(chara => IsBlockingPath(chara))
                .Intersect(c.look.CharactersInView())
                .Select(chara => chara.tile)
                .ToList());

        toReturn.AddRange(_characters.GetCharacterList()
            .Where(chara => IsBlockingPath(chara))
            .Select(chara => chara.tile)
            .ToList());

        return toReturn;
    }

    /// <summary>
    /// Start the movement on a path, with and action on end of this path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="OnEnd"></param>
    public void MoveOnPath(List<Tile> path)
    {
        c.SetCanPlayValue(false);

        currentPath = path.ToList();

        index = 0;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);

        anim.SetFloat("speed", animSpeed); // Blend tree anim speed

        _input.SetActiveClick(false);
        _ui.SetActivePlayerUI_Action(false);

        c.HideTilesFeedbacks();

        StartCoroutine(MoveToDestination());
    }

    /// <summary>
    /// Orient this object to another position, except on Y axis. Possibility to add an offset (euler angles).
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="offset"></param>
    public void OrientTo(Vector3 targetPosition, float offset = 0)
    {
        Vector3 lookPos = targetPosition - transform.position;
        lookPos.y = 0;
        Quaternion endRotation = Quaternion.Euler(Vector3.zero);
        if (lookPos != Vector3.zero)
            endRotation = Quaternion.LookRotation(lookPos);
        endRotation *= Quaternion.Euler(new Vector3(0, offset, 0));
        c.transform.rotation = endRotation;

        c.healthBar.GetComponentInParent<UI_OrientToCamera>().OrientToCamera();
    }

    /// <summary>
    /// Return true if the character can walk on this type of tile.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public bool CanWalkOn(TileType tileType) => walkableTiles.Contains(tileType);

    /// <summary>
    /// Return true if the character can move to the tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CanMoveTo(Tile tile)
    {
        if (!c.CanPlay()) return false; // Can't play

        bool tileInFog =_rules.enableFogOfWar && !c.look.VisibleTiles().Contains(tile);
        if (tileInFog && !c.move.movementInFogOfWarAllowed) return false; // Tile in fog

        List<Tile> path = Pathfinding.Pathfind(
            c.tile,
            tile,
            Pathfinding.TileInclusion.WithEnd,
            new MovementRules(walkableTiles, GetTraversableCharacterTiles()));
        if (path.Count == 0) return false; // No path
        if (path.Count > movementRange) return false; // Out range

        return true; // In range
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns tiles where are blockers
    /// </summary>
    /// <returns></returns>
    private List<Tile> Blockers()
    {
        List<Tile> toReturn = new List<Tile>();

        // Add not walkableTiles
        toReturn.AddRange(_board.GetTilesAround(c.tile, movementRange, _rules.useDiagonals)
            .Where(t => !CanWalkOn(t.type))
            .ToList());

        // Add characters
        toReturn.AddRange(GetTraversableCharacterTiles());

        return toReturn;
    }
    
    /// <summary>
    /// Move the object to a desination and execute an action in the end.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToDestination()
    {
        while (true)
        {
            _camera.ResetPosition();

            if (c.transform.position != destination) // Move
            {
                c.transform.position = Vector3.MoveTowards(c.transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
            else // On tile enter
            {
                OnTileEnter();

                x = currentPath[index].x;
                y = currentPath[index].y;

                if (IsTheLastTile()) 
                {
                    EndMove();
                    yield break; // EXIT : End path
                }
                else
                {
                    NextTile();
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// Happened when a character move to the next tile.
    /// </summary>
    private void NextTile()
    {
        index++;
        destination = currentPath[index].transform.position;
        OrientTo(currentPath[index].transform.position);
    }

    /// <summary>
    /// Return true if is the last tile of the movement.
    /// </summary>
    /// <returns></returns>
    private bool IsTheLastTile() => index + 1 >= currentPath.Count || c.movementRange <= 0;

    /// <summary>
    /// Happend in the end of the movement.
    /// </summary>
    private void EndMove()
    {
        anim.SetFloat("speed", 0f);
        _input.SetActiveClick();
        _ui.SetActivePlayerUI_Action(true);

        if (_characters.current.behavior.playable)
        {
            c.EnableTilesFeedbacks();
        }

        Turns.EndTurn();
    }

    /// <summary>
    /// Played when the character enters a tile.
    /// </summary>
    private void OnTileEnter()
    {

    }

    /// <summary>
    /// Return true if the character blocks the path, depending the capacity to pass through other characters.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private bool IsBlockingPath(C__Character character)
    {
        if (_rules.canPassThrough == M_Rules.PassThrough.Nobody)
            return true;
        else if (_rules.canPassThrough == M_Rules.PassThrough.AlliesOnly && c.infos.IsAlliedTo(character))
            return false;

        return false;

    }
}
