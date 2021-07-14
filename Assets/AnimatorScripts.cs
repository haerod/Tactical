using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorScripts : MonoBehaviour
{
    [Header("REFERENCES")]

    public Character c;
    public Animator anim;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    public void StartShoot()
    {
        anim.SetBool("shoot", true);
    }

    // Called by animation
    public void EndShoot()
    {
        anim.SetBool("shoot", false);
        c.attack.EndAttack();
    }

    public void StartHitReaction()
    {
        anim.SetBool("hit", true);
    }

    // Called by animation
    public void EndHitReaction()
    {
        anim.SetBool("hit", false);
    }

    public void Death()
    {
        anim.SetBool("death", true);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
