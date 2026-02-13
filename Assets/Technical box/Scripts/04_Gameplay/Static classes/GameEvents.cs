using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages static events of non-singleton classes.
/// </summary>
public static class GameEvents
{
    public static event EventHandler<Unit> OnAnyActionStart;
    public static event EventHandler<Unit> OnAnyActionEnd;
    public static event EventHandler<Unit> OnAnyMovementStart;
    public static event EventHandler<Unit> OnAnyAttackStart;
    public static event EventHandler<Unit> OnAnyAttackEnd;
    public static event EventHandler<HealthChangedEventArgs> OnAnyHealthLoss;
    public static event EventHandler<HealthChangedEventArgs> OnAnyHealthGain;
    public static event EventHandler<Unit> OnAnyDeath;
    public static event EventHandler<Module_WeaponSelectionButton> OnAnyWeaponSelectionButtonEnter;
    public static event EventHandler<Module_WeaponSelectionButton> OnAnyWeaponSelectionButtonExit;
    public static event EventHandler<List<string>> OnAnyTooltipHovered;
    public static event EventHandler OnAnyTooltipExit;
    
    public class HealthChangedEventArgs : EventArgs
    {
        public Unit_Health health;
        public int healthChangedAmount;
        public List<DamageType> resistanceDamageTypes, weaknessDamageTypes;
    }
    
    public static void ClearAllEvents()
    {
        OnAnyAttackStart = null;
        OnAnyAttackEnd = null;
        OnAnyActionStart = null;
        OnAnyActionEnd = null;
        OnAnyMovementStart = null;
        OnAnyHealthLoss = null;
        OnAnyHealthGain = null;
        OnAnyDeath = null;
        OnAnyWeaponSelectionButtonEnter = null;
        OnAnyWeaponSelectionButtonExit = null;
        OnAnyTooltipHovered = null;
        OnAnyTooltipExit = null;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public static void InvokeOnAnyActionStart(Unit startingActionUnit) => OnAnyActionStart?.Invoke(null, startingActionUnit);
    public static void InvokeOnAnyActionEnd(Unit endingActionUnit) => OnAnyActionEnd?.Invoke(null, endingActionUnit);
    public static void InvokeOnAnyMovementStart(Unit movingUnit) => OnAnyMovementStart?.Invoke(null, movingUnit);
    public static void InvokeOnAnyAttackStart(Unit attackingUnit) => OnAnyAttackStart?.Invoke(null, attackingUnit);
    public static void InvokeOnAnyAttackEnd(Unit attackingUnit) => OnAnyAttackEnd?.Invoke(null, attackingUnit);
    public static void InvokeOnAnyHealthLoss(Unit_Health healthDamaged, int healthLoss, List<DamageType> resistanceDamageTypes, List<DamageType> weaknessDamageTypes) => 
        OnAnyHealthLoss?.Invoke(null, new HealthChangedEventArgs {
            health = healthDamaged, 
            healthChangedAmount = healthLoss,
            resistanceDamageTypes = resistanceDamageTypes,
            weaknessDamageTypes = weaknessDamageTypes });
    public static void InvokeOnAnyHealthGain(Unit_Health healthHealed, int healthGain) => OnAnyHealthGain?.Invoke(null, new HealthChangedEventArgs {
        health = healthHealed,
        healthChangedAmount = healthGain });
    public static void InvokeOnAnyDeath(Unit deadUnit) => OnAnyDeath?.Invoke(null, deadUnit);
    public static void InvokeOnAnyWeaponSelectionButtonEnter(Module_WeaponSelectionButton selectedButton) => OnAnyWeaponSelectionButtonEnter?.Invoke(null, selectedButton);
    public static void InvokeOnAnyWeaponSelectionButtonExit(Module_WeaponSelectionButton unselectedButton) => OnAnyWeaponSelectionButtonExit?.Invoke(null, unselectedButton);
    public static void InvokeOnAnyTooltipHovered(List<string> tooltips) => OnAnyTooltipHovered?.Invoke(null, tooltips);
    public static void InvokeOnAnyTooltipExit() => OnAnyTooltipExit?.Invoke(null, EventArgs.Empty);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================

}