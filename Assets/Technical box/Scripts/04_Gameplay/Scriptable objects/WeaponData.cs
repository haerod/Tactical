using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New weapon data", menuName = "Basic Unity Tactical Tool/Weapon data", order = 3)]
public class WeaponData : ScriptableObject
{
    [Header("PROJECTILE")]
    [SerializeField] private bool _useProjectile;
    public bool useProjectile => _useProjectile;
    
    [Header("DAMAGES")]
    
    [SerializeField] private Vector2Int _damagesRange = new (3, 5);
    public Vector2Int damagesRange => _damagesRange;
    [SerializeField] private List<DamageType> _damageType;
    public List<DamageType> damageType => _damageType;
    
    [Header("RANGE")]
    
    [SerializeField] private bool _canAttackAnythingInView = true;
    public bool canAttackAnythingInView => _canAttackAnythingInView;
    [SerializeField] private bool _isMeleeWeapon;
    public bool isMeleeWeapon => _isMeleeWeapon;
    [SerializeField] private int _range;
    public int range => _range;
    
    [Header("PRECISION")]
    
    [SerializeField] private int _precisionMalusByDistance = 5;
    public int precisionMalusByDistance => _precisionMalusByDistance;
    [SerializeField] private int _precisionModifier;
    public int precisionModifier => _precisionModifier;
    
    [Header("AMMO")]
    
    [SerializeField] private Ammo _ammo;
    public Ammo ammo => _ammo;
    
    [SerializeField] private bool _canReload;
    public bool canReload => _canReload;
    [SerializeField] private bool _needAmmoToReload;
    public bool needAmmoToReload => _needAmmoToReload;
    [SerializeField] private int _ammoCount;
    public int ammoCount => _ammoCount;
    public bool usesAmmo => ammoType != null;
    public AmmoType ammoType => ammo == null ? null : ammo.ammoType;
    
    [Header("ACTION POINTS")]
    
    [SerializeField] private int _actionPointCost = 1;
    public int actionPointCost => _actionPointCost;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns random damage in the weapon's damage range.
    /// </summary>
    /// <returns></returns>
    public int RandomDamages() => Random.Range(_damagesRange.x, _damagesRange.y+1);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
