using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using static Utils;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_NameDisplayer : MonoBehaviour
{
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        
        Hide();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the unit's name.
    /// </summary>
    private void Show(Unit unit)
    {
        _nameText.text = unit.unitName;
        _panel.SetActive(true);
    }
    
    /// <summary>
    /// Hides the module.
    /// </summary>
    private void Hide() => _panel.SetActive(false);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyActionStart(object sender, Unit startingUnit)
    {
        Hide();
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingUnit)
    {
        if(!_units.current.behavior.playable)
            return; // NPC
        
        Show(endingUnit);
    }
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        Show(startingUnit);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        Hide();
    }
}