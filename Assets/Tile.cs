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
    [Space]
    [SerializeField] private GameObject area = null;
    public enum TileMaterial { Basic, Open, Closed, Path, Area}

    [HideInInspector] public bool hole = false;

    [Header("REFERENCES")]

    [SerializeField] private MeshRenderer rend = null;
    [Space]
    [SerializeField] private TextMesh cText = null;
    [SerializeField] private TextMesh hText = null;
    [SerializeField] private TextMesh fText = null;

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

    public void ResteTileSkin()
    {
        SetMaterial(TileMaterial.Basic);
    }

    public void DisableRenderer()
    {
        rend.enabled = false;
    }

    public void SetMaterial(TileMaterial mat)
    {
        if(mat != TileMaterial.Area)
        {
            area.SetActive(false);
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
                area.SetActive(true);
                break;
            default:
                break;
        }
    }

    public bool IsOccupied()
    {
        foreach (Character c in _characters.characters)
        {
            if (c.gridMove.x == x && c.gridMove.y == y)
                return true;
        }
        return false;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
