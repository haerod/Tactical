using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New weapon", menuName = "Basic Unity Tactical Tool/Weapon", order = 3)]
public class Weapon : ScriptableObject
{
    [SerializeField] private string weaponName;
    [SerializeField] private Sprite icon;
    
    [Header("DAMAGES")]
    [SerializeField] private Vector2Int damagesRange = new Vector2Int(3, 5);
    [SerializeField] private List<DamageType> damageType;
    
    [Header("RANGE")]
    [SerializeField] private bool anythingInView = true;
    [SerializeField] private bool isMeleeWeapon;
    [SerializeField] private int specificRange;
    
    [Header("PRECISION")]
    [SerializeField] private int precisionMalusByDistance = 5;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the malus of precision by tile of distance with the target.
    /// </summary>
    /// <returns></returns>
    public int GetPrecisionMalusByDistance() => precisionMalusByDistance;
    
    /// <summary>
    /// Returns true if the weapon can touch anything in view (depending on the obstacles).
    /// </summary>
    public bool GetTouchInView() => anythingInView;

    /// <summary>
    /// Returns the range of the weapon.
    /// </summary>
    /// <returns></returns>
    public int GetRange() => specificRange;
    
    /// <summary>
    /// Returns true if the weapon is a melee weapon, else returns false.
    /// </summary>
    /// <returns></returns>
    public bool IsMeleeWeapon() => isMeleeWeapon;

    /// <summary>
    /// Returns the weapon's name.
    /// </summary>
    /// <returns></returns>
    public string GetName() => weaponName;
    
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
    
    /// <summary>
    /// Returns the weapon's icon.
    /// </summary>
    /// <returns></returns>
    public Sprite GetIcon() => icon;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
