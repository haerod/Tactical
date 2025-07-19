using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public abstract class A__Action : MonoBehaviour
{
    [SerializeField] private List<A__ActionTrigger> actionTriggers;
    
    [Header("REFERENCES")]
    
    [SerializeField] protected C__Character c;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void SubscribeToEvents()
    {
        foreach (A__ActionTrigger actionTrigger in actionTriggers)
        {
            switch (actionTrigger.name)
            {
                case "OnAlliedHover":
                    // print("OnAlliedHover subscribed");
                    break;
                case "OnTileEnter":
                    _input.OnTileEnter += Input_OnTileEnter;
                    break;
                case "OnTileClicked":
                    _input.OnTileClick += Input_OnTileClick;
                    break;
                case "OnCharacterClicked":
                    _input.OnCharacterClick += Input_OnCharacterClick;
                    break;
                default:
                    break;
            }
        }
    }

    public void UnsubscribeToEvents()
    {
        foreach (A__ActionTrigger actionTrigger in actionTriggers)
        {
            switch (actionTrigger.name)
            {
                case "OnAlliedHover":
                    // print("OnAlliedHover unsubscribed");
                    break;
                case "OnTileEnter":
                    _input.OnTileEnter -= Input_OnTileEnter;
                    break;
                case "OnTileClicked":
                    _input.OnTileClick -= Input_OnTileClick;
                    break;
                case "OnCharacterClicked":
                    _input.OnCharacterClick -= Input_OnCharacterClick;
                    break;
                default:
                    break;
            }
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    protected virtual void Input_OnCharacterClick(object sender, C__Character clickedCharacter) { }
    
    protected virtual void Input_OnTileEnter(object sender, Tile enteredTile)
    {
        if(enteredTile.character)
            OnHoverCharacter(enteredTile.character);
    }

    protected virtual void Input_OnTileClick(object sender, Tile clickedTile) { }
    
    protected virtual void OnHoverCharacter(C__Character hoveredCharacter)
    {
        if (hoveredCharacter == c)
            OnHoverItself();
        else if (c.team.IsAllyOf(hoveredCharacter))
            OnHoverAllied(hoveredCharacter);
        else if (c.team.IsEnemyOf(hoveredCharacter))
            OnHoverEnemy(hoveredCharacter);
    }

    protected virtual void OnHoverAllied(C__Character hoveredAlly) { }

    protected virtual void OnHoverEnemy(C__Character hoveredEnemy) { }

    protected virtual void OnHoverItself() { }
}
