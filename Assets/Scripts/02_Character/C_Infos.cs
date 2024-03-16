using UnityEngine;
using UnityEditor;
using static M__Managers;

public class C_Infos : MonoBehaviour
{
    public string designation = "Name";
    public Team team;

    [Header("REFERENCES")]
    //[SerializeField] private C__Character c = null;
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
    /// Called by CharacterAutoRename.
    /// </summary>
    public void SetTeamMaterials()
    {
        if(!team)
        {
            Debug.LogError(designation + " doesn't have a team. Please assign a team.");
            return;
        }

        if (rend1 && team.mainMaterial)
        {
            rend1.material = team.mainMaterial;
            EditorUtility.SetDirty(rend1);
        }
        if (rend2 && team.secondaryMaterial)
        {
            rend2.material = team.secondaryMaterial;
            EditorUtility.SetDirty(rend2);
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
