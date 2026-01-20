using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class A_Heal : A__Action
{
    [SerializeField] private int healAmount = 1;
    
    [SerializeField] private int healReach = 1;
    [SerializeField] private bool includeDiagonalsInReach = true;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns true if the unit can be healed.
    /// </summary>
    /// <param name="unitToHeal"></param>
    /// <returns></returns>
    public bool IsHealable(Unit unitToHeal)
    {
        if(unitToHeal.health.IsFullLife())
            return false; // Already full life
        
        bool isOnHealReach = _board
            .GetCoordinatesAround(unit.coordinates, healReach, includeDiagonalsInReach)
            .Contains(unitToHeal.coordinates);
        
        return isOnHealReach;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Heal the character of heal amount HP.
    /// </summary>
    /// <param name="characterToHeal"></param>
    private void HealCharacter(Unit characterToHeal)
    {
        StartAction();
        
        characterToHeal.health.Heal(healAmount);
        
        EndAction();
    }

    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================
    
    protected override void OnClickAlly(Unit clickedCharacter)
    {
        if(!IsHealable(clickedCharacter))
            return; // Not healable

        HealCharacter(clickedCharacter);
    }
}