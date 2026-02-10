using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// Trigger to a tooltip's displayer for weapon's images.
/// /// </summary>
public class TooltipTrigger_Weapon : TooltipTrigger_Basic
{
    [SerializeField] private bool showActionPoints = false;
    [SerializeField] private bool showDamage = true;
    [SerializeField] private bool showDamageTypes = true;
    [SerializeField] private bool showPrecisionModifier = true;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the weapon's description
    /// </summary>
    /// <returns></returns>
    private string GetWeaponDescription()
    {
        string toReturn = "";
        
        Weapon weapon = GetComponentInParent<Module_WeaponSelectionButton>().weapon;
        WeaponData weaponData = weapon.data;
        
        // Name
        toReturn = $"<b>{weapon.itemName}</b>";
        // Action points
        if (showActionPoints)
            toReturn += $" ({weaponData.actionPointCost} AP)";
        
        // Damage range
        if(showDamage)
        {
            Vector2Int weaponDamageRange = weaponData.damagesRange;
            if (weaponDamageRange.x != weaponDamageRange.y)
                toReturn += $"\n{weaponDamageRange.x} - {weaponDamageRange.y} damage";
            else
                toReturn += $"\n {weaponDamageRange.x} damage";
        }
        
        // Damage types
        if(showDamageTypes)
        {
            List<DamageType> damageTypes = weaponData.damageType;
            if (damageTypes.Count > 1)
            {
                toReturn += "\n" + string.Join(", ", damageTypes);
            }
            else if (damageTypes.Count == 1)
            {
                toReturn += "\n" + damageTypes[0].name;
            }
        }

        // Precision modifier
        if (showPrecisionModifier)
        {
            int precisionModifier = weaponData.precisionModifier;
            if (precisionModifier != 0)
            {
                toReturn += "\n<i>";
                if (precisionModifier > 0)
                    toReturn += $"\n+{precisionModifier}% precision";
                else
                    toReturn += $"\n{precisionModifier}% precision";
            }
        }
        
        return toReturn;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyTooltipHovered(new List<string>() {GetWeaponDescription()});
    }
}