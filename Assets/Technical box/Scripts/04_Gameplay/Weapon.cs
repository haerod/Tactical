using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    [field:Header("WEAPON")]
    [field:SerializeField] public WeaponData data {get; private set;}
    
    [Header("CURRENT AMMO")]
    
    [SerializeField] private int _ammoCount;
    public int ammoCount => _ammoCount;
    public bool hasAvailableAmmoToSpend => !data.usesAmmo || (data.usesAmmo && _ammoCount > 0);
    public bool isFullOfAmmo => data.usesAmmo && ammoCount >= data.ammoCount;

    [Header("GRAPHICS")] 
    [SerializeField] private List<GameObject> _graphics;
    public List<GameObject> graphics => _graphics;
    [SerializeField] private Transform _weaponEnd;
    public Transform weaponEnd => _weaponEnd;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private RuntimeAnimatorController weaponAnimatorController;
    public RuntimeAnimatorController animatorController => weaponAnimatorController;
    
    private U__Unit unit;
    
    public event EventHandler<Weapon> OnAmmoCountChanged;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void OnDisable()
    {
        if(!unit)
            return;
        if(!unit.attack)
            return;
        
        unit.attack.OnAttackStart -= Attack_OnAttackStart;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Setup the parameters of the weapon.
    /// </summary>
    /// <param name="currentUnit"></param>
    public void Setup(U__Unit currentUnit)
    {
        unit = currentUnit;
        if(unit.attack)
            unit.attack.OnAttackStart += Attack_OnAttackStart;
    }
    
    /// <summary>
    /// Reloads the weapon of a given count of ammo.
    /// Returns the count of ammo used to reload.
    /// </summary>
    /// <param name="givenAmmo"></param>
    /// <returns></returns>
    public int ReloadWeaponOf(int givenAmmo)
    {
        if(!data.usesAmmo)
            return 0; // Don't use ammo
        
        int usedAmmo = AmmoCountToFullyReload();
        if(AmmoCountToFullyReload() > givenAmmo)
            usedAmmo = givenAmmo;
        
        _ammoCount += usedAmmo;
        OnAmmoCountChanged?.Invoke(this, this);
        
        return usedAmmo;
    }
    
    /// <summary>
    /// Returns how much ammo are needed to fully reload the weapon.
    /// </summary>
    /// <returns></returns>
    public int AmmoCountToFullyReload() => data.ammoCount - _ammoCount;
    
    /// <summary>
    /// Reduces ammo count by 1.
    /// </summary>
    public void SpendAmmo()
    {
        _ammoCount--;
        OnAmmoCountChanged?.Invoke(this, this);
    }

    /// <summary>
    /// Shows the weapon's graphics.
    /// </summary>
    public void ShowGraphics() => graphics.ForEach(graphic => graphic.SetActive(true));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables the muzzle flash during 0.2 seconds. 
    /// </summary>
    private void PlayMuzzleFlash()
    {
        if (!muzzleFlash) 
            return; // No muzzle flash
        
        muzzleFlash.SetActive(true);
        Wait(0.2f, () => muzzleFlash.SetActive(false));
    }
    
    /// <summary>
    /// Starts a wait for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits coroutine.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        if(muzzleFlash)
            PlayMuzzleFlash();
    }
}
