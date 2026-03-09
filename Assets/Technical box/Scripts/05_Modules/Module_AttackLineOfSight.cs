using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using static M__Managers;

/// <summary>
/// Shows and hides line of sight feedbacks in world
/// </summary>
public class Module_AttackLineOfSight : MonoBehaviour
{
    [SerializeField] private bool _showOnMeleeWeapons = false;
    [SerializeField] private bool _showIfUntouchableTarget = false;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private LineRenderer _lineRenderer;
    
    private bool _isShowing;
    private Unit _attacker, _target;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        
        _lineRenderer.useWorldSpace = true;
    }
    
    private void Update()
    {
        if(!_isShowing)
            return; // Not showing
        
        Vector3 weaponEndTransform = _attacker.weaponHolder.weaponGraphics.weaponEnd.position;
        
        _lineRenderer.gameObject.SetActive(true);
        _lineRenderer.SetPosition(0, weaponEndTransform);
        _lineRenderer.SetPosition(1, _target.transform.position + Vector3.up * weaponEndTransform.y);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Displays the line feedback.
    /// </summary>
    /// <param name="newAttacker"></param>
    /// <param name="newTarget"></param>
    private void Show(Unit newAttacker, Unit newTarget)
    {
        if(!_showOnMeleeWeapons && newAttacker.weaponHolder.weaponData.isMeleeWeapon)
        {
            Hide();
            return;
        }
        
        if(!_showIfUntouchableTarget && newAttacker.attack.GetChanceToTouch(newTarget) <= 0)
        {
            Hide();
            return;
        }
        
        _isShowing = true;
        _attacker = newAttacker;
        _target = newTarget;
    }

    /// <summary>
    /// Hides the line feedback.
    /// </summary>
    private void Hide()
    {
        _isShowing = false;
        _lineRenderer.gameObject.SetActive(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
    }
    
    private void InputEvents_OnEnemyEnter(object sender, Unit enteredUnit)
    {
        Unit current = _units.current;
        
        if(!current.CanPlay())
            return; // Can't play
        if(!enteredUnit.team.IsEnemyOf(current))
            return; // Not an enemy
        if(!current.actionsHolder.HasAvailableAction<A_Attack>())
            return; // No available attack action
        if(!current.attack.AttackableTiles().Contains(enteredUnit.tile))
            return; // Can't attack this character
        
        Show(current, enteredUnit);
    }
    
    private void InputEvents_OnTileExit(object sender, Tile exitedTile)
    {
        Hide();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit actingUnit)
    {
        Hide();
    }
}