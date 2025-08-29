using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static M__Managers;

public abstract class A__Action : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] protected C__Character c;
    
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

    // Characters
    // ----------
    
    /// <summary>
    /// Happens when any character is hovered, including itself.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAnyCharacter(C__Character hoveredCharacter) { }

    /// <summary>
    /// Happens when any character is hovered, except itself.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAnyOtherCharacter(C__Character hoveredCharacter) { }
    
    /// <summary>
    /// Happens when an ally is hovered.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAlly(C__Character hoveredCharacter) { }

    /// <summary>
    /// Happens when an enemy is hovered.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverEnemy(C__Character hoveredCharacter) { }
    
    /// <summary>
    /// Happens when the character is hovered.
    /// </summary>
    protected virtual void OnHoverItself() { }
    
    /// <summary>
    /// Happens when the cursor leaves any character.
    /// </summary>
    /// <param name="leftCharacter"></param>
    protected virtual void OnExitCharacter(C__Character leftCharacter) { }
    
    /// <summary>
    /// Happens when any character is hovered, including itself.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAnyCharacter(C__Character clickedCharacter) { }
    
    /// <summary>
    /// Happens when any character is clicked, except itself.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAnyOtherCharacter(C__Character clickedCharacter) { }
    
    /// <summary>
    /// Happens when an ally is clicked.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAlly(C__Character clickedCharacter) { }
    
    /// <summary>
    /// Happens when an enemy is clicked.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickEnemy(C__Character clickedCharacter) { }
    
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
    
    private void InputEvents_OnCharacterEnter(object sender, C__Character enteredCharacter)
    {
        OnHoverAnyCharacter(enteredCharacter);
        
        if (enteredCharacter == c)
            OnHoverItself();
        else
        {
            OnHoverAnyOtherCharacter(enteredCharacter);

            if (c.team.IsAllyOf(enteredCharacter))
                OnHoverAlly(enteredCharacter);
            else
                OnHoverEnemy(enteredCharacter);
        }
    }

    private void InputEvents_OnCharacterExit(object sender, C__Character leftCharacter)
    {
        OnExitCharacter(leftCharacter);
    }

    private void InputEvents_OnCharacterClick(object sender, C__Character clickedCharacter)
    {
        OnClickAnyCharacter(clickedCharacter);
        
        if (clickedCharacter == c)
            OnClickItself();
        else
        {
            OnClickAnyOtherCharacter(clickedCharacter);

            if (c.team.IsAllyOf(clickedCharacter))
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
