using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

public class M_Board : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform charactersParent;

    [Header("DEBUG")]
    public TileGrid tileGrid;
    public static M_Board _instance;
    public static M_Board instance => _instance ?? FindFirstObjectByType<M_Board>();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        // Singleton
        if (!_instance)
            _instance = this;
        else
            Debug.LogError("There is more than one M_Board in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        
        tileGrid.Setup(transform.Cast<Transform>().Select(t => t.GetComponent<Tile>()).ToList());
    }
    
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the tile at (x,y) coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile GetTileAtCoordinates(int x, int y) => tileGrid[x, y];
    
    /// <summary>
    /// Returns the tile at given coordinates.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public Tile GetTileAtCoordinates(Coordinates coordinates) => tileGrid[coordinates.x, coordinates.y];
    
    /// <summary>
    /// Returns a tile with an offset if it exists, otherwise returns null.
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public Tile GetTileWithOffset(int xOffset, int yOffset, Tile tile) => GetTileAtCoordinates(tile.coordinates.x + xOffset, tile.coordinates.y + yOffset);

    /// <summary>
    /// Returns the tiles around a tile, with a radius.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="distance"></param>
    /// <param name="useDiagonals"></param>
    /// <returns></returns>
    public List<Tile> GetTilesAround(Tile startTile, int distance, bool useDiagonals)
    {
        List<Tile> toReturn = new List<Tile>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                Tile tile = _board.GetTileAtCoordinates(x + startTile.coordinates.x, y + startTile.coordinates.y);

                if (!tile)
                    continue; // No tile
                if (tile == startTile)
                    continue; // Start tile
                if (useDiagonals)
                {
                    toReturn.Add(tile);
                    continue; // Not using diagonals
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance > distance)
                    continue; // Not in diagonal

                toReturn.Add(tile);
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Returns the coordinates around coordinates, with a radius
    /// </summary>
    /// <param name="startCoordinates"></param>
    /// <param name="distance"></param>
    /// <param name="useDiagonals"></param>
    /// <returns></returns>
    public List<Coordinates> GetCoordinatesAround(Coordinates startCoordinates, int distance, bool useDiagonals)
    {
        List<Coordinates> coordinatesToReturn = new List<Coordinates>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                Coordinates currentCoordinates = new Coordinates(x + startCoordinates.x, y + startCoordinates.y);

                if (currentCoordinates == startCoordinates)
                    continue; // Start coordinates

                if (useDiagonals)
                {
                    coordinatesToReturn.Add(currentCoordinates);
                    continue; // Not using diagonals
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance > distance)
                    continue; // Not in diagonal

                coordinatesToReturn.Add(currentCoordinates);
            }
        }
        
        return coordinatesToReturn;
    }

    /// <summary>
    /// Returns all coordinates of the edges of a square of center x, y with a radius.
    /// A radius of 0 returns the coordinates x,y.
    /// </summary>
    /// <param name="xCenter"></param>
    /// <param name="yCenter"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<Coordinates> GetEmptySquareCoordinatesWithRadius(int xCenter, int yCenter, int radius)
    {
        List<Coordinates> toReturn = new List<Coordinates>();

        radius = Mathf.Abs(radius);
        int edgeLenght = 2 * radius + 1;

        if (radius == 0) // Distance = 0
        {
            toReturn.Add(new Coordinates(xCenter, yCenter));
            return toReturn;
        }

        for (int i = 0; i < edgeLenght; i++) // Top line
        {
            toReturn.Add(new Coordinates(xCenter - radius, yCenter - radius + i));
        }

        for (int i = 0; i < edgeLenght; i++) // Bot line
        {
            toReturn.Add(new Coordinates(xCenter + radius, yCenter - radius + i));
        }

        for (int i = 0; i < edgeLenght - 2; i++) // Left line
        {
            toReturn.Add(new Coordinates(xCenter - (radius - 1) + i, yCenter - radius));
        }

        for (int i = 0; i < edgeLenght - 2; i++) // Right line
        {
            toReturn.Add(new Coordinates(xCenter - (radius - 1) + i, yCenter + radius));
        }

        return toReturn;
    }

    /// <summary>
    /// Returns all the coordinates of square's points of centre x,y with a radius.
    /// A radius of 0 returns the coordinates x,y.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<Coordinates> GetFullSquareCoordinatesWithRadius(Coordinates centerCoordinates, int radius)
    {
        List<Coordinates> toReturn = new List<Coordinates>();

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                toReturn.Add(new Coordinates(centerCoordinates.x+i, centerCoordinates.y+j));
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Returns all the coordinates of a center x,y with and edge of edgeLength.
    /// </summary>
    /// <param name="xCenter"></param>
    /// <param name="yCenter"></param>
    /// <param name="edgeLength"></param>
    /// <returns></returns>
    public List<Coordinates>  GetFullSquareCoordinatesWithEdge(int xCenter, int yCenter, int edgeLength)
    {
        List<Coordinates> toReturn = new List<Coordinates>();
        Coordinates botLeftCoordinates = new Coordinates(xCenter-(edgeLength-2), yCenter-(edgeLength-2));

        for (int i = 0 ; i < edgeLength; i++)
        {
            for (int j = 0; j < edgeLength; j++)
            {
                toReturn.Add(new Coordinates(botLeftCoordinates.x + i, botLeftCoordinates.y + j));
            }
        }
        
        return toReturn;
    }

    /// <summary>
    /// Returns the closest coordinates where there is no tile around, except the tile sending.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="selectionPosition"></param>
    /// <param name="senderTile"></param>
    /// <returns></returns>
    public Coordinates GetClosestFreePositionAround(Coordinates coordinates, Vector3 selectionPosition, Tile senderTile)
    {
        Coordinates toReturn = coordinates;

        int distance = 1;
        bool founded = false;
        float bestDistance = 0;

        while (!founded)
        {
            List<Coordinates> aroundCoordinates = GetEmptySquareCoordinatesWithRadius(coordinates.x, coordinates.y, distance);

            foreach (Coordinates testedCoordinate in aroundCoordinates)
            {
                Tile testedTile = GetTileAtCoordinates(testedCoordinate.x, testedCoordinate.y);

                if (testedTile && testedTile != senderTile) 
                    continue;

                founded = true;
                toReturn.x = testedCoordinate.x;
                toReturn.y = testedCoordinate.y;
                
                if (Vector3.Distance(new Vector3(testedCoordinate.x, 0, testedCoordinate.y), selectionPosition) < bestDistance)
                {
                    toReturn.x = testedCoordinate.x;
                    toReturn.y = testedCoordinate.y;
                }
            }

            distance++;
        }

        return toReturn;
    }
    
    /// <summary>
    /// With two tiles in diagonal, returns the two other tiles to make a square.
    /// </summary>
    /// <param name="tileFrom"></param>
    /// <param name="tileTo"></param>
    /// <returns></returns>
    public List<Tile> GetTilesOfASquareWithDiagonals(Tile tileFrom, Tile tileTo)
    {
        if (!tileFrom.IsDiagonalWith(tileTo))
            return null; // They are not in diagonal

        Tile tile1 = GetTileWithBiggestX(tileFrom, tileTo);
        Tile tile2 = GetTileWithLowestX(tileFrom, tileTo);
        Tile tile3, tile4;

        if(tile1.coordinates.y > tile2.coordinates.y)
        {
            tile3 = GetTileAtCoordinates(tile1.coordinates.x, tile2.coordinates.y);
            tile4 = GetTileAtCoordinates(tile2.coordinates.x, tile1.coordinates.y);
        }
        else
        {
            tile3 = GetTileAtCoordinates(tile2.coordinates.x, tile1.coordinates.y);
            tile4 = GetTileAtCoordinates(tile1.coordinates.x, tile2.coordinates.y);
        }

        return new List<Tile> { tile3, tile4 };
    }
    
    /// <summary>
    /// Returns the covers adjacent (without diagonals) at coordinates.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="coveringTypes"></param>
    /// <returns></returns>
    public List<Cover> GetAdjacentCoversAt(Coordinates coordinates, List<TileType> coveringTypes)
    {
        List<Cover> coversToReturn = new List<Cover>();
        List<Coordinates> aroundCoordinatesList = _board.GetCoordinatesAround(coordinates, 1, false);

        foreach (Coordinates aroundCoordinates in aroundCoordinatesList)
        {
            Tile tileWithCover = GetTileAtCoordinates(aroundCoordinates);
            
            if(tileWithCover)
            {
                Cover tileCover = tileWithCover.GetComponent<Cover>();

                if (tileCover && coveringTypes.Contains(tileCover.GetCoveringTileType()))
                    coversToReturn.Add(tileCover);
            }
        }
        
        return coversToReturn;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns the tile with the biggest x coordinate, or null if two x are equal.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    private Tile GetTileWithBiggestX(Tile tile1, Tile tile2)
    {
        if(tile1.coordinates.x == tile2.coordinates.x)
            return null;

        if(tile1.coordinates.x > tile2.coordinates.x)
            return tile1;

        return tile2;
    }

    /// <summary>
    /// Return the tile with the lowest x coordinate, or null if two x are equal.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    private Tile GetTileWithLowestX(Tile tile1, Tile tile2)
    {
        if (tile1.coordinates.x == tile2.coordinates.x)
            return null;

        return tile1.coordinates.x < tile2.coordinates.x ? tile1 : tile2;
    }
}
