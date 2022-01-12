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

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void ShowValues()
    {
        cText.gameObject.SetActive(true);
        hText.gameObject.SetActive(true);
        fText.gameObject.SetActive(true);

        cText.text = cost.ToString();
        hText.text = heuristic.ToString();
        fText.text = f.ToString();
    }

    public void HideValues()
    {
        cText.gameObject.SetActive(false);
        hText.gameObject.SetActive(false);
        fText.gameObject.SetActive(false);
    }

    public void ResetTileValues()
    {
        cost = 0;
        heuristic = 0;
        f = 0;
        parent = null;
    }

    public void ResetTileSkin()
    {
        SetMaterial(TileMaterial.Basic);
    }

    public void DisableRenderer()
    {
        rend.enabled = false;
    }

    public void EnableBigObstacle()
    {
        bigObstacle.SetActive(true);
    }

    public enum TileMaterial { Basic, Open, Closed, Path, Area, Attackable, Range}
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

    public bool IsOccupied()
    {
        foreach (Character c in _characters.characters)
        {
            if (c.move.x == x && c.move.y == y)
                return true;
        }
        return false;
    }

    public Character Character()
    {
        foreach (Character c in _characters.characters)
        {
            if(c.Tile() == this) return c;
        }

        return null;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}