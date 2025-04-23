using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class C_AnimatorScripts : MonoBehaviour
{
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int Dodge = Animator.StringToHash("dodge");
    private static readonly int Death1 = Animator.StringToHash("death");

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
        anim.SetBool(Shoot, true);
    }

    /// <summary>
    /// End shoot animation.
    /// Called by animation.
    /// </summary>
    public void EndShoot()
    {
        anim.SetBool(Shoot, false);
        c.attack.EndAttack();
    }

    /// <summary>
    /// Start the hit reaction animation.
    /// </summary>
    public void StartHitReaction() => anim.SetBool(Hit, true);

    /// <summary>
    /// End the hit reaction animation.
    /// Called by animation.
    /// </summary>
    public void EndHitReaction() => anim.SetBool(Hit, false);

    /// <summary>
    /// Start the dodge reaction.
    /// </summary>
    public void StartDodge() => anim.SetTrigger(Dodge);

    /// <summary>
    /// Start the death animation.
    /// </summary>
    public void Death() => anim.SetBool(Death1, true);

    /// <summary>
    /// Enable or disable visuals of the characters.
    /// </summary>
    public void SetVisualActives(bool value) => visuals.ForEach(o => o.SetActive(value));

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
