using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New weapon data", menuName = "Basic Unity Tactical Tool/Weapon data", order = 3)]
public class WeaponData : ScriptableObject
{
    [SerializeField] private string _weaponName;
    public string weaponName => _weaponName;
    [SerializeField] private Sprite _icon;
    public Sprite icon => _icon;
    
    [Header("DAMAGES")]
    [SerializeField] private Vector2Int _damagesRange = new Vector2Int(3, 5);
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
    
    [Header("AMMO")]
    
    [SerializeField] private AmmoType _ammoType;
    public AmmoType ammoType => _ammoType;
    
    [SerializeField] private bool _needAmmoToReload;
    public bool needAmmoToReload => _needAmmoToReload;
    [SerializeField] private int _ammoCount;
    public int ammoCount => _ammoCount;
    
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
