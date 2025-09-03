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

    [Header("ANIMATION")]
    [Range(0,10f)]
    [SerializeField] private float speed = 6;
    [Range(0,1f)]
    [SerializeField] private float animSpeed = .5f;
    
    private List<Tile> currentPath;
    private int index;
    private Vector3 destination;
    
    public List<Tile> movementArea => GetMovementArea().ToList();
    private List<Tile> currentMovementArea = new List<Tile>();
    private bool anythingChangedOnBoard = true;
    
    public static EventHandler OnAnyMovementStart;
    public event EventHandler OnMovementStart;
    public event EventHandler OnMovementEnd;
    public event EventHandler<Tile> OnUnitEnterTile;
    public event EventHandler<List<Tile>> OnMovableTileEnter;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        A_Move.OnAnyMovementStart += Move_OnAnyMovementStart;
        C_Health.OnAnyDeath += Health_OnAnyDeath;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

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
    /// Returns true if the unit can walk on this type of tile.
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
        
        return tileAtCoordinates && CanWalkOn(tileAtCoordinates.type);
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

        if (_rules.IsFogOfWar() && !c.look.visibleTiles.Contains(tile)) 
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
    /// Returns the move area depending on the rules.
    /// </summary>
    private List<Tile> GetMovementArea()
    {
        if (!anythingChangedOnBoard) 
            return currentMovementArea.ToList(); // Nothing change

        currentMovementArea.Clear();
        List<Tile> tilesToTest = new();
        List<Tile> tilesToTestNextTurn = new();
        List<Tile> alreadyTested = new();
        
        tilesToTest.Add(c.tile);
        
        for (int i = 0; i < movementRange; i++)
        {
            foreach (Tile testedTile in tilesToTest)
            {
                tilesToTestNextTurn.AddRange(_board
                    .GetTilesAround(testedTile, 1, useDiagonalMovement)
                    .Where(tile => Pathfinding.IsDirectionWalkable(tile, testedTile, new MovementRules(walkableTiles, GetTraversableCharacterTiles(), useDiagonalMovement)))
                    .Except(alreadyTested)
                    .Except(tilesToTest)
                    .ToList());
            }
            
            alreadyTested.AddRange(tilesToTest);
            currentMovementArea.AddRange(tilesToTestNextTurn);
            tilesToTest = tilesToTestNextTurn.ToList();
            tilesToTestNextTurn.Clear();
        }

        anythingChangedOnBoard = false;
        
        currentMovementArea = currentMovementArea
            .Intersect(c.look.visibleTiles)
            .ToList();

        return currentMovementArea.ToList();
    }
    
    /// <summary>
    /// Starts the movement on a path, with and action on end of this path.
    /// </summary>
    /// <param name="path"></param>
    private void MoveOnPath(List<Tile> path)
    {
        OnMovementStart?.Invoke(this, EventArgs.Empty);
        OnAnyMovementStart?.Invoke(this, EventArgs.Empty);
        
        c.SetCanPlayValue(false);

        currentPath = path.ToList();

        index = 0;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);

        c.anim.SetSpeed(animSpeed); // Blend tree anim speed
        c.anim.ExitCrouch();

        _input.SetActivePlayerInput(false);
        
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
                OnUnitEnterTile?.Invoke(this, currentPath[index]);

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
    private bool IsTheLastTile()
    {
        if(index + 1 >= currentPath.Count)
            return true; // Last tile
        if(IsBlockingPath(currentPath[index + 1].character))
            return true; // Blocked by a character
        if(c.movementRange <= 0)
            return true; // No movement range
        
        return false;
    }

    /// <summary>
    /// Happens in the end of the movement.
    /// </summary>
    private void EndMove()
    {
        c.anim.SetSpeed(0f);
        
        if(c.cover.AreCoversAround())
            c.anim.EnterCrouch();
        
        Wait(0.2f, () =>
        {
            OnMovementEnd?.Invoke(this, EventArgs.Empty);
            _characters.EndCurrentUnitTurn();
        });
    }
    
    /// <summary>
    /// Returns the characters which block the movement (depending on the rules).
    /// </summary>
    /// <returns></returns>
    private List<Tile> GetTraversableCharacterTiles()
    {
        List<Tile> toReturn = new List<Tile>();

        if (_rules.IsFogOfWar())
            toReturn.AddRange(_characters.GetUnitsList()
                .Where(chara => IsBlockingPath(chara))
                .Intersect(c.look.CharactersVisibleInFog())
                .Select(chara => chara.tile)
                .ToList());

        toReturn.AddRange(_characters.GetUnitsList()
            .Where(chara => IsBlockingPath(chara))
            .Select(chara => chara.tile)
            .ToList());

        return toReturn;
    }
    
    /// <summary>
    /// Returns true if the character blocks the path, depending on the capacity to pass through other characters.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private bool IsBlockingPath(C__Character character)
    {
        if(!c.look.CharactersVisibleInFog().Contains(character))
            return false; // Invisible character
        
        switch (canPassThrough)
        {
            case PassThrough.Nobody:
                return true;
            case PassThrough.AlliesOnly when c.team.IsAllyOf(character):
                return false;
            case PassThrough.Everybody:
                return false;
            default:
                return false;
        }
    }
    
    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================

    protected override void OnHoverTile(Tile hoveredTile)
    {
        if(!c.CanPlay())
            return; // Can't play
        
        OrientTo(hoveredTile.transform.position);
        
        List<Tile> currentPathfinding = Pathfinding.GetPath(
            c.tile,
            hoveredTile,
            Pathfinding.TileInclusion.WithStartAndEnd,
            new MovementRules(
                walkableTiles, 
                GetTraversableCharacterTiles(), 
                useDiagonalMovement));
        
        if (currentPathfinding.Count == 0)
            return; // No path
        
        OnMovableTileEnter?.Invoke(this, currentPathfinding);
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
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Health_OnAnyDeath(object sender, EventArgs e)
    {
        anythingChangedOnBoard = true;
    }

    private void Move_OnAnyMovementStart(object sender, EventArgs e)
    {
        anythingChangedOnBoard = true;
    }
}
