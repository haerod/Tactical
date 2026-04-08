using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static M__Managers;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Module_UnitDamageDisplayer : MonoBehaviour
{
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Unit _unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnUnitExit += InputEvents_OnUnitExit;
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables feedback.
    /// </summary>
    private void Show ()
    {
        Weapon equippedWeapon = _unit.weaponHolder.weapon;

        if (!equippedWeapon)
        {
            _text.gameObject.SetActive(false);
            return; // No equipped weapon
        }
        
        _text.text = $"{equippedWeapon.data.damagesRange.x} damage";

        if (equippedWeapon.data.usesAmmo && !equippedWeapon.hasAvailableAmmoToSpend)
            _text.text += $" (empty)";

        _text.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Disables feedback.
    /// </summary>
    private void Hide() => _text.gameObject.SetActive(false);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
    
    private void InputEvents_OnUnitEnter(object sender, Unit hoveredUnit)
    {
        Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(hoveredUnit != _unit)
            return; // Another unit
        if(!currentUnit.look.UnitsVisibleInFog().Contains(_unit))
            return; // Invisible unit
        if(currentUnit.team.IsTeammateOf(_unit))
            return; // Teammate
        
        Show();
    }
    
    private void InputEvents_OnUnitExit(object sender, Unit exitedUnit)
    {
        if(exitedUnit == _unit)
            Hide();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit startingActionUnit)
    {
        Hide();
    }
}