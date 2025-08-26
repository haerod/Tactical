using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class A_Move : A__Action
{
    [Header("PARAMETERS")]

    public int movementRange = 6;
    public bool useDiagonalMovement = true;
    public enum PassThrough {Everybody, Nobody, AlliesOnly}
    public PassThrough canPassThrough = PassThrough.Nobody;
    public List<TileType> walkableTiles;
    
    [Header("FOG OF WAR")]
    public bool movementInFogOfWarAllowed = false;

    [Header("ANIMATION")]
    [Range(0,10f)]
    [SerializeField] private float speed = 6;
    [Range(0,1f)]
    [SerializeField] private float animSpeed = .5f;

    private List<Tile> currentPath;
    private int index;
    private Vector3 destination;

    public static event EventHandler OnAnyMovementStart;
    public static event EventHandler OnAnyMovementEnd;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        if(c.cover.AreCoversAround())
            c.anim.EnterCrouch();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the move area depending on the rules.
    /// </summary>
    public List<Tile> MovementArea()
    {
        if (!c.CanPlay())
            return new List<Tile>();

        // Fog of war disabled
        if (!_rules.enableFogOfWar)
            return _board
                .GetTilesAround(c.tile, movementRange,useDiagonalMovement)
                .Except(Blockers())
                .Where(t => !t.IsOccupiedByCharacter())
                .ToList();

        // Fog of war && can walk in fog of war
        if (movementInFogOfWarAllowed)
            return _board
                .GetTilesAround(c.tile, movementRange, useDiagonalMovement)
                .Except(Blockers())
                .Where(t => !t.IsOccupiedByCharacter() || (t.IsOccupiedByCharacter() && !c.look.HasSightOn(t)))
                .ToList();

        // Fog of war && can not walk in fog of war
        return _board
            .GetTilesAround(c.tile, movementRange, useDiagonalMovement)
            .Except(Blockers())
            .Intersect(c.look.VisibleTiles())
            .Where(t => !t.IsOccupiedByCharacter())
            .ToList();
    }

    /// <summary>
    /// Returns the characters which block the movement (depending on the rules).
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetTraversableCharacterTiles()
    {
        List<Tile> toReturn = new List<Tile>();

        if (_rules.enableFogOfWar)
            toReturn.AddRange(_characters.GetCharacterList()
                .Where(chara => IsBlockingPath(chara))
                .Intersect(c.look.CharactersVisibleInFog())
                .Select(chara => chara.tile)
                .ToList());

        toReturn.AddRange(_characters.GetCharacterList()
            .Where(chara => IsBlockingPath(chara))
            .Select(chara => chara.tile)
            .ToList());

        return toReturn;
    }

    /// <summary>
    /// Orients this object to another position, except on Y axis. Possibility to add an offset (euler angles).
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

        c.unitUI.OrientToCamera();
    }

    /// <summary>
    /// Returns true if the character can walk on this type of tile.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public bool CanWalkOn(TileType tileType) => walkableTiles.Contains(tileType);

    /// <summary>
    /// Returns true if the character can walk at the given coordinates.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public bool CanWalkAt(Coordinates coordinates)
    {
        Tile tileAtCoordinates = _board.GetTileAtCoordinates(coordinates);
        
        if(!tileAtCoordinates)
            return false; // No tile
        
        return CanWalkOn(tileAtCoordinates.type);
    }
    
    /// <summary>
    /// Returns true if the character can move to the tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CanMoveTo(Tile tile)
    {
        if (!c.CanPlay()) 
            return false; // Can't play

        bool tileInFog =_rules.enableFogOfWar && !c.look.VisibleTiles().Contains(tile);
        if (tileInFog && !c.move.movementInFogOfWarAllowed) 
            return false; // Tile in fog

        List<Tile> path = Pathfinding.GetPath(
            c.tile,
            tile,
            Pathfinding.TileInclusion.WithEnd,
            new MovementRules(walkableTiles, GetTraversableCharacterTiles(), useDiagonalMovement));
        
        if (path.Count == 0) 
            return false; // No path
        if (path.Count > movementRange) 
            return false; // Out range

        return true; // In range
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts the movement on a path, with and action on end of this path.
    /// </summary>
    /// <param name="path"></param>
    private void MoveOnPath(List<Tile> path)
    {
        OnAnyMovementStart?.Invoke(this, EventArgs.Empty);
        
        c.SetCanPlayValue(false);

        currentPath = path.ToList();

        index = 0;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);

        c.anim.SetSpeed(animSpeed); // Blend tree anim speed
        c.anim.ExitCrouch();

        _input.SetActiveClick(false);

        c.HideTilesFeedbacks();

        StartCoroutine(MoveToDestination());
    }
    
    /// <summary>
    /// Returns tiles where are blockers
    /// </summary>
    /// <returns></returns>
    private List<Tile> Blockers()
    {
        List<Tile> toReturn = new List<Tile>();

        // Add not walkableTiles
        toReturn.AddRange(_board.GetTilesAround(c.tile, movementRange, useDiagonalMovement)
            .Where(t => !CanWalkOn(t.type))
            .ToList());

        // Add characters
        toReturn.AddRange(GetTraversableCharacterTiles());

        return toReturn;
    }
    
    /// <summary>
    /// Moves the object to a destination and executes an action in the end.
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
                c.coordinates.x = currentPath[index].coordinates.x;
                c.coordinates.y = currentPath[index].coordinates.y;

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
    /// Happens when a character move to the next tile.
    /// </summary>
    private void NextTile()
    {
        index++;
        destination = currentPath[index].transform.position;
        OrientTo(currentPath[index].transform.position);
    }

    /// <summary>
    /// Returns true if is the last tile of the movement.
    /// </summary>
    /// <returns></returns>
    private bool IsTheLastTile() => index + 1 >= currentPath.Count || c.movementRange <= 0;

    /// <summary>
    /// Happens in the end of the movement.
    /// </summary>
    private void EndMove()
    {
        OnAnyMovementEnd?.Invoke(this, EventArgs.Empty);
        c.anim.SetSpeed(0f);
        
        if(c.cover.AreCoversAround())
            c.anim.EnterCrouch();
        
        _input.SetActiveClick();

        if (_characters.current.behavior.playable)
            c.EnableTilesFeedbacks();

        Turns.EndTurn();
    }
    
    /// <summary>
    /// Returns true if the character blocks the path, depending on the capacity to pass through other characters.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private bool IsBlockingPath(C__Character character)
    {
        if (canPassThrough == PassThrough.Nobody)
            return true;
        if (canPassThrough == PassThrough.AlliesOnly && c.team.IsAllyOf(character))
            return false;

        return false;
    }
    
    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================

    protected override void OnHoverTile(Tile hoveredTile)
    {
        OrientTo(hoveredTile.transform.position);
    }

    protected override void OnClickTile(Tile clickedTile)
    {
        if (!CanMoveTo(clickedTile)) 
            return; // Tile out of movement range

        MoveOnPath(Pathfinding.GetPath(
            c.tile,
            clickedTile,
            Pathfinding.TileInclusion.WithEnd,
            new MovementRules(walkableTiles, GetTraversableCharacterTiles(), useDiagonalMovement)));
    }
}
