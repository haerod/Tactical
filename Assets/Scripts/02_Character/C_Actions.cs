using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class C_Actions : MonoBehaviour
{
    [SerializeField] private List<A__Action> actions;
    
    [Header("REFERENCES")]
    [SerializeField] private C__Character c;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Character subscribes to click event.
    /// </summary>
    public void SubscribeToInputClick() => _input.OnClickOnCharacter += Input_OnClickOnCharacter;

    /// <summary>
    /// Character unsubscribes to click event.
    /// </summary>
    public void UnsubscribeToInputClick() => _input.OnClickOnCharacter -= Input_OnClickOnCharacter;

    /// <summary>
    /// Character subscribes to Input's OnEnterTile event.
    /// </summary>
    public void SubscribeToEnterTile() => _input.OnEnterTile += Input_OnEnterTile;

    /// <summary>
    /// Character unsubscribes to Input's OnEnterTile event.
    /// </summary>
    public void UnsubscribeToEnterTile() => _input.OnEnterTile -= Input_OnEnterTile;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void OnHoverCharacter(C__Character hoveredCharacter)
    {
        if (hoveredCharacter == c)
        {
            OnHoverItself();
            return;
        }
        else if (c.team.IsAllyOf(hoveredCharacter))
        {
            OnHoverAllied(hoveredCharacter);
            return;
        }
        else if (c.team.IsEnemyOf(hoveredCharacter))
        {
            OnHoverEnemy(hoveredCharacter);
            return;
        }
    }

    private void OnHoverAllied(C__Character hoveredCharacter)
    {
        
    }

    private void OnHoverEnemy(C__Character hoveredCharacter)
    {
        
    }

    private void OnHoverItself()
    {
        
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Input_OnClickOnCharacter(object sender, C__Character clickedCharacter)
    {
    }
    
    private void Input_OnEnterTile(object sender, Tile enteredTile)
    {
        if(enteredTile.character)
            OnHoverCharacter(enteredTile.character);
    }
}
