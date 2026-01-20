using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.Text;
using static M__Managers;

/// <summary>
/// Shows and hides line of sight feedbacks in world
/// </summary>
public class Module_AttackLineOfSight : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private LineRenderer lineRenderer;
    
    private bool isShowing;
    private Unit attacker, target;
    
    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
    }

    private void OnDisable()
    {
        if(currentUnit)
            currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
    }

    private void Update()
    {
        if(!isShowing)
            return; // Not showing
        
        Vector3 weaponEndTransform = attacker.weaponHolder.weaponGraphics.weaponEnd.position;
        
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.SetPosition(0, weaponEndTransform);
        lineRenderer.SetPosition(1, target.transform.position + Vector3.up * weaponEndTransform.y);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void Show(Unit newAttacker, Unit newTarget)
    {
        isShowing = true;
        attacker = newAttacker;
        target = newTarget;
    }

    private void Hide()
    {
        isShowing = false;
        lineRenderer.gameObject.SetActive(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        currentUnit = startingUnit;
        
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
    }

    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        
        currentUnit = null;
    }

    private void InputEvents_OnEnemyEnter(object sender, Unit enteredUnit)
    {
        Unit current = _units.current;
        
        if(!current.CanPlay())
            return; // Can't play
        if(!enteredUnit.team.IsEnemyOf(current))
            return; // Not an enemy
        if(!current.attack.AttackableTiles().Contains(enteredUnit.tile))
            return; // Can't attack this character
        
        
        Show(current, enteredUnit);
    }
    
    private void InputEvents_OnTileExit(object sender, Tile exitedTile)
    {
        Hide();
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        Hide();
    }
}