using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class C_AnimatorScripts : MonoBehaviour
{
    [Header("REFERENCES")]

    public C__Character c;
    public Animator anim;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Start shoot animation.
    /// </summary>
    public void StartShoot()
    {
        anim.SetBool("shoot", true);
    }

    /// <summary>
    /// End shoot animation.
    /// Called by animation
    /// </summary>
    public void EndShoot()
    {
        anim.SetBool("shoot", false);
        c.attack.EndAttack();
    }

    /// <summary>
    /// Start the hit reaction animation.
    /// </summary>
    public void StartHitReaction()
    {
        anim.SetBool("hit", true);
    }

    /// <summary>
    /// End the hit reaction animation.
    /// Called by animation
    /// </summary>
    public void EndHitReaction()
    {
        anim.SetBool("hit", false);
    }

    /// <summary>
    /// Start the death animation.
    /// </summary>
    public void Death()
    {
        anim.SetBool("death", true);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
