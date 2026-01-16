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
public class Module_TooltipTigger_Weapon : Module_TooltipTriggerBase
{
    [SerializeField] private bool showDamage = true;
    [SerializeField] private bool showDamageTypes = true;
    [SerializeField] private bool showPrecisionModifier = true;
    
    private WeaponData weaponData;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private string GetWeaponDescription()
    {
        string toReturn = "";
        
        WeaponData weaponData = GetComponent<Module_WeaponSelectionButton>().GetWeapon();
        
        // Name
        toReturn = $"<b>{weaponData.weaponName}</b>";
        
        // Damage range
        if(showDamage)
        {
            Vector2Int weaponDamageRange = weaponData.damagesRange;
            if (weaponDamageRange.x != weaponDamageRange.y)
                toReturn += $"\n{weaponDamageRange.x} - {weaponDamageRange.y}";
            else
                toReturn += "\n" + weaponDamageRange.x.ToString();
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