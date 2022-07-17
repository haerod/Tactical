using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class C_Infos : MonoBehaviour
{
    public string designation = "Name";
    public int team = 1;

    [Header("REFERENCES")]
    [SerializeField] private C__Character c = null;
    [SerializeField] private Renderer rend1 = null;
    [SerializeField] private Renderer rend2 = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    public void Start()
    {
        string playable = "PC";
        if (!c.behavior.playable) playable = "NPC";

        transform.parent.name = string.Format("{2} - {0} (T{1})", designation, team, playable);
        SetTeamMaterials();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Set the team materials to the character.
    /// </summary>
    private void SetTeamMaterials()
    {
        rend1.material = _rules.teamInfos[c.Team()].mat1;
        rend2.material = _rules.teamInfos[c.Team()].mat2;
    }
}
