using System.Collections.Generic;

public class MovementRules
{
    public readonly List<TileType> allowedTileTypes;
    public readonly List<Tile> blockingCharacterTiles;
    public readonly bool useDiagonals;
    
    public MovementRules(List<TileType> allowedTileTypes, List<Tile> blockingCharacterTiles, bool  useDiagonals)
    {
        this.allowedTileTypes = allowedTileTypes;
        this.blockingCharacterTiles = blockingCharacterTiles;
        this.useDiagonals = useDiagonals;
    }
}