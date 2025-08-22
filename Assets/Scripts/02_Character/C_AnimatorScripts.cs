using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class C_AnimatorScripts : MonoBehaviour
{
    [Header("REFERENCES")]

    public C__Character c;

    [SerializeField] private Animator anim;
    [SerializeField] private List<GameObject> visuals;

    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int Dodge = Animator.StringToHash("dodge");
    private static readonly int Death1 = Animator.StringToHash("death");
    private static readonly int Crouch = Animator.StringToHash("crouch");
    private static readonly int IsMelee = Animator.StringToHash("isMelee");
    private static readonly int Aim = Animator.StringToHash("aim");
    private readonly int Speed = Animator.StringToHash("speed");
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        c.weaponHolder.OnWeaponChange += WeaponHolder_OnWeaponChange;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Add the good animator controller depending on the given weapon.
    /// </summary>
    /// <param name="weaponGraphics"></param>
    public void SetWeaponAnimation(WeaponGraphics weaponGraphics)
    {
        anim.runtimeAnimatorController = weaponGraphics.GetWeaponAnimatorController();
        anim.SetBool(IsMelee, weaponGraphics.GetWeapon().IsMeleeWeapon());
    }
    
    /// <summary>
    /// Sets the Speed animator's parameter.
    /// </summary>
    /// <param name="speedValue"></param>
    public void SetSpeed(float speedValue) => anim.SetFloat(Speed, speedValue);
    
    /// <summary>
    /// Starts attack animation.
    /// </summary>
    public void StartAttack() => anim.SetBool(Attack, true);

    /// <summary>
    /// Starts aim animation.
    /// </summary>
    public void StartAim() => anim.SetBool(Aim, true);
    
    /// <summary>
    /// Stops aim animation.
    /// </summary>
    public void StopAim() => anim.SetBool(Aim, false);
    
    /// <summary>
    /// Ends shoot animation.
    /// Called by animation.
    /// </summary>
    public void EndAttack()
    {
        anim.SetBool(Attack, false);
        anim.SetBool(Aim, false);
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
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void WeaponHolder_OnWeaponChange(object sender, WeaponGraphics weaponGraphics)
    {
        SetWeaponAnimation(weaponGraphics);
    }
}
