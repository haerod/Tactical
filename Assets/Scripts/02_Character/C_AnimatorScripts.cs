using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class C_AnimatorScripts : MonoBehaviour
{
    [Header("REFERENCES")]

    public C__Character c;

    [SerializeField] private Animator anim = null;
    [SerializeField] private List<GameObject> visuals = null;

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
    /// Called by animation.
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
    /// Called by animation.
    /// </summary>
    public void EndHitReaction()
    {
        anim.SetBool("hit", false);
    }

    /// <summary>
    /// Start the dodge reaction.
    /// </summary>
    public void StartDodge()
    {
        anim.SetTrigger("dodge");
    }

    /// <summary>
    /// Start the death animation.
    /// </summary>
    public void Death()
    {
        anim.SetBool("death", true);
    }

    /// <summary>
    /// Enable or disable visuals of the characters.
    /// </summary>
    public void SetVisualActives(bool value)
    {
        visuals
            .ForEach(o => o.SetActive(value));
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
