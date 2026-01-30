using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class description
/// </summary>
public class Ammo : Item
{
    [Header("AMMO")]
    
    [SerializeField] private AmmoType _ammoType;
    public AmmoType ammoType => _ammoType;
    
    [Header("GRAPHICS")]
    
    [SerializeField] private List<GameObject> _graphics;
    public List<GameObject> graphics => _graphics;

    [Header("PROJECTILE")]
    
    [SerializeField] private float speed;
    [SerializeField] private bool destroyOnHit;
    [SerializeField] private bool becomeChildOnHit;
    
    public event EventHandler OnProjectileHit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables or disable graphics.
    /// </summary>
    /// <param name="value"></param>
    public void SetGraphicsActive(bool value) => graphics.ForEach(g => g.SetActive(value));
    
    public void GoTo(Transform target) => StartCoroutine(GoTo_Coroutine(target));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private IEnumerator GoTo_Coroutine(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        
        OnProjectileHit?.Invoke(this, EventArgs.Empty);
        
        if(destroyOnHit)
            Destroy(gameObject);
        else if(becomeChildOnHit)
            transform.SetParent(target);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}