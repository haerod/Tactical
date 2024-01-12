using UnityEngine;
using UnityEditor;
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

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Set the team materials to the character.
    /// Called by M_Board.
    /// </summary>
    public void SetTeamMaterials()
    {
        M_Rules rules = GameObject.FindAnyObjectByType<M_Rules>();

        if (rend1 && rules.teamInfos[c.Team()].mat1)
        {
            rend1.material = rules.teamInfos[c.Team()].mat1;
            EditorUtility.SetDirty(rend1);
        }
        if (rend2 && rules.teamInfos[c.Team()].mat2)
        {
            rend2.material = rules.teamInfos[c.Team()].mat2;
            EditorUtility.SetDirty(rend2);
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
