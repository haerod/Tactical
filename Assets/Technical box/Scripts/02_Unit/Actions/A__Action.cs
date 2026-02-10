using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using static M__Managers;

public abstract class A__Action : MonoBehaviour
{
    [SerializeField] private string _actionName;
    public string actionName => _actionName;
    [SerializeField] private Sprite _icon;
    public Sprite icon => _icon;
    [TextArea][SerializeField] private string _actionDescription;
    public string actionDescription => _actionDescription;
    
    [Header("ACTION POINTS")] 
    
    [SerializeField] protected bool _costDependsOnWeapon;
    [SerializeField] protected bool _spendAllActionPoints;
    [SerializeField] protected int _actionPointCost = 1;
    [SerializeField] protected bool _usableWithoutActionPoints;

    public bool costDependsOnTheWeapon => _costDependsOnWeapon;
    public int actionPointCost => costDependsOnTheWeapon ? unit.weaponHolder.weaponData.actionPointCost : _actionPointCost;
    public bool usableWithoutActionPoints => _usableWithoutActionPoints;
    public bool spendAllActionPoints => _spendAllActionPoints;
    
    [Header("REFERENCES")]
    
    [SerializeField] protected Unit unit;

    public bool isAvailable => IsAvailable();
    
    public event EventHandler<A__Action> OnActionStart;
    public event EventHandler<A__Action> OnActionEnd;
    
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
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Starts the action.
    /// </summary>
    protected void StartAction()
    {
        GameEvents.InvokeOnAnyActionStart(unit);
        OnActionStart?.Invoke(this, this);
    }
    
    /// <summary>
    /// Ends the action and set the usability of the next ones.
    /// </summary>
    protected void EndAction()
    {
        GameEvents.InvokeOnAnyActionEnd(unit);
        OnActionEnd?.Invoke(this, this);
        
        if (unit.actionsHolder.areAvailableActions)
        {
            if (!unit.behavior.playable)
                unit.behavior.PlayBehavior();
            return; // Other actions to do
        }
        
        unit.SetCanPlayValue(false);
        _units.EndCurrentUnitTurn();
    }

    /// <summary>
    /// Returns true if is available.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsAvailable() => true;
    
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
    protected virtual void OnHoverAnyCharacter(Unit hoveredCharacter) { }

    /// <summary>
    /// Happens when any character is hovered, except itself.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAnyOtherCharacter(Unit hoveredCharacter) { }
    
    /// <summary>
    /// Happens when an ally is hovered.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    protected virtual void OnHoverAlly(Unit hoveredCharacter) { }

    /// <summary>
    /// Happens when an enemy is hovered.
    /// </summary>
    /// <param name="hoveredUnit"></param>
    protected virtual void OnHoverEnemy(Unit hoveredUnit) { }
    
    /// <summary>
    /// Happens when the character is hovered.
    /// </summary>
    protected virtual void OnHoverItself() { }
    
    /// <summary>
    /// Happens when the cursor leaves any character.
    /// </summary>
    /// <param name="exitedUnit"></param>
    protected virtual void OnExitCharacter(Unit exitedUnit) { }
    
    /// <summary>
    /// Happens when any character is hovered, including itself.
    /// </summary>
    /// <param name="clickedUnit"></param>
    protected virtual void OnClickAnyCharacter(Unit clickedUnit) { }
    
    /// <summary>
    /// Happens when any character is clicked, except itself.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAnyOtherCharacter(Unit clickedCharacter) { }
    
    /// <summary>
    /// Happens when an ally is clicked.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickAlly(Unit clickedCharacter) { }
    
    /// <summary>
    /// Happens when an enemy is clicked.
    /// </summary>
    /// <param name="clickedCharacter"></param>
    protected virtual void OnClickEnemy(Unit clickedCharacter) { }
    
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
    
    private void InputEvents_OnUnitEnter(object sender, Unit enteredCharacter)
    {
        if (!unit.look.UnitsVisibleInFog().Contains(enteredCharacter))
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

    private void InputEvents_OnUnitExit(object sender, Unit leftCharacter)
    {
        OnExitCharacter(leftCharacter);
    }

    private void InputEvents_OnUnitClick(object sender, Unit clickedCharacter)
    {
        if (!unit.look.UnitsVisibleInFog().Contains(clickedCharacter))
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
