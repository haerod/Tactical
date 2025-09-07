using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using static M__Managers;

public abstract class A__Action : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] protected U__Unit unit;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void SubscribeToEvents()
    {
        InputEvents.OnCharacterEnter += InputEvents_OnCharacterEnter;
        InputEvents.OnCharacterExit += InputEvents_OnCharacterExit;
        InputEvents.OnCharacterClick += InputEvents_OnCharacterClick;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnTileClick += InputEvents_OnTileClick;
    }

    public void UnsubscribeToEvents()
    {
        InputEvents.OnCharacterEnter -= InputEvents_OnCharacterEnter;
        InputEvents.OnCharacterExit -= InputEvents_OnCharacterExit;
        InputEvents.OnCharacterClick -= InputEvents_OnCharacterClick;
        InputEvents.OnTileEnter -= InputEvents_OnTileEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
        InputEvents.OnTileClick -= InputEvents_OnTileClick;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts a wait for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    protected void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits coroutine.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // Characters
    // ----------
    
    /// <summary>
    /// Happens when any character is hovered, including itself.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAnyCharacter(U__Unit hoveredCharacter) { }

    /// <summary>
    /// Happens when any character is hovered, except itself.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAnyOtherCharacter(U__Unit hoveredCharacter) { }
    
    /// <summary>
    /// Happens when an ally is hovered.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAlly(U__Unit hoveredCharacter) { }

    /// <summary>
    /// Happens when an enemy is hovered.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverEnemy(U__Unit hoveredCharacter) { }
    
    /// <summary>
    /// Happens when the character is hovered.
    /// </summary>
    protected virtual void OnHoverItself() { }
    
    /// <summary>
    /// Happens when the cursor leaves any character.
    /// </summary>
    /// <param name="leftCharacter"></param>
    protected virtual void OnExitCharacter(U__Unit leftCharacter) { }
    
    /// <summary>
    /// Happens when any character is hovered, including itself.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAnyCharacter(U__Unit clickedCharacter) { }
    
    /// <summary>
    /// Happens when any character is clicked, except itself.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAnyOtherCharacter(U__Unit clickedCharacter) { }
    
    /// <summary>
    /// Happens when an ally is clicked.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAlly(U__Unit clickedCharacter) { }
    
    /// <summary>
    /// Happens when an enemy is clicked.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickEnemy(U__Unit clickedCharacter) { }
    
    /// <summary>
    /// Happens when the character is clicked.
    /// </summary>
    protected virtual void OnClickItself() { }
    
    // Tiles
    // -----
    
    /// <summary>
    /// Happens when a tile is hovered.
    /// </summary>
    /// <param name="hoveredTile"></param>
    protected virtual void OnHoverTile(Tile hoveredTile) { }
    
    /// <summary>
    /// Happens when a tile is hovered and occupied by a character.
    /// </summary>
    /// <param name="hoveredTile"></param>
    protected virtual void OnHoverOccupiedTile(Tile hoveredTile) { }
    
    /// <summary>
    /// Happens when a tile is hovered and without character.
    /// </summary>
    /// <param name="hoveredTile"></param>
    protected virtual void OnHoverFreeTile(Tile hoveredTile) { }
    
    /// <summary>
    /// Happens when the cursor leaves any tile.
    /// </summary>
    /// <param name="hoveredTile"></param>
    protected virtual void OnExitTile(Tile hoveredTile) { }
    
    /// <summary>
    /// Happens when a tile is clicked.
    /// </summary>
    /// <param name="clickedTile"></param>
    protected virtual void OnClickTile(Tile clickedTile) { }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputEvents_OnCharacterEnter(object sender, U__Unit enteredCharacter)
    {
        if (!unit.look.CharactersVisibleInFog().Contains(enteredCharacter))
        {
            OnHoverFreeTile(enteredCharacter.tile);
            return; // Invisible character
        }
        
        OnHoverAnyCharacter(enteredCharacter);
        
        if (enteredCharacter == unit)
            OnHoverItself();
        else
        {
            OnHoverAnyOtherCharacter(enteredCharacter);

            if (unit.team.IsAllyOf(enteredCharacter))
                OnHoverAlly(enteredCharacter);
            else
                OnHoverEnemy(enteredCharacter);
        }
    }

    private void InputEvents_OnCharacterExit(object sender, U__Unit leftCharacter)
    {
        OnExitCharacter(leftCharacter);
    }

    private void InputEvents_OnCharacterClick(object sender, U__Unit clickedCharacter)
    {
        if (!unit.look.CharactersVisibleInFog().Contains(clickedCharacter))
        {
            OnClickTile(clickedCharacter.tile);
            return; // Invisible character
        }
        
        OnClickAnyCharacter(clickedCharacter);
        
        if (clickedCharacter == unit)
            OnClickItself();
        else
        {
            OnClickAnyOtherCharacter(clickedCharacter);

            if (unit.team.IsAllyOf(clickedCharacter))
                OnClickAlly(clickedCharacter);
            else
                OnClickEnemy(clickedCharacter);
        }
    }

    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        OnHoverTile(enteredTile);
        
        if(enteredTile.character)
            OnHoverOccupiedTile(enteredTile);
        else
            OnHoverFreeTile(enteredTile);
    }

    private void InputEvents_OnTileExit(object sender, Tile leavedTile)
    {
        OnExitTile(leavedTile);
    }

    private void InputEvents_OnTileClick(object sender, Tile clickedTile)
    {
        OnClickTile(clickedTile);
    }
}
