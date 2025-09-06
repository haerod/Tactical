using UnityEngine;
using UnityEditor;
using static M__Managers;

public class U_Team : MonoBehaviour
{
    public Team team;

    [Header("REFERENCES")]
    [SerializeField] private Renderer rend1;
    [SerializeField] private Renderer rend2;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Sets the team materials to the unit.
    /// Called by UnitAutoRename.
    /// </summary>
    public void SetTeamMaterials()
    {
        if(!team)
        {
            Debug.LogError(transform.parent.name + " doesn't have a team. Please assign a team.", transform.parent.gameObject);
            return; // No team assigned
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

    /// <summary>
    /// Returns true if the unit is an ally of the given unit. Else returns false.
    /// </summary>
    /// <param name="testedCharacter"></param>
    /// <returns></returns>
    public bool IsAllyOf(U__Unit testedCharacter) => team == testedCharacter.unitTeam;

    /// <summary>
    /// Returns true if the unit is an enemy of the given unit. Else returns false.
    /// </summary>
    /// <param name="testedCharacter"></param>
    /// <returns></returns>
    public bool IsEnemyOf(U__Unit testedCharacter) => team != testedCharacter.unitTeam;
    
    /// <summary>
    /// Returns true if the unit is in the same team of the given unit. Else returns false.
    /// </summary>
    /// <param name="testedCharacter"></param>
    /// <returns></returns>
    public bool IsTeammateOf(U__Unit testedCharacter) => team == testedCharacter.unitTeam;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
