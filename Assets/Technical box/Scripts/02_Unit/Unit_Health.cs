using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class Unit_Health : MonoBehaviour
{
    [SerializeField] private int _healthMax = 5;
    public int healthMax => _healthMax;
    public int currentHealth { get; private set; }
    
    [Header("- RESISTANCES AND WEAKNESSES -")][Space]
    
    [SerializeField] private ResistanceWeaknessBehavior increaseOrDecreaseBy;
    private enum ResistanceWeaknessBehavior {Amount, Percent, MultiplyOrDivide}
    
    [Space]
    
    [SerializeField] private int amountValue;
    [Range(0,100)][SerializeField] private int percentValue;
    [SerializeField] private int multiplyOrDivideValue;
    
    [Space]
    
    [SerializeField] private List<DamageType> resistances;
    [SerializeField] private List<DamageType> weaknesses;
    
    [Header("- REFERENCES -")][Space]

    [SerializeField] private Unit unit;
    
     public bool isFullHealth => currentHealth >= _healthMax;
     
     public event EventHandler OnHealthChanged;
     public event EventHandler OnDeath;
     public event EventHandler OnTryDamageApplied;
     public event EventHandler OnNonLethalDamageApplied;
     
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        currentHealth = _healthMax;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Adds damage amount to the health (triggering resistances and weaknesses) and checks for death.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageTypes"></param>
    public void AddDamages(int damage, List<DamageType> damageTypes)
    {
        OnTryDamageApplied?.Invoke(this, EventArgs.Empty);
        
        bool resistanceTriggered = damageTypes.Any(type => resistances.Contains(type));
        bool weaknessTriggered = damageTypes.Any(type => weaknesses.Contains(type));
        
        // Cancel effect if resistance and weaknesses are triggered
        if(resistanceTriggered && weaknessTriggered)
        {
            resistanceTriggered = false;
            weaknessTriggered = false;
        }
        
        if(resistanceTriggered)
        {
            damage = ApplyResistanceOrWeaknessToDamage(damage, true);
            
            GameEvents.InvokeOnAnyResistancesTriggered(this, damageTypes
                .Where(type => resistances.Contains(type))
                .ToList());
        }

        if(weaknessTriggered)
        {
            damage = ApplyResistanceOrWeaknessToDamage(damage, false);
            
            GameEvents.InvokeOnAnyWeaknessesTriggered(this, damageTypes
                .Where(type => weaknesses.Contains(type))
                .ToList());
        }
        
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, currentHealth);
        
        if (currentHealth == 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            GameEvents.InvokeOnAnyDeath(unit);
        }
        else
            OnNonLethalDamageApplied?.Invoke(this, EventArgs.Empty);
        
        GameEvents.InvokeOnAnyHealthLoss(this, damage);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Adds heal amount to health, clamped or not on the health value.
    /// </summary>
    /// <param name="healAmount"></param>
    /// <param name="clampValue"></param>
    public void Heal(int healAmount, bool clampValue = true)
    {
        currentHealth += healAmount;
        
        if(clampValue)
            currentHealth = Mathf.Clamp(currentHealth, 0, _healthMax);
        
        GameEvents.InvokeOnAnyHealthGain(this, healAmount);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the given damage depending on resistance or weakness.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="trueResistanceFalseWeakness"></param>
    /// <returns></returns>
    private int ApplyResistanceOrWeaknessToDamage(int damage, bool trueResistanceFalseWeakness)
    {
        if(trueResistanceFalseWeakness) // Resistance
        {
            switch (increaseOrDecreaseBy)
            {
                case ResistanceWeaknessBehavior.Amount:
                    return Mathf.Clamp(damage - amountValue, 0, damage);

                case ResistanceWeaknessBehavior.Percent:
                    float damagePercented = damage * (percentValue / 100f);
                    if (Mathf.Approximately(damagePercented % 1, .5f)) // Fix RoundToInt 0.5 native issue (check Unity API)
                        damagePercented += .1f;
                    return damage - Mathf.RoundToInt(damagePercented);

                case ResistanceWeaknessBehavior.MultiplyOrDivide:
                    float dividedDamage = (float) damage / multiplyOrDivideValue;
                    if (Mathf.Approximately(dividedDamage % 1, .5f)) // Fix RoundToInt 0.5 native issue (check Unity API)
                        dividedDamage += .1f;
                    return Mathf.RoundToInt(dividedDamage);

                default:
                    Debug.LogError("This kind of operation don't exist. Please add it.");
                    return damage;
            }
        }
        else // Weakness
        {
            switch (increaseOrDecreaseBy)
            {
                case ResistanceWeaknessBehavior.Amount:
                    return damage + amountValue;

                case ResistanceWeaknessBehavior.Percent:
                    float damagePercented = damage * (percentValue / 100f);
                    if (Mathf.Approximately(damagePercented % 1, .5f)) // Fix RoundToInt 0.5 native issue (check Unity API)
                        damagePercented += .1f;
                    return damage + Mathf.RoundToInt(damagePercented);

                case ResistanceWeaknessBehavior.MultiplyOrDivide:
                    return damage * multiplyOrDivideValue;

                default:
                    Debug.LogError("This kind of operation don't exist. Please add it.");
                    return damage;
            }
        }
    }
}