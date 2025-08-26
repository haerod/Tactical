using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class UI_ActionEffectHolder : MonoBehaviour
{
    [SerializeField] private string missText = "MISS";
    
    [Header("REFERENCES")]
    [SerializeField] private GameObject actionEffectFeedbackPrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        A_Attack.OnAttackMiss += Attack_OnAttackMiss;
        C_Health.OnAnyHealthLoss += Health_OnAnyHealthLoss;
        C_Health.OnAnyHealthGain += Health_OnAnyHealthGain;
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
    
    private void Attack_OnAttackMiss(object sender, C__Character missedCharacter)
    {
        DisplayActionEffectFeedback(missText, missedCharacter.transform);
    }
    
    private void Health_OnAnyHealthLoss(object sender, int healthLoss)
    {
        C_Health health = (C_Health) sender;
        Transform target = health.transform;
        
        DisplayActionEffectFeedback(healthLoss.ToString(), target);
    }
    
    private void Health_OnAnyHealthGain(object sender, int healthGain)
    {
        C_Health health = (C_Health) sender;
        Transform target = health.transform;
        
        DisplayActionEffectFeedback(healthGain.ToString(), target);
    }
}
