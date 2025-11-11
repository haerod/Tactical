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
    public static event EventHandler<U__Unit> OnAnyActionStart;
    public static event EventHandler<U__Unit> OnAnyActionEnd;
    public static event EventHandler<U__Unit> OnAnyMovementStart;
    public static event EventHandler<U__Unit> OnAnyAttackStart;
    public static event EventHandler<HealthChangedEventArgs> OnAnyHealthLoss;
    public static event EventHandler<HealthChangedEventArgs> OnAnyHealthGain;
    public static event EventHandler<U__Unit> OnAnyDeath;
    public static event EventHandler<Module_WeaponSelectionButton> OnAnyWeaponSelectionButtonEnter;
    public static event EventHandler<Module_WeaponSelectionButton> OnAnyWeaponSelectionButtonExit;
    public static event EventHandler<List<string>> OnAnyTooltipHovered;
    public static event EventHandler OnAnyTooltipExit;
    
    public class HealthChangedEventArgs : EventArgs
    {
        public U_Health health;
        public int healthChangedAmount;
    }
    
    public static void ClearAllEvents()
    {
        OnAnyAttackStart = null;
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

    public static void InvokeOnAnyActionStart(U__Unit startingActionUnit) => OnAnyActionStart?.Invoke(null, startingActionUnit);
    public static void InvokeOnAnyActionEnd(U__Unit endingActionUnit) => OnAnyActionEnd?.Invoke(null, endingActionUnit);
    public static void InvokeOnAnyMovementStart(U__Unit movingUnit) => OnAnyMovementStart?.Invoke(null, movingUnit);
    public static void InvokeOnAnyAttackStart(U__Unit attackingUnit) => OnAnyAttackStart?.Invoke(null, attackingUnit);
    public static void InvokeOnAnyHealthLoss(U_Health healthDamaged, int healthLoss) => OnAnyHealthLoss?.Invoke(null, new HealthChangedEventArgs {
        health = healthDamaged, 
        healthChangedAmount = healthLoss});
    public static void InvokeOnAnyHealthGain(U_Health healthHealed, int healthGain) => OnAnyHealthGain?.Invoke(null, new HealthChangedEventArgs {
        health = healthHealed,
        healthChangedAmount = healthGain });
    public static void InvokeOnAnyDeath(U__Unit deadUnit) => OnAnyDeath?.Invoke(null, deadUnit);
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