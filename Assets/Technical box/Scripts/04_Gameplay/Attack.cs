using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Attack class
/// </summary>
public class Attack
{
    public readonly Unit target;
    public readonly int damage;
    public readonly List<DamageType> damageTypes;
    
    // ======================================================================
    // INIT
    // ======================================================================
    
    public Attack(Unit target, int damage, List<DamageType> damageTypes)
    {
        this.target = target;
        this.damage = damage;
        this.damageTypes = damageTypes;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public void TriggerAttack() => AttackTarget();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private void AttackTarget()
    {
        target.health.AddDamages(damage, damageTypes);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
}