using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using static M__Managers;

public abstract class A__Action : MonoBehaviour
{
    
    [SerializeField] protected bool usableOnTurnStart;
    [SerializeField] protected List<A__Action> allowedActionsOnEnd;
    
    [Header("REFERENCES")]
    
    [SerializeField] protected U__Unit unit;
    
    public bool isUsableOnStart => usableOnTurnStart;
    
    public static event EventHandler<U__Unit> OnAnyActionStart;
    public static event EventHandler<U__Unit> OnAnyActionEnd;
    
    private bool canUseAction;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================

    protected virtual void OnDisable()
    {
        UnsubscribeToEvents();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Subscribes to action's events.
    /// </summary>
    public void SubscribeToEvents()
    {
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnUnitExit += InputEvents_OnUnitExit;
        InputEvents.OnUnitClick += InputEvents_OnUnitClick;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnTileClick += InputEvents_OnTileClick;
    }

    /// <summary>
    /// Unsubscribes to action's events.
    /// </summary>
    public void UnsubscribeToEvents()
    {
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
        InputEvents.OnUnitExit -= InputEvents_OnUnitExit;
        InputEvents.OnUnitClick -= InputEvents_OnUnitClick;
        InputEvents.OnTileEnter -= InputEvents_OnTileEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
        InputEvents.OnTileClick -= InputEvents_OnTileClick;
    }
    
    /// <summary>
    /// Returns if the action can be used this turn.
    /// </summary>
    /// <returns></returns>
    public bool CanUse() => canUseAction;
    
    /// <summary>
    /// Set the value of Can use action.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public void SetCanUseAction(bool value) => canUseAction = value;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts the action.
    /// </summary>
    protected void StartAction()
    {
        OnAnyActionStart?.Invoke(this, unit);
    }
    
    /// <summary>
    /// Ends the action and set the usability of the next ones.
    /// </summary>
    protected virtual void EndAction()
    {
        unit.actions.SetActionsUsabilityOf(allowedActionsOnEnd);
        OnAnyActionEnd?.Invoke(this, unit);
        
        if (allowedActionsOnEnd.Count > 0)
            return; // Other actions to do
        
        unit.SetCanPlayValue(false);
        _units.EndCurrentUnitTurn();
    }
    
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
    /// <param name="hoveredUnit"></param>
    protected virtual void OnHoverEnemy(U__Unit hoveredUnit) { }
    
    /// <summary>
    /// Happens when the character is hovered.
    /// </summary>
    protected virtual void OnHoverItself() { }
    
    /// <summary>
    /// Happens when the cursor leaves any character.
    /// </summary>
    /// <param name="exitedUnit"></param>
    protected virtual void OnExitCharacter(U__Unit exitedUnit) { }
    
    /// <summary>
    /// Happens when any character is hovered, including itself.
    /// </summary>
    /// <param name="clickedUnit"></param>
    protected virtual void OnClickAnyCharacter(U__Unit clickedUnit) { }
    
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
    
    private void InputEvents_OnUnitEnter(object sender, U__Unit enteredCharacter)
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

    private void InputEvents_OnUnitExit(object sender, U__Unit leftCharacter)
    {
        OnExitCharacter(leftCharacter);
    }

    private void InputEvents_OnUnitClick(object sender, U__Unit clickedCharacter)
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
