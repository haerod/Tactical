using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class FM_ActionEffectHolder : MonoBehaviour
{
    [SerializeField] private string missText = "MISS";
    
    [Header("REFERENCES")]
    [SerializeField] private GameObject actionEffectFeedbackPrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        U_Health.OnAnyHealthLoss += Health_OnAnyHealthLoss;
        U_Health.OnAnyHealthGain += Health_OnAnyHealthGain;
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
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingCharacter)
    {
        startingCharacter.attack.OnAttackMiss += Attack_OnAttackMiss;
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingCharacter)
    {
        endingCharacter.attack.OnAttackMiss -= Attack_OnAttackMiss;
    }
    
    private void Attack_OnAttackMiss(object sender, U__Unit missedCharacter)
    {
        DisplayActionEffectFeedback(missText, missedCharacter.transform);
    }
    
    private void Health_OnAnyHealthLoss(object sender, int healthLoss)
    {
        U_Health health = (U_Health) sender;
        Transform target = health.transform;
        
        DisplayActionEffectFeedback(healthLoss.ToString(), target);
    }
    
    private void Health_OnAnyHealthGain(object sender, int healthGain)
    {
        U_Health health = (U_Health) sender;
        Transform target = health.transform;
        
        DisplayActionEffectFeedback(healthGain.ToString(), target);
    }
}
