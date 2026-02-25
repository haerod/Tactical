using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_PowerDisplayer : MonoBehaviour
{
    [Header("- REFERENCES -")] [Space]
    
    [SerializeField] private GameObject _powerIconPrefab;
    [SerializeField] private Transform _parent;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    private void Awake()
    {
        DestroyAllButtons();
    }
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
    }
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Destroys the old buttons, creates new ones and gives them parameters.
    /// </summary>
    /// <param name="unit"></param>
    private void CreatePowerIcons(Unit unit)
    {
        DestroyAllButtons();

        foreach (Power power in unit.powersHolder.powers)
        {
            Module_PowerDisplayerIcon powerDisplayerIcon = Instantiate(_powerIconPrefab, _parent).GetComponent<Module_PowerDisplayerIcon>();
            powerDisplayerIcon.Initialize(power);
        }
    }
    
    /// <summary>
    /// Destroys all the buttons.
    /// </summary>
    private void DestroyAllButtons()
    {
        foreach (Transform child in _parent)
            Destroy(child.gameObject);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.CanPlay())
            return; // Can't play
        if(!startingUnit.behavior.playable)
            return; // Not playable character
        
        CreatePowerIcons(startingUnit);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        DestroyAllButtons();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit actingUnit)
    {
        DestroyAllButtons();
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        if(!endingActionUnit.behavior.playable)
            return; // Not playable character
        
        CreatePowerIcons(endingActionUnit);
    }
}