using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.Text;
using static M__Managers;

/// <summary>
/// FM_AttackLineOfSight : Shows and hide line of sight feedbacks in world
/// </summary>
public class FM_AttackLineOfSight : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private LineRenderer lineRenderer;
    
    private bool isShowing;
    private U__Unit attacker, target;
    
    private U__Unit currentUnit;
    
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
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd -= Units_OnUnitTurnEnd;
        
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
        
        if(currentUnit)
            currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
    }

    private void Update()
    {
        if(!isShowing)
            return; // Not showing
        
        Vector3 weaponEndTransform = attacker.weaponHolder.GetCurrentWeaponGraphics().GetWeaponEnd().position;
        
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

    private void Show(U__Unit newAttacker, U__Unit newTarget)
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

    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        currentUnit = startingUnit;
        
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
    }

    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        
        currentUnit = null;
    }

    private void InputEvents_OnEnemyEnter(object sender, U__Unit enteredUnit)
    {
        U__Unit current = _units.current;
        
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