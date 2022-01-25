using System.Collections;
using UnityEngine;
using static M__Managers;

public class Tile : MonoBehaviour
{
    [Header("COORDINATES (debug)")]

    public int x;
    public int y;
    
    [Header("PATHFININDING VALUES (debug)")]
    public int cost;
    public int heuristic; // heuristic (10 = adjacent, 14 = diagonal)
    public int f; // f = cost + heuristic
    public Tile parent;

    [Header("MATERIALS (debug)")]
    [SerializeField] private Material basic = null;
    [SerializeField] private Material open = null;
    [SerializeField] private Material closed = null;
    [SerializeField] private Material path = null;
    [SerializeField] private Material area = null;
    [SerializeField] private Material attackable = null;
    [SerializeField] private Material range = null;
    [Space]
    [SerializeField] private GameObject areaObject = null;

    public enum Type { Basic, Hole, BigObstacle}
    public Type type = Type.Basic;

    [Header("REFERENCES")]

    [SerializeField] private MeshRenderer rend = null;
    [Space]
    [SerializeField] private TextMesh cText = null;
    [SerializeField] private TextMesh hText = null;
    [SerializeField] private TextMesh fText = null;
    [Space]
    [SerializeField] private GameObject bigObstacle = null;
    public enum TileMaterial { Basic, Open, Closed, Path, Area, Attackable, Range} // Update the SetMaterial() method if add/remove a tile material.

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

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
    /// Disable renderer and set hole tile type.
    /// </summary>
    public void EnableHole()
    {
        type = Tile.Type.Hole;
        rend.enabled = false;
    }

    /// <summary>
    /// Enable the big obstacle on tile and set big obstacle type.
    /// </summary>
    public void EnableBigObstacle()
    {
        type = Tile.Type.BigObstacle;
        bigObstacle.SetActive(true);
    }

    /// <summary>
    /// Set the appearance of a tile with the new material.
    /// </summary>
    /// <param name="mat"></param>
    public void SetMaterial(TileMaterial mat)
    {
        if(mat != TileMaterial.Area || mat != TileMaterial.Attackable)
        {
            areaObject.SetActive(false);
        }

        switch (mat)
        {
            case TileMaterial.Basic:
                rend.material = basic;
                break;
            case TileMaterial.Open:
                rend.material = open;
                break;
            case TileMaterial.Closed:
                rend.material = closed;
                break;
            case TileMaterial.Path:
                rend.material = path;
                break;
            case TileMaterial.Area:
                areaObject.SetActive(true);
                areaObject.GetComponent<Renderer>().material = area;
                break;
            case TileMaterial.Attackable:
                areaObject.SetActive(true);
                areaObject.GetComponent<Renderer>().material = attackable;
                break;
            case TileMaterial.Range:
                rend.material = range;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set the tile skin to "basic".
    /// Short cut of SetMaterial(TileMaterial.Basic).
    /// </summary>
    public void ResetTileSkin()
    {
        SetMaterial(TileMaterial.Basic);
    }

    /// <summary>
    /// Return true if the tile is occupied by a character.
    /// </summary>
    /// <returns></returns>
    public bool IsOccupiedByCharacter()
    {
        foreach (C__Character c in _characters.characters)
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
        foreach (C__Character c in _characters.characters)
        {
            if(c.Tile() == this) return c;
        }

        return null;
    }

    // DEBUG
    // =====

    /// <summary>
    /// Debug : Show the pathfinding values visually. 
    /// To show, enable the texts on the prefab, there are the TextMesh components in references, and call this method.
    /// </summary>
    public void ShowValues()
    {
        cText.gameObject.SetActive(true);
        hText.gameObject.SetActive(true);
        fText.gameObject.SetActive(true);

        cText.text = cost.ToString();
        hText.text = heuristic.ToString();
        fText.text = f.ToString();
    }

    /// <summary>
    /// Debug : Hide the pathfinding values visually.
    /// To hide, enable the texts on the prefab, there are the TextMesh components in references, and call this method.
    /// Uncomment this lines in M_TileBoard, in GenerateBoard() method for a better lisibility.
    /// </summary>
    public void HideValues()
    {
        cText.gameObject.SetActive(false);
        hText.gameObject.SetActive(false);
        fText.gameObject.SetActive(false);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}