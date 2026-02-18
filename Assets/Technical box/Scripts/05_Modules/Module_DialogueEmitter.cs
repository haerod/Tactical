using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static Utils;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_DialogueEmitter : MonoBehaviour
{
    [SerializeField] private bool _showOnTurnStart = true;
    [SerializeField] private bool _showUnitName = true;
    [SerializeField] private float _delayBeforeShow;
    
    [Space]
    
    [TextArea][SerializeField] private string _dialogue;
    [SerializeField] private Transform _target;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private GameObject _dialogueTextPrefab;
    [SerializeField] private Unit _unit;
    
    private bool _showed;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        if(_showOnTurnStart)
            _units.OnUnitTurnStart += Units_OnUnitTurnStart;
    }
    
    private void OnDisable()
    {
        if(_showOnTurnStart)
            _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
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
        UI_ActionEffectFeedback instantiatedActionEffectFeedback = Instantiate(_dialogueTextPrefab)
            .GetComponentInChildren<UI_ActionEffectFeedback>();
        instantiatedActionEffectFeedback.SetText(text);
        instantiatedActionEffectFeedback.PositionAt(referenceTarget);
        Wait(Time.deltaTime, delegate {
            instantiatedActionEffectFeedback.transform.GetChild(0).gameObject.SetActive(true);
            instantiatedActionEffectFeedback.transform.GetChild(1).gameObject.SetActive(true); });
    }
    
    /// <summary>
    /// Starts a wait for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    protected void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));
    
    /// <summary>
    /// Waits coroutine.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(startingUnit != _unit) 
            return; // Not the good unit
        if (_showed)
            return; // Already showed
        
        Unit targetUnit = _target.GetComponent<Unit>();
        
        string toDisplay = _showUnitName ? 
            ColoredText($"<b>{targetUnit.unitName}</b>\n{_dialogue}", targetUnit.unitTeam.teamColor) : 
            _dialogue;
        
        Wait(_delayBeforeShow, () => DisplayActionEffectFeedback(toDisplay, _target));
        _showed = true;
    }
}