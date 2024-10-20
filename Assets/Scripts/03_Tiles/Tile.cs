using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static M__Managers;

public class Tile : MonoBehaviour
{
    [Header("PROPERTIES")]

    public TileType type;

    [Header("COORDINATES")]

    public int x; // Let it serialized to let Tile be dirty.
    public int y; // Let it serialized to let Tile be dirty.
    public Vector2Int coordinates => new Vector2Int(x, y);

    [HideInInspector] public int cost;
    [HideInInspector] public int heuristic;
    [HideInInspector] public int f;
    [HideInInspector] public Tile parent;

    public enum TileMaterial { Yellow, Grey, Red, Green, Blue }
    public enum Directions { Top, Down, Right, Left }

    public C__Character character => Character();

    [Header("REFERENCES")]

    [SerializeField] private MeshRenderer areaRend = null;
    [Space]
    [SerializeField] private Material green = null;
    [SerializeField] private Material blue = null;
    [SerializeField] private Material red = null;
    [SerializeField] private Material yellow = null;
    [SerializeField] private Material grey = null;
    [Space]
    [SerializeField] private GameObject areaObject = null;
    [SerializeField] private GameObject fogMask = null;
    [Space]
    [SerializeField] private GameObject topLine = null;
    [SerializeField] private GameObject downLine = null;
    [SerializeField] private GameObject leftLine = null;
    [SerializeField] private GameObject rightLine = null;

    public List<Cover> covers;

    [HideInInspector] public bool hasCovers => covers.Count > 0;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Give the values to the tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="tileName"></param>
    public void Setup(Vector2Int newCoordinates)
    {
        x = newCoordinates.x;
        y = newCoordinates.y;

        string newName = $"{type} ({x},{y}) ";
        newName = newName.Replace("(TileType)", "");

        gameObject.name = newName;

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    /// <summary>
    /// Move tile position at the asked coordinates.
    /// Rename the element.
    /// Set the new elements dirty.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void MoveAtGridPosition(int x, int y) => transform.position = new Vector3(x, 0, y);

    /// <summary>
    /// Calculate f, cost and heuristic and set the parent
    /// </summary>
    /// <param name="endTile"></param>
    /// <param name="parentTile"></param>
    public void CalulateValues(Tile endTile, Tile parentTile)
    {
        parent = parentTile;
        cost = parent.cost + GetCost(parentTile);
        heuristic = (Mathf.Abs(endTile.x - x) + Mathf.Abs(endTile.y - y)) * 10;
        f = cost + heuristic;
    }

    /// <summary>
    /// Add a cover in covers list.
    /// </summary>
    /// <param name="cover"></param>
    public void AddCover(Cover cover)
    {
        if (covers.Contains(cover))
            return; // Already this cover in the list

        covers.Add(cover);
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Remove a cover form covers list.
    /// </summary>
    /// <param name="cover"></param>
    public void RemoveCover(Cover cover)
    {
        if (!covers.Contains(cover))
            return; // This cover doesn't in the list

        covers.Remove(cover);
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Get covers list.
    /// </summary>
    /// <returns></returns>
    public List<Cover> GetCovers() => covers;

    /// <summary>
    /// Return true if it's a cover between this tile and another tile.
    /// </summary>
    /// <param name="otherTile"></param>
    /// <param name="allowedWalkableTypes"></param>
    /// <returns></returns>
    public bool IsCoverBetween(Tile otherTile, List<TileType> allowedWalkableTypes)
    {            
        List<Cover> testedCovers = new List<Cover>();

        if(!Utils.IsVoidList(GetCovers()))
            testedCovers.AddRange(GetCovers());
        if(!Utils.IsVoidList(otherTile.GetCovers()))
            testedCovers.AddRange(otherTile.GetCovers());

        return testedCovers
            .Where(c => !allowedWalkableTypes.Contains(c.type))
            .Where(c => c.IsBetweenTiles(this, otherTile))
            .FirstOrDefault() != null;
    }

    /// <summary>
    /// Return the cost of movement from tile to another.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="currentTile"></param>
    /// <returns></returns>
    public int GetCost(Tile currentTile) => IsDiagonalWith(currentTile) ? 14 : 10;

    /// <summary>
    /// Return true if a tile is in diagonal with another tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsDiagonalWith(Tile tile) => x != tile.x && y != tile.y;

    /// <summary>
    /// Reset the pathfinding tiles value.
    /// </summary>
    public void ResetTileValues()
    {
        cost = 0;
        heuristic = 0;
        f = 0;
        parent = null;
    }

    /// <summary>
    /// Enable the view lines.
    /// </summary>
    public void EnableViewLine(Directions direction)
    {
        switch (direction)
        {
            case Directions.Top:
                topLine.SetActive(true);
                break;
            case Directions.Down:
                downLine.SetActive(true);
                break;
            case Directions.Right:
                rightLine.SetActive(true);
                break;
            case Directions.Left:
                leftLine.SetActive(true);
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// Disbale all the view lines.
    /// </summary>
    public void DisableViewLine()
    {
        topLine.SetActive(false);
        downLine.SetActive(false);
        rightLine.SetActive(false);
        leftLine.SetActive(false);
    }

    /// <summary>
    /// Enable or disable the fog mask
    /// </summary>
    /// <param name="value"></param>
    public void SetFogMaskActive(bool value)
    {
        fogMask.SetActive(value);
    }

    /// <summary>
    /// Set the appearance of a tile with the new material.
    /// </summary>
    /// <param name="tileMaterial"></param>
    public void SetMaterial(TileMaterial tileMaterial)
    {
        areaObject.SetActive(true);

        if (tileMaterial == TileMaterial.Yellow)
            areaRend.material = yellow;
        else if (tileMaterial == TileMaterial.Grey)
            areaRend.material = grey;
        else if (tileMaterial == TileMaterial.Blue)
            areaRend.material = blue;
        else if (tileMaterial == TileMaterial.Green)
            areaRend.material = green;
        else if (tileMaterial == TileMaterial.Red)
            areaRend.material = red;
        else
            Debug.LogError("Add a material here");
    }

    /// <summary>
    /// Set the tile skin to "basic".
    /// Short cut of SetMaterial(TileMaterial.Basic).
    /// </summary>
    public void ResetTileSkin() => areaObject.SetActive(false);

    /// <summary>
    /// Return true if the tile is occupied by a character.
    /// </summary>
    /// <returns></returns>
    public bool IsOccupiedByCharacter()
    {
        foreach (C__Character c in _characters.GetCharacterList())
        {
            if (c.move.x == x && c.move.y == y)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Return the character on this tile.
    /// Return null if is nobody on this tile.
    /// </summary>
    /// <returns></returns>
    public C__Character Character()
    {
        foreach (C__Character c in _characters.GetCharacterList())
        {
            if(c.tile == this) 
                return c;
        }

        return null;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}