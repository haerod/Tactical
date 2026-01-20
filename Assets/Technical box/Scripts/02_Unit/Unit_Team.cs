using UnityEngine;
using UnityEditor;
using static M__Managers;

public class Unit_Team : MonoBehaviour
{
    public Team team;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns true if the unit is an ally of the given unit. Else returns false.
    /// </summary>
    /// <param name="testedUnit"></param>
    /// <returns></returns>
    public bool IsAllyOf(Unit testedUnit) => team == testedUnit.unitTeam;

    /// <summary>
    /// Returns true if the unit is an enemy of the given unit. Else returns false.
    /// </summary>
    /// <param name="testedUnit"></param>
    /// <returns></returns>
    public bool IsEnemyOf(Unit testedUnit) => team != testedUnit.unitTeam;
    
    /// <summary>
    /// Returns true if the unit is in the same team of the given unit. Else returns false.
    /// </summary>
    /// <param name="testedUnit"></param>
    /// <returns></returns>
    public bool IsTeammateOf(Unit testedUnit) => team == testedUnit.unitTeam;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
