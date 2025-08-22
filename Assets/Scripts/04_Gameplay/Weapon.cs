using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New weapon", menuName = "Basic Unity Tactical Tool/Weapon", order = 3)]
public class Weapon : ScriptableObject
{
    [SerializeField] private string weaponName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Vector2Int damagesRange = new Vector2Int(3, 5);
    [SerializeField] private List<DamageType> damageType;
    [SerializeField] private bool isMeleeWeapon;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns true if the weapon is a melee weapon, else returns false.
    /// </summary>
    /// <returns></returns>
    public bool IsMeleeWeapon() => isMeleeWeapon;

    /// <summary>
    /// Returns the weapon's name.
    /// </summary>
    /// <returns></returns>
    public string GetWeaponName() => weaponName;
    
    /// <summary>
    /// Returns the weapon's damage range.
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetDamagesRange() => damagesRange;
    
    /// <summary>
    /// Returns random damage in the weapon's damage range.
    /// </summary>
    /// <returns></returns>
    public int GetDamages() => Random.Range(damagesRange.x, damagesRange.y+1);
    
    /// <summary>
    /// Returns the weapon's damage types.
    /// </summary>
    /// <returns></returns>
    public List<DamageType> GetDamageTypes() => damageType;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
