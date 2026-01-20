using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class Module_ActionEffectHolder : MonoBehaviour
{
    [SerializeField] private string missText = "MISS";
    
    [Header("REFERENCES")]
    [SerializeField] private GameObject actionEffectFeedbackPrefab;
    
    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyHealthLoss += Health_OnAnyHealthLoss;
        GameEvents.OnAnyHealthGain += Health_OnAnyHealthGain;
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
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        currentUnit.attack.OnAttackMiss -= Attack_OnAttackMiss;
        currentUnit = null;
    }
    
    private void Attack_OnAttackMiss(object sender, Unit missedUnit)
    {
        DisplayActionEffectFeedback(missText, missedUnit.transform);
    }
    
    private void Health_OnAnyHealthLoss(object sender, GameEvents.HealthChangedEventArgs args)
    {
        DisplayActionEffectFeedback(
            args.healthChangedAmount.ToString(), 
            args.health.transform);
    }
    
    private void Health_OnAnyHealthGain(object sender, GameEvents.HealthChangedEventArgs args)
    {
        DisplayActionEffectFeedback(
            args.healthChangedAmount.ToString(), 
            args.health.transform);
    }
}
