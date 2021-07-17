using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class Health : MonoBehaviour
{
    public int health = 10;
    public int maxHealth = 10;

    [Header("REFERENCES")]

    [SerializeField] private Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void AddDamages(int damages, Character attacker)
    {
        health -= damages;

        if (health <= 0)
        {
            health = 0;
            Death();
        }
        else
        {
            c.anim.StartHitReaction();
        }

        _feedbacks.ActionEffectFeedback(damages.ToString(), transform.parent);
        c.healthBar.DisplayCurrentLife();
    }

    public bool IsDead() => health <= 0;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void Death()
    {
        c.anim.Death();
        _characters.DeadCharacter(c);
        Wait(1, () => { c.healthBar.SetLifeBarActive(false); });
    }

    private void Wait(float time, Action OnEnd)
    {
        StartCoroutine(Wait_Co(time, OnEnd));
    }

    IEnumerator Wait_Co(float time, Action OnEnd)
    {
        yield return new WaitForSeconds(time);

        OnEnd();
    }
}
