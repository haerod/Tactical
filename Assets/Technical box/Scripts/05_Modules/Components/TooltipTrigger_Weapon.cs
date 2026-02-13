using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using static Utils;
using static M__Managers;

/// <summary>
/// Trigger to a tooltip's displayer for weapon's images.
/// /// </summary>
public class TooltipTrigger_Weapon : TooltipTrigger_Basic
{
    [SerializeField] private bool _showEquipCost = true;
    [SerializeField] private bool _showDamage = true;
    [SerializeField] private bool _showDamageTypes = true;
    [SerializeField] private bool _showPrecisionModifier = true;
    
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
        toReturn += $"{weapon.itemName}";
        
        // Equip cost
        if (_showEquipCost && _units.current.weaponHolder.weapon != weapon)
        {
            A_TakeWeapon actionTakeWeapon = _units.current.actionsHolder.GetActionOfType<A_TakeWeapon>();
            if(actionTakeWeapon.actionPointCost == 0)
                toReturn += SizedText($"\nEquip for free", 20);
            else
                toReturn += SizedText($"\nEquip for {actionTakeWeapon.actionPointCost} action", 20);
        }
        
        // Equipped
        if (_units.current.weaponHolder.weapon == weapon)
            toReturn += SizedText($"\nEquipped", 20);
        
        // Damage range
        if(_showDamage)
        {
            Vector2Int weaponDamageRange = weaponData.damagesRange;
            if (weaponDamageRange.x != weaponDamageRange.y)
                toReturn += "\n\n" + SizedText($"{weaponDamageRange.x} - {weaponDamageRange.y} damage", 28);
            else
                toReturn += "\n\n" + SizedText($"{weaponDamageRange.x} damage", 28);
        }
        
        // Damage types
        if(_showDamageTypes)
        {
            List<DamageType> damageTypes = weaponData.damageType;
            if (damageTypes.Count > 1)
            {
                toReturn += SizedText($" ({string.Join(", ", damageTypes)})", 28);
            }
            else if (damageTypes.Count == 1)
            {
                toReturn +=  SizedText($" ({damageTypes[0].name})", 28);
            }
        }

        // Precision modifier
        if (_showPrecisionModifier)
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