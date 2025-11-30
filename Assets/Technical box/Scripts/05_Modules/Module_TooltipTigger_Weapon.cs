using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// Trigger to a tooltip's displayer for weapon's images.
/// /// </summary>
public class Module_TooltipTigger_Weapon : Module_TooltipTriggerBase
{
    [SerializeField] private bool showDamage = true;
    [SerializeField] private bool showDamageTypes = true;
    
    private Weapon weapon;
    
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
        
        Weapon weapon = GetComponent<Module_WeaponSelectionButton>().GetWeapon();
        
        // Name
        toReturn = $"<b>{weapon.GetName()}</b>";
        
        // Damage range
        if(showDamage)
        {
            Vector2Int weaponDamageRange = weapon.GetDamagesRange();
            if (weaponDamageRange.x != weaponDamageRange.y)
                toReturn += $"\n{weaponDamageRange.x} - {weaponDamageRange.y}";
            else
                toReturn += "\n" + weaponDamageRange.x.ToString();
        }
        
        // Damage types
        if(showDamageTypes)
        {
            List<DamageType> damageTypes = weapon.GetDamageTypes();
            if (damageTypes.Count > 1)
            {
                toReturn += "\n" + string.Join(", ", damageTypes);
            }
            else if (damageTypes.Count == 1)
            {
                toReturn += "\n" + damageTypes[0].name;
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