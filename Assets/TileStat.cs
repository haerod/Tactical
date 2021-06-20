using System.Collections;
using UnityEngine;

public class TileStat : MonoBehaviour
{
    [Header("COORDINATES (debug)")]

    public int x;
    public int y;

    [Header("PATHFININDING VALUES (debug)")]
    public int cost;
    public int heuristic; // heuristic (10 = adjacent, 14 = diagonal)
    public int f; // f = cost + heuristic
    public TileStat parent;

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

    public void ResetTile()
    {
        cost = 0;
        heuristic = 0;
        f = 0;
        parent = null;
    }

    public void DisableRenderer()
    {
        rend.enabled = false;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
