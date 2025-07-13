using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class C_AnimatorScripts : MonoBehaviour
{
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int Dodge = Animator.StringToHash("dodge");
    private static readonly int Death1 = Animator.StringToHash("death");
    private static readonly int Crouch = Animator.StringToHash("crouch");
    private static readonly int IsMelee = Animator.StringToHash("isMelee");

    [Header("REFERENCES")]

    public C__Character c;

    [SerializeField] private Animator anim = null;
    [SerializeField] private List<GameObject> visuals = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        anim.runtimeAnimatorController = c.weaponHolder.GetCurrentWeaponGraphics().GetWeaponAnimatorController();
        anim.SetBool(IsMelee, c.attack.GetCurrentWeapon().IsMeleeWeapon());
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Starts shoot animation.
    /// </summary>
    public void StartShoot() => anim.SetBool(Shoot, true);

    /// <summary>
    /// Ends shoot animation.
    /// Called by animation.
    /// </summary>
    public void EndShoot()
    {
        anim.SetBool(Shoot, false);
        c.attack.EndAttack();
    }

    /// <summary>
    /// Starts the hit reaction animation.
    /// </summary>
    public void StartHitReaction() => anim.SetBool(Hit, true);

    /// <summary>
    /// Ends the hit reaction animation.
    /// Called by animation.
    /// </summary>
    public void EndHitReaction() => anim.SetBool(Hit, false);

    /// <summary>
    /// Starts the dodge reaction.
    /// </summary>
    public void StartDodge() => anim.SetTrigger(Dodge);

    /// <summary>
    /// Starts the crouch animation.
    /// </summary>
    public void EnterCrouch() => anim.SetBool(Crouch, true);
    
    /// <summary>
    /// Ends the crouch animation.
    /// </summary>
    public void ExitCrouch() => anim.SetBool(Crouch, false);
    
    /// <summary>
    /// Starts the death animation.
    /// </summary>
    public void Death() => anim.SetBool(Death1, true);

    /// <summary>
    /// Enables or disables visuals of the characters.
    /// </summary>
    public void SetVisualActives(bool value) => visuals.ForEach(o => o.SetActive(value));

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
