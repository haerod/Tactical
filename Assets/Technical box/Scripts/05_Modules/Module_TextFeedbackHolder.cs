using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static M__Managers;
using static Utils;

public class Module_TextFeedbackHolder : MonoBehaviour
{
    [SerializeField] private string missText = "MISS";
    [SerializeField] private string reloadText = "RELOAD";
    [SerializeField] private string outOfAmmoText = "OUT OF AMMO";
    
    [Space]
    
    [SerializeField] private Color _resistanceColor = Color.red;
    [SerializeField] private Color _weaknessColor = Color.yellow;
    [SerializeField] private Color _healColor = Color.green;
    
    [Header("- REFERENCES -")] [Space]
    
    [SerializeField] private GameObject actionEffectFeedbackPrefab;
    
    private Unit currentUnit;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyAttackEnd += GameEvents_OnAnyAttackEnd;
        GameEvents.OnAnyHealthLoss += Health_OnAnyHealthLoss;
        GameEvents.OnAnyHealthGain += Health_OnAnyHealthGain;
        InputEvents.OnUnitClick += InputEvents_OnUnitClick;
    }
    
    private void OnDisable()
    {
        if(!currentUnit)
            return;
        
        currentUnit.attack.OnAttackMiss -= Attack_OnAttackMiss;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Create an action effect feedback.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="referenceTarget"></param>
    private void DisplayActionEffectFeedback(string text, Transform referenceTarget)
    {
        UI_ActionEffectFeedback instantiatedActionEffectFeedback = Instantiate(actionEffectFeedbackPrefab, transform)
            .GetComponent<UI_ActionEffectFeedback>();
        instantiatedActionEffectFeedback.SetText(text);
        instantiatedActionEffectFeedback.PositionAt(referenceTarget);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        currentUnit = startingUnit;
        currentUnit.attack.OnAttackMiss += Attack_OnAttackMiss;
        A_Reload reload = currentUnit.actionsHolder.GetActionOfType<A_Reload>();
        if (reload)
            reload.OnReloadEnd += Reload_OnReloadEnd;
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        currentUnit.attack.OnAttackMiss -= Attack_OnAttackMiss;

        A_Reload reload = currentUnit.actionsHolder.GetActionOfType<A_Reload>();
        if (reload)
            reload.OnReloadEnd -= Reload_OnReloadEnd;
        currentUnit = null;
    }
    
    private void GameEvents_OnAnyAttackEnd(object sender, Unit endingUnit)
    {
        if (!endingUnit.behavior.playable)
            return; // NPC
        
        Weapon currentWeapon = endingUnit.weaponHolder.weapon;
        if(currentWeapon.data.usesAmmo && currentWeapon.currentLoadedAmmo == 0)
            DisplayActionEffectFeedback(outOfAmmoText, endingUnit.transform);    
    }
    
    private void Attack_OnAttackMiss(object sender, Unit missedUnit)
    {
        DisplayActionEffectFeedback(missText, missedUnit.transform);
    }
    
    private void Reload_OnReloadEnd(object sender, Unit reloadedUnit)
    {
        DisplayActionEffectFeedback(reloadText, reloadedUnit.transform);
    }
    
    private void Health_OnAnyHealthLoss(object sender, GameEvents.HealthChangedEventArgs args)
    {
        string _toDisplay = $"{args.healthChangedAmount.ToString()} damage";
        
        if(args.weaknessDamageTypes.Count > 0)
            _toDisplay = $"{ColoredText($"{_toDisplay}\n(Weakness)", _weaknessColor)}";
        else if(args.resistanceDamageTypes.Count > 0)
            _toDisplay = $"{ColoredText($"{_toDisplay}\n(Resistance)", _resistanceColor)}";
        
        DisplayActionEffectFeedback(
            _toDisplay, 
            args.health.transform);
    }
    
    private void Health_OnAnyHealthGain(object sender, GameEvents.HealthChangedEventArgs args)
    {
        string _toDisplay = args.healthChangedAmount > 0 ? 
            $"+{args.healthChangedAmount}HP healed" : 
            $"Full HP";
        
        DisplayActionEffectFeedback(ColoredText(_toDisplay, _healColor), args.health.transform);
    }
    
    private void InputEvents_OnUnitClick(object sender, Unit clickedUnit)
    {
        if(clickedUnit.team.IsAllyOf(currentUnit))
            return; // CLick on ally
        
        Weapon _weapon = currentUnit.weaponHolder.weapon;
        
        if(!_weapon.data.usesAmmo)
            return; // Weapon don't use ammo
        if(_weapon.hasAvailableAmmoToSpend)
            return; // Already has ammo
        
        DisplayActionEffectFeedback(outOfAmmoText, currentUnit.transform);
    }
}
