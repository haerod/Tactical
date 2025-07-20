using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class A_Heal : A__Action
{
    [SerializeField] private int healAmount = 1;
    
    [SerializeField] private int healReach = 1;
    [FormerlySerializedAs("useDiagonals")] [SerializeField] private bool includeDiagonalsInReach = true;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private bool IsHealable(C__Character characterToHeal)
    {
        if(characterToHeal.health.IsFullLife())
            return false; // Already full life
        
        bool isOnHealReach = _board
            .GetCoordinatesAround(c.coordinates, healReach, includeDiagonalsInReach)
            .Contains(characterToHeal.coordinates);
        
        return isOnHealReach;
    }
    
    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================

    protected override void OnHoverAlly(C__Character hoveredCharacter)
    {
        if(!IsHealable(hoveredCharacter))
            return; // Not healable
        
    }

    protected override void OnClickAlly(C__Character clickedCharacter)
    {
        if(!IsHealable(clickedCharacter))
            return; // Not healable
        
        clickedCharacter.health.Heal(healAmount);
    }
}