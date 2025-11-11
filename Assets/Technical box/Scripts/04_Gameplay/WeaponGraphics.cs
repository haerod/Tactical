using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponGraphics : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform weaponEnd;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private RuntimeAnimatorController weaponAnimatorController;

    private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================

    private void OnDisable()
    {
        if(!unit.attack)
            return;
        
        unit.attack.OnAttackStart -= Attack_OnAttackStart;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void Setup(U__Unit currentUnit)
    {
        unit = currentUnit;
        if(unit.attack)
            unit.attack.OnAttackStart += Attack_OnAttackStart;
    }

    /// <summary>
    /// Returns the animator controller corresponding to the weapon.
    /// </summary>
    /// <returns></returns>
    public RuntimeAnimatorController GetWeaponAnimatorController() => weaponAnimatorController;

    /// <summary>
    /// Returns the current weapon.
    /// </summary>
    /// <returns></returns>
    public Weapon GetWeapon() => weapon;

    /// <summary>
    /// Returns the end of the weapon (ex: to add line of sight). 
    /// </summary>
    /// <returns></returns>
    public Transform GetWeaponEnd() => weaponEnd;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

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
