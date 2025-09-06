using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class U_Health : MonoBehaviour
{
    public int health = 5;

    [Header("RESISTANCES AND WEAKNESSES")]
    [Space]
    [SerializeField] private ResistanceWeaknessBehavior increaseOrDecreaseBy;
    private enum ResistanceWeaknessBehavior {Amount, Percent, MultiplyOrDivide}
    [Space]
    [SerializeField] private int amountValue;
    [Range(0,100)]
    [SerializeField] private int percentValue;
    [SerializeField] private int multiplyOrDivideValue;
    [Space]
    [SerializeField] private List<DamageType> resistances;
    [SerializeField] private List<DamageType> weaknesses;
    
    [Header("REFERENCES")]

    [SerializeField] private U__Unit unit;

     public int currentHealth = 5;

     public event EventHandler HealthChanged;
     public static event EventHandler<int> OnAnyHealthLoss;
     public static event EventHandler<int> OnAnyHealthGain;
     public event EventHandler OnDeath;
     public static event EventHandler OnAnyDeath;
     
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
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
        bool resistanceTriggered = false;
        bool weaknessTriggered = false;

        foreach (DamageType testedDamageType in damageTypes)
        {
            if (resistances.Contains(testedDamageType))
                resistanceTriggered = true;
            if (weaknesses.Contains(testedDamageType))
                weaknessTriggered = true;
        }

        if(resistanceTriggered && weaknessTriggered)
        {
            resistanceTriggered = false;
            weaknessTriggered = false;
        }

        if(resistanceTriggered)
        {
            switch (increaseOrDecreaseBy)
            {
                case ResistanceWeaknessBehavior.Amount:
                    damage = Mathf.Clamp(damage - amountValue, 0, damage);
                    break;

                case ResistanceWeaknessBehavior.Percent:
                    float damagePercented = damage * (percentValue / 100f);

                    if (Mathf.Approximately(damagePercented % 1, .5f)) // Fix RoundToInt 0.5 native issue (check Unity API)
                        damagePercented += .1f;

                    damage -= Mathf.RoundToInt(damagePercented);
                    break;

                case ResistanceWeaknessBehavior.MultiplyOrDivide:
                    float dividedDamage = (float) damage / multiplyOrDivideValue;

                    if (Mathf.Approximately(dividedDamage % 1, .5f)) // Fix RoundToInt 0.5 native issue (check Unity API)
                        dividedDamage += .1f;

                    damage = Mathf.RoundToInt(dividedDamage);
                    break;

                default:
                    Debug.LogError("This kind of operation don't exist. Please add it.");
                    break;
            }
        }

        if(weaknessTriggered)
        {
            switch (increaseOrDecreaseBy)
            {
                case ResistanceWeaknessBehavior.Amount:
                    damage += amountValue;
                    break;

                case ResistanceWeaknessBehavior.Percent:
                    float damagePercented = damage * (percentValue / 100f);

                    if (Mathf.Approximately(damagePercented % 1, .5f)) // Fix RoundToInt 0.5 native issue (check Unity API)
                        damagePercented += .1f;

                    damage += Mathf.RoundToInt(damagePercented);
                    break;

                case ResistanceWeaknessBehavior.MultiplyOrDivide:
                    damage *= multiplyOrDivideValue;
                    break;

                default:
                    Debug.LogError("This kind of operation don't exist. Please add it.");
                    break;
            }
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
        else
        {
            unit.anim.StartHitReaction();
        }

        OnAnyHealthLoss?.Invoke(this, damage);
        HealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int healAmount, bool clampValue = true)
    {
        currentHealth += healAmount;
        
        if(!clampValue)
            return; // Don't clamp health value

        if (currentHealth > health)
            currentHealth = health;
        
        OnAnyHealthGain?.Invoke(this, healAmount);
        HealthChanged?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Returns true if the unit has 0 health or less.
    /// </summary>
    /// <returns></returns>
    public bool IsDead() => currentHealth <= 0;

    /// <summary>
    /// Returns true if current health is at maximum value.
    /// </summary>
    /// <returns></returns>
    public bool IsFullLife() => currentHealth == health;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Starts the death anim and disables the life bar and the collider after a second.
    /// </summary>
    private void Death()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        OnAnyDeath?.Invoke(this, EventArgs.Empty);
        unit.anim.Death();
        _units.RemoveUnit(unit);
        unit.GetComponentInChildren<Collider>().gameObject.SetActive(false);
    }
}