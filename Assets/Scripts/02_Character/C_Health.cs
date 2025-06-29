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

    [SerializeField] private C__Character c = null;

    [HideInInspector] public int currentHealth = 5;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        currentHealth = health;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Add damage amount to the health (triggering resistances and weaknesses) and check for death.
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

        _feedback.ActionEffectFeedback(damage.ToString(), transform.parent);
        c.unitUI.UpdateHealthBar();
    }

    /// <summary>
    /// Return true if the character has 0 health or less.
    /// </summary>
    /// <returns></returns>
    public bool IsDead() => currentHealth <= 0;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Start the death anim, inform M_Characters of the death and disable the life bar after a second and the collider
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
    /// Start a wait for "time" seconds and execute an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Wait for "time" seconds and execute an action.
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