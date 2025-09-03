using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static M__Managers;
using TMPro;
using UnityEngine.Serialization;

public class Tile : Entity
{
    [Header("PROPERTIES")]

    public TileType type;

    [Header("COORDINATES")]
    
    [HideInInspector] public int cost;
    [HideInInspector] public int heuristic;
    [HideInInspector] public int f;
    [HideInInspector] public Tile parent;

    public enum TileMaterial { Yellow, Grey, Red, Green, Blue }
    public enum Directions { Top, Down, Right, Left }

    public C__Character character => Character();

    [Header("REFERENCES")]

    [SerializeField] private MeshRenderer areaRend;
    [Space]
    [SerializeField] private Material green;
    [SerializeField] private Material blue;
    [SerializeField] private Material red;
    [SerializeField] private Material yellow;
    [SerializeField] private Material grey;
    [Space]
    [SerializeField] private GameObject areaObject;
    [SerializeField] private GameObject fogMask;
    [Space]
    [SerializeField] private GameObject topLine;
    [SerializeField] private GameObject downLine;
    [SerializeField] private GameObject leftLine;
    [SerializeField] private GameObject rightLine;
    [SerializeField] private TextMeshProUGUI coordinatesText;
    
    [Header("DEBUG")]
    [SerializeField] private bool showCoordinates;
    
    [HideInInspector] public Vector3 worldPosition => transform.position;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        if(!showCoordinates)
            return;
        
        coordinatesText.gameObject.SetActive(true);
        coordinatesText.text = coordinates.x + "," + coordinates.y;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Gives the values to the tile.
    /// </summary>
    /// <param name="newCoordinates"></param>
    public void Setup(Coordinates newCoordinates)
    {
        if(coordinates == null)
            coordinates = new Coordinates(newCoordinates.x, newCoordinates.y);
        else
            coordinates.SetCoordinates(newCoordinates);
        
        string newName = $"{type} ({coordinates.x},{coordinates.y}) ";
        newName = newName.Replace("(TileType)", "");

        gameObject.name = newName;

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    /// <summary>
    /// Moves tile position at the asked coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void MoveAtGridPosition(int x, int y) => transform.position = new Vector3(x, 0, y);

    /// <summary>
    /// Calculates f, cost and heuristic and sets the parent
    /// </summary>
    /// <param name="endTile"></param>
    /// <param name="parentTile"></param>
    public void CalculateValues(Tile endTile, Tile parentTile)
    {
        parent = parentTile;
        cost = parent.cost + GetCost(parentTile);
        heuristic = (Mathf.Abs(endTile.coordinates.x - coordinates.x) + Mathf.Abs(endTile.coordinates.y - coordinates.y)) * 10;
        f = cost + heuristic;
    }

    /// <summary>
    /// Returns the cost of movement from tile to another.
    /// </summary>
    /// <param name="currentTile"></param>
    /// <returns></returns>
    public int GetCost(Tile currentTile) => IsDiagonalWith(currentTile) ? 14 : 10;

    /// <summary>
    /// Returns true if a tile is in diagonal with another tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsDiagonalWith(Tile tile) => coordinates.x != tile.coordinates.x && coordinates.y != tile.coordinates.y;

    /// <summary>
    /// Resets the pathfinding tiles value.
    /// </summary>
    public void ResetTileValues()
    {
        cost = 0;
        heuristic = 0;
        f = 0;
        parent = null;
    }

    /// <summary>
    /// Enables the view lines.
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
    /// Disables all the view lines.
    /// </summary>
    public void DisableViewLine()
    {
        topLine.SetActive(false);
        downLine.SetActive(false);
        rightLine.SetActive(false);
        leftLine.SetActive(false);
    }

    /// <summary>
    /// Enables or disables the fog mask
    /// </summary>
    /// <param name="value"></param>
    public void SetFogMaskActive(bool value)
    {
        fogMask.SetActive(value);
    }

    /// <summary>
    /// Sets the appearance of a tile with the new material.
    /// </summary>
    /// <param name="tileMaterial"></param>
    public void SetMaterial(TileMaterial tileMaterial)
    {
        areaObject.SetActive(true);

        switch (tileMaterial)
        {
            case TileMaterial.Yellow:
                areaRend.material = yellow;
                break;
            case TileMaterial.Grey:
                areaRend.material = grey;
                break;
            case TileMaterial.Blue:
                areaRend.material = blue;
                break;
            case TileMaterial.Green:
                areaRend.material = green;
                break;
            case TileMaterial.Red:
                areaRend.material = red;
                break;
            default:
                Debug.LogError("Add a material here");
                break;
        }
    }

    /// <summary>
    /// Sets the tile skin to "basic".
    /// Shortcut of SetMaterial(TileMaterial.Basic).
    /// </summary>
    public void ResetTileSkin() => areaObject.SetActive(false);

    /// <summary>
    /// Returns true if the tile is occupied by a character.
    /// </summary>
    /// <returns></returns>
    public bool IsOccupiedByCharacter() => Character();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the character on this tile.
    /// Returns null if is nobody on this tile.
    /// </summary>
    /// <returns></returns>
    private C__Character Character() => _characters.GetUnitsList().FirstOrDefault(c => c.tile == this);
}