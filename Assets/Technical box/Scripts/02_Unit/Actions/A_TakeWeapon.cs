using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class A_TakeWeapon : A__Action
{
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Equips the given weapon to the unit.
    /// </summary>
    /// <param name="weapon"></param>
    public void EquipWeapon(Weapon weapon)
    {
        StartAction();

        unit.weaponHolder.EquipWeapon(weapon);

        EndAction();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================

}