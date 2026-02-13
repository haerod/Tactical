using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class A_Move : A__Action
{
    [Header("- PARAMETERS -")][Space]

    [SerializeField] private int _movementRange = 4;
    public int movementRange => _movementRange;
    [SerializeField] private bool useDiagonalMovement = true;
    private enum PassThrough {Everybody, Nobody, AlliesOnly}
    [SerializeField] private PassThrough canPassThrough = PassThrough.Nobody;
    [SerializeField] private List<TileType> walkableTiles;
    [Range(0,10f)]
    [SerializeField] private float speed = 6;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private MoveOnBoard moveOnBoard;
    [SerializeField] private Transform rotationTarget;
    
    public List<Tile> movementArea => GetMovementArea().ToList();
    
    private List<Tile> currentMovementArea = new();
    private bool anythingChangedOnBoard = true;
    
    public event EventHandler OnMovementStart;
    public event EventHandler OnMovementEnd;
    public event EventHandler OnMovableTileHovered;
    public event EventHandler<Tile> OnUnitEnterTile;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyMovementStart += Move_OnAnyMovementStart;
        GameEvents.OnAnyDeath += Health_OnAnyDeath;

        moveOnBoard.OnTileEnter += MoveOnBoard_OnTileEnter;
        moveOnBoard.OnMovementEnded += MoveOnBoard_OnMovementEnded;
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        moveOnBoard.OnTileEnter -= MoveOnBoard_OnTileEnter;
        moveOnBoard.OnMovementEnded -= MoveOnBoard_OnMovementEnded;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the movement range of the unit.
    /// </summary>
    /// <returns></returns>
    public int GetMovementRange() => _movementRange;
    
    /// <summary>
    /// Orients this object to another position, except on Y axis. Possibility to add an offset (euler angles).
    /// </summary>
    /// <param name="targetPosition"></param>
    public void OrientTo(Vector3 targetPosition)
    {
        Vector3 lookPos = targetPosition - transform.position;
        lookPos.y = 0;
        Quaternion endRotation = Quaternion.Euler(Vector3.zero);
        if (lookPos != Vector3.zero)
            endRotation = Quaternion.LookRotation(lookPos);
        rotationTarget.rotation = endRotation;

        unit.ui.OrientToCamera();
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
    /// Returns true if the character can move to the tile (including the movement range).
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CanMoveTo(Tile tile)
    {
        if (!unit.CanPlay()) 
            return false; // Can't play

        if (_level.isFogOfWar && !unit.look.visibleTiles.Contains(tile)) 
            return false; // Tile in fog
        
        return movementArea.Contains(tile);
    }
    
    /// <summary>
    /// Returns true if the unit can go to the given position (theoretical, without range or play infos).
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CanMoveTowards(Tile tile) => Pathfinding.GetPath(
                unit.tile,
                tile,
                Pathfinding.TileInclusion.WithEnd,
                new MovementRules(walkableTiles, GetTraversableUnitTiles(), useDiagonalMovement))
            .Count > 0;
    
    /// <summary>
    /// Get the furthest tile on the path to a given tile, depending on movement range.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    public Tile GetFurthestTileTowards(Tile targetTile)
    {
        List<Tile> path = Pathfinding.GetPath(
            unit.tile,
            targetTile,
            Pathfinding.TileInclusion.WithEnd,
            new MovementRules(walkableTiles, GetTraversableUnitTiles(), useDiagonalMovement))
            .ToList();
        
        if (path.Count == 0)
            return null;
        
        if(path.Count > _movementRange)
            return path[_movementRange-1];

        else
            return path.Last();
    }
    
    /// <summary>
    /// Starts the movement until a tile, with and action on end of this path.
    /// </summary>
    /// <param name="tile"></param>
    public void MoveTo(Tile tile) => MoveOn(Pathfinding.GetPath(
                unit.tile,
                tile,
                Pathfinding.TileInclusion.WithEnd,
                new MovementRules(walkableTiles, GetTraversableUnitTiles(), useDiagonalMovement))
            .ToList());
    
    /// <summary>
    /// Returns the path to the destination tile.
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    public List<Tile> GetPathTo(Tile destination) => Pathfinding.GetPath(
        unit.tile,
        destination,
        Pathfinding.TileInclusion.WithStartAndEnd,
        new MovementRules(
            walkableTiles, 
            GetTraversableUnitTiles(), 
            useDiagonalMovement));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Starts the movement on a path, with and action on end of this path.
    /// </summary>
    /// <param name="path"></param>
    private void MoveOn(List<Tile> path)
    {
        StartAction();
        OnMovementStart?.Invoke(this, EventArgs.Empty);
        GameEvents.InvokeOnAnyMovementStart(unit);
        moveOnBoard.Move(path.ToList(), speed);
    }
    
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
        
        tilesToTest.Add(unit.tile);
        
        for (int i = 0; i < _movementRange; i++)
        {
            foreach (Tile testedTile in tilesToTest)
            {
                tilesToTestNextTurn.AddRange(_board
                    .GetTilesAround(testedTile, 1, useDiagonalMovement)
                    .Where(tile => Pathfinding.IsDirectionWalkable(tile, testedTile, new MovementRules(walkableTiles, GetTraversableUnitTiles(), useDiagonalMovement)))
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
            .Intersect(unit.look.visibleTiles)
            .ToList();

        return currentMovementArea.ToList();
    }
    
    /// <summary>
    /// Returns tiles where are blockers
    /// </summary>
    /// <returns></returns>
    private List<Tile> Blockers()
    {
        List<Tile> toReturn = new();

        // Add not walkableTiles
        toReturn.AddRange(_board.GetTilesAround(unit.tile, _movementRange, useDiagonalMovement)
            .Where(t => !CanWalkOn(t.type))
            .ToList());

        // Add characters
        toReturn.AddRange(GetTraversableUnitTiles());

        return toReturn;
    }
    
    /// <summary>
    /// Happens in the end of the movement.
    /// </summary>
    private void EndMove()
    {
        OnMovementEnd?.Invoke(this, EventArgs.Empty);
        
        Wait(0.2f, () =>
        {
            
            EndAction();
        });
    }
    
    /// <summary>
    /// Returns the units which block the movement (depending on the rules).
    /// </summary>
    /// <returns></returns>
    private List<Tile> GetTraversableUnitTiles()
    {
        List<Tile> toReturn = new List<Tile>();

        if (_level.isFogOfWar)
            toReturn.AddRange(_units.units
                .Where(chara => IsBlockingPath(chara))
                .Intersect(unit.look.UnitsVisibleInFog())
                .Select(chara => chara.tile)
                .ToList());

        toReturn.AddRange(_units.units
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
    private bool IsBlockingPath(Unit character)
    {
        if(!unit.look.UnitsVisibleInFog().Contains(character))
            return false; // Invisible character
        
        switch (canPassThrough)
        {
            case PassThrough.Nobody:
                return true;
            case PassThrough.AlliesOnly when unit.team.IsAllyOf(character):
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
        if(!unit.CanPlay())
            return; // Can't play
        if(!unit.actionsHolder.CanUse(this))
            return; // Can't do this action
        
        OrientTo(hoveredTile.transform.position);
        
        if (!CanMoveTo(hoveredTile))
            return; // No path
        
        OnMovableTileHovered?.Invoke(this, EventArgs.Empty);
    }
    
    protected override void OnClickTile(Tile clickedTile)
    {
        if(!unit.actionsHolder.CanUse(this))
            return; // Can't do this action
        if (!CanMoveTo(clickedTile))
            return; // Tile out of movement range

        List<Tile> path = Pathfinding.GetPath(
            unit.tile,
            clickedTile,
            Pathfinding.TileInclusion.WithEnd,
            new MovementRules(walkableTiles, GetTraversableUnitTiles(), useDiagonalMovement))
            .Take(_movementRange)
            .ToList();
        
        MoveOn(path);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Health_OnAnyDeath(object sender, Unit deadUnit)
    {
        anythingChangedOnBoard = true;
    }
    
    private void Move_OnAnyMovementStart(object sender, Unit movingUnit)
    {
        anythingChangedOnBoard = true;
    }
    
    private void MoveOnBoard_OnTileEnter(object sender, Tile enteredTile)
    {
        unit.coordinates = enteredTile.coordinates;
        OnUnitEnterTile?.Invoke(this, enteredTile);
    }
    
    private void MoveOnBoard_OnMovementEnded(object sender, EventArgs e)
    {
        EndMove();
    }
}
