using System;

public static class InputEvents
{
    public static event EventHandler<Tile> OnTileEnter;
    public static event EventHandler<Tile> OnFreeTileEnter;
    public static event EventHandler<Tile> OnTileExit;
    public static event EventHandler<Tile> OnTileClick;
    public static event EventHandler OnNoTile;
    
    public static event EventHandler<Unit> OnUnitEnter;
    public static event EventHandler<Unit> OnUnitExit;
    public static event EventHandler <Unit> OnUnitClick;
    public static event EventHandler<Unit> OnEnemyEnter;
    public static event EventHandler<Unit> OnAllyEnter;
    public static event EventHandler OnCurrentUnitEnter;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public static void TileHovered(Tile tile) => TileHoveredEvents(tile);
    public static void TileUnhovered(Tile tile) => OnTileExit?.Invoke(null, tile);
    public static void TileClick(Tile tile) => OnTileClick?.Invoke(null, tile);
    public static void UnitUnhovered(Unit unit) => OnUnitExit?.Invoke(null, unit);
    public static void UnitHovered(Unit unit) => CharacterHoveredEvents(unit);
    public static void UnitClick(Unit unit)  => OnUnitClick?.Invoke(null, unit);
    public static void NothingHovered() => OnNoTile?.Invoke(null, EventArgs.Empty);

    public static void ClearEvents()
    {
        OnTileEnter = null;
        OnFreeTileEnter = null;
        OnTileExit = null;
        OnTileClick = null;
        OnNoTile = null;
        OnUnitEnter = null;
        OnUnitExit = null;
        OnUnitClick = null;
        OnEnemyEnter = null;
        OnAllyEnter = null;
        OnCurrentUnitEnter = null;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Events happening if the pointer overlaps a tile.
    /// </summary>
    /// <param name="tile"></param>
    private static void TileHoveredEvents(Tile tile)
    {
        OnTileEnter?.Invoke(null, tile);
        
        Unit currentUnit = M__Managers._units.current;
        if(!currentUnit)
            return; // No current unit
        
        if (!currentUnit.move.CanWalkAt(tile.coordinates) || !currentUnit.CanPlay()) 
            return; // Can't go on this tile or can't play
        
        bool pointedCharacterIsVisible = !M__Managers._level.isFogOfWar || currentUnit.look.visibleTiles.Contains(tile);

        if (tile.IsOccupiedByUnit() && pointedCharacterIsVisible)
        {
            CharacterHoveredEvents(tile.character);
            return; // Tile occupied by a unit
        }
        
        OnFreeTileEnter?.Invoke(null, tile);
    }
    
    /// <summary>
    /// Events happening if the pointer overlaps a occupied by a unit.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    private static void CharacterHoveredEvents(Unit hoveredCharacter)
    {
        Unit currentUnit = M__Managers._units.current;
        Unit currentTarget = hoveredCharacter;

        if (!currentUnit)
            return;
        
        OnUnitEnter?.Invoke(null, currentTarget);
        
        if (currentUnit.team.IsAllyOf(currentTarget)) // Unit or allie
        {
            if (currentUnit == currentTarget)
                OnCurrentUnitEnter?.Invoke(null, EventArgs.Empty);
            else
                OnAllyEnter?.Invoke(null, currentTarget);
        }
        else // Enemy
            OnEnemyEnter?.Invoke(null, currentTarget);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}