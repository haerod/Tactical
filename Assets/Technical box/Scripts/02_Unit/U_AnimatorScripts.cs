using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class U_AnimatorScripts : MonoBehaviour
{
    [Header("REFERENCES")]

    [SerializeField] private U__Unit unit;
    [SerializeField] private Animator anim;
    [SerializeField] private List<GameObject> visuals;

    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int Dodge = Animator.StringToHash("dodge");
    private static readonly int Death1 = Animator.StringToHash("death");
    private static readonly int Crouch = Animator.StringToHash("crouch");
    private static readonly int Aim = Animator.StringToHash("aim");
    private readonly int Speed = Animator.StringToHash("speed");
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        if(unit.cover.AreCoversAround())
            EnterCrouch();

        unit.move.OnMovementStart += Move_OnMovementStart;
        unit.move.OnMovementEnd += Move_OnMovementEnd;
        unit.attack.OnAttackStart += Attack_OnAttackStart;
        unit.attack.OnAttackableEnemyHovered += Attack_OnAttackableEnemyHovered;
        unit.attack.OnUnitExit += Attack_OnUnitExit;
        unit.health.OnDeath += Health_OnDeath;
    }
    
    private void OnDisable()
    {
        unit.move.OnMovementStart -= Move_OnMovementStart;
        unit.move.OnMovementEnd -= Move_OnMovementEnd;
        unit.attack.OnAttackStart -= Attack_OnAttackStart;
        unit.attack.OnAttackableEnemyHovered -= Attack_OnAttackableEnemyHovered;
        unit.attack.OnUnitExit -= Attack_OnUnitExit;
        unit.health.OnDeath -= Health_OnDeath;
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
        if(anim)
            anim.runtimeAnimatorController = weaponGraphics.GetWeaponAnimatorController();
    }
    
    /// <summary>
    /// Ends shoot animation.
    /// Called by animation.
    /// </summary>
    public void EndAttack()
    {
        anim.SetBool(Attack, false);
        anim.SetBool(Aim, false);
        unit.attack.EndAttack();
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
    /// Enables or disables visuals of the characters.
    /// </summary>
    public void SetVisualActives(bool value) => visuals.ForEach(o => o.SetActive(value));

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
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
    /// Ends the crouch animation.
    /// </summary>
    private void ExitCrouch() => anim.SetBool(Crouch, false);
    
    /// <summary>
    /// Starts the death animation.
    /// </summary>
    private void Death() => anim.SetBool(Death1, true);
    
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
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        StartAttack();
    }
    
    private void Attack_OnAttackableEnemyHovered(object sender, U__Unit enemy)
    {
        StartAim();
    }

    private void Attack_OnUnitExit(object sender, U__Unit exitedUnit)
    {
        StopAim();
    }

    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        Death();
    }
}
