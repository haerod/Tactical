using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class Infos : MonoBehaviour
{
    public string designation = "Name";
    public int team = 1;

    [Header("REFERENCES")]
    [SerializeField] private Character c = null;
    [SerializeField] private Renderer rend1 = null;
    [SerializeField] private Renderer rend2 = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    public void Start()
    {
        transform.parent.name = string.Format("Character : {0} ({1})", designation, team);
        GetTeamColors();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void GetTeamColors()
    {
        rend1.material = _rules.teamInfos[c.Team()].mat1;
        rend2.material = _rules.teamInfos[c.Team()].mat2;
    }
}
