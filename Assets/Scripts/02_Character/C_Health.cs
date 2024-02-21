using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class C_Health : MonoBehaviour
{
    public int health = 5;

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
    /// Add "damages" damage to the health and check for death.
    /// </summary>
    /// <param name="damages"></param>
    public void AddDamages(int damages)
    {
        currentHealth -= damages;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
        else
        {
            c.anim.StartHitReaction();
        }

        _feedback.ActionEffectFeedback(damages.ToString(), transform.parent);
        c.healthBar.DisplayCurrentHealth();
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
    /// Start the death anim, inform M_Characters of the death and disable the life bar after a second
    /// </summary>
    private void Death()
    {
        c.anim.Death();
        _characters.RemoveDeadCharacter(c);
        Wait(1, () => { c.healthBar.SetLifeBarActive(false); });
    }

    /// <summary>
    /// Start a wait for "time" seconds and execute an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="OnEnd"></param>
    private void Wait(float time, Action OnEnd)
    {
        StartCoroutine(Wait_Co(time, OnEnd));
    }

    /// <summary>
    /// Wait for "time" seconds and execute an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="OnEnd"></param>
    /// <returns></returns>
    IEnumerator Wait_Co(float time, Action OnEnd)
    {
        yield return new WaitForSeconds(time);

        OnEnd();
    }
}
