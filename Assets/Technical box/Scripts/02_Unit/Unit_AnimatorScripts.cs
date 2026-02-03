using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Unit_AnimatorScripts : MonoBehaviour
{
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private Unit unit;
    [SerializeField] private Animator anim;
    
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int Dodge = Animator.StringToHash("dodge");
    private static readonly int Death = Animator.StringToHash("death");
    private static readonly int Crouch = Animator.StringToHash("crouch");
    private static readonly int Aim = Animator.StringToHash("aim");
    private static readonly int Reload = Animator.StringToHash("reload");
    private readonly int Speed = Animator.StringToHash("speed");
    
    private A_Reload reload;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        if (unit.cover.AreCoversAround())
            StartCrouch();
        
        SetWeaponAnimation(unit.weaponHolder.weapon);
        
        unit.move.OnMovementStart += Move_OnMovementStart;
        unit.move.OnMovementEnd += Move_OnMovementEnd;
        unit.attack.OnAttackStart += Attack_OnAttackStart;
        unit.attack.OnAttackableEnemyHovered += Attack_OnAttackableEnemyHovered;
        unit.attack.OnUnitExit += Attack_OnUnitExit;
        unit.health.OnDeath += Health_OnDeath;
        unit.health.OnNonLethalDamageApplied += Health_OnNonLethalDamageApplied;
        unit.weaponHolder.OnWeaponChange += WeaponHolder_OnWeaponChange;
        
        reload = unit.actionsHolder.GetActionOfType<A_Reload>();
        if (reload)
            reload.OnReloadStart += Reload_OnReloadStart;
    }
    
    private void OnDisable()
    {
        unit.move.OnMovementStart -= Move_OnMovementStart;
        unit.move.OnMovementEnd -= Move_OnMovementEnd;
        unit.attack.OnAttackStart -= Attack_OnAttackStart;
        unit.attack.OnAttackableEnemyHovered -= Attack_OnAttackableEnemyHovered;
        unit.attack.OnUnitExit -= Attack_OnUnitExit;
        unit.health.OnDeath -= Health_OnDeath;
        unit.health.OnNonLethalDamageApplied -= Health_OnNonLethalDamageApplied;
        unit.weaponHolder.OnWeaponChange -= WeaponHolder_OnWeaponChange;
        
        if(reload)
            reload.OnReloadStart -= Reload_OnReloadStart;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Ends shoot animation.
    /// Called by animation.
    /// </summary>
    public void EndAttack()
    {
        anim.SetBool(Attack, false);
        anim.SetBool(Aim, false);
        unit.actionsHolder.GetActionOfType<A_Attack>().ExecuteAttack();
    }
    
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
    /// Ends reloading.
    /// Called by animation.
    /// </summary>
    public void EndReload()
    {
        anim.SetBool(Reload, false);
        unit.actionsHolder.GetActionOfType<A_Reload>().EndReload();
    }
    
    // public void SetReload(bool value) => anim.SetBool(Reload, value);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Add the good animator controller depending on the given weapon.
    /// </summary>
    /// <param name="weapon"></param>
    private void SetWeaponAnimation(Weapon weapon) => anim.runtimeAnimatorController = weapon.animatorController;
    
    /// <summary>
    /// Sets the Speed animator's parameter.
    /// </summary>
    /// <param name="speedValue"></param>
    private void SetSpeed(float speedValue) => anim.SetFloat(Speed, speedValue);
    
    /// <summary>
    /// Starts attack animation.
    /// </summary>
    private void StartAttack() => anim.SetBool(Attack, true);
    
    /// <summary>
    /// Starts aim animation.
    /// </summary>
    private void StartAim() => anim.SetBool(Aim, true);
    
    /// <summary>
    /// Stops aim animation.
    /// </summary>
    private void StopAim() => anim.SetBool(Aim, false);
    
    /// <summary>
    /// Starts the hit reaction animation.
    /// </summary>
    private void StartHitReaction() => anim.SetBool(Hit, true);
    
    /// <summary>
    /// Ends the crouch animation.
    /// </summary>
    private void ExitCrouch() => anim.SetBool(Crouch, false);
    
    /// <summary>
    /// Starts the death animation.
    /// </summary>
    private void StartDeath() => anim.SetBool(Death, true);

    private void StartReload() => anim.SetBool(Reload, true);
    
    /// <summary>
    /// Starts the crouch animation.
    /// </summary>
    private void StartCrouch() => anim.SetBool(Crouch, true);

    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        ExitCrouch();
        SetSpeed(1);
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        SetSpeed(0);
        
        if(unit.cover.AreCoversAround())
            StartCrouch();
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        StartAttack();
    }
    
    private void Attack_OnAttackableEnemyHovered(object sender, Unit enemy)
    {
        StartAim();
    }
    
    private void Attack_OnUnitExit(object sender, Unit exitedUnit)
    {
        StopAim();
    }
    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        StartDeath();
    }
    
    private void Health_OnNonLethalDamageApplied(object sender, EventArgs e)
    {
        StartHitReaction();
    }
    
    private void WeaponHolder_OnWeaponChange(object sender, Unit_WeaponHolder.WeaponChangeEventArgs args)
    {
        SetWeaponAnimation(args.newWeapon);
    }
    
    private void Reload_OnReloadStart(object sender, EventArgs e)
    {
        StartReload();
    }
}
