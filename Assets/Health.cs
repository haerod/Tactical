using UnityEngine;
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
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void Death()
    {
        c.anim.Death();
        _characters.DeadCharacter(c);
    }
}
