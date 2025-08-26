using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class C_Health : MonoBehaviour
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

    [SerializeField] private C__Character c;

     public int currentHealth = 5;

     public static event EventHandler<int> OnAnyHealthLoss;
     public static event EventHandler<int> OnAnyHealthGain;
     
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
            c.anim.StartHitReaction();
        }

        OnAnyHealthLoss?.Invoke(this, damage);
        c.unitUI.UpdateHealthBar();
    }

    public void Heal(int healAmount, bool clampValue = true)
    {
        currentHealth += healAmount;
        
        if(!clampValue)
            return; // Don't clamp health value

        if (currentHealth > health)
            currentHealth = health;
        
        OnAnyHealthGain?.Invoke(this, healAmount);
        c.unitUI.UpdateHealthBar();
    }
    
    /// <summary>
    /// Returns true if the character has 0 health or less.
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
    /// Starts the death anim, informs M_Characters of the death and disables the life bar and the collider after a second.
    /// </summary>
    private void Death()
    {
        c.anim.Death();
        _characters.RemoveCharacter(c);
        _rules.RemoveCharacter(c);
        c.GetComponentInChildren<Collider>().gameObject.SetActive(false);
        Wait(1, () => { c.unitUI.Hide(); });
    }

    /// <summary>
    /// Starts a waits for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits for "time" seconds and executes an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
}