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

    /// <summary>
    /// Returns true if the character can be healed.
    /// </summary>
    /// <param name="characterToHeal"></param>
    /// <returns></returns>
    private bool IsHealable(C__Character characterToHeal)
    {
        if(characterToHeal.health.IsFullLife())
            return false; // Already full life
        
        bool isOnHealReach = _board
            .GetCoordinatesAround(c.coordinates, healReach, includeDiagonalsInReach)
            .Contains(characterToHeal.coordinates);
        
        return isOnHealReach;
    }

    /// <summary>
    /// Heal the character of heal amount HP.
    /// </summary>
    /// <param name="characterToHeal"></param>
    private void HealCharacter(C__Character characterToHeal) => characterToHeal.health.Heal(healAmount);

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

        HealCharacter(clickedCharacter);
    }
}