using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Create a unit ragdoll
/// </summary>
public class Module_UnitRagdoll2 : MonoBehaviour
{
    [SerializeField] private bool dropWeapon;
    
    [Space]
    
    [SerializeField] private bool _applyDeadMaterial;
    [SerializeField] private Material _deadMaterial;

    [Space]

    [SerializeField] private float _impactForce = 40f;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private GameObject unitRagdollPrefab;
    [SerializeField] private Unit unit;
    
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        unit.health.OnDeath += Health_OnDeath;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Creates a ragdoll and match it with the other parts.
    /// </summary>
    private void CreateRagdoll()
    {
        GameObject unitRagdoll = Instantiate(
            unitRagdollPrefab,
            unit.transform.position,
            unit.transform.rotation);
        
        if(dropWeapon)
            DropWeaponOnTheFloor(unit);
        
        MatchGraphics(unit.graphics.transform, unitRagdoll.transform);
        ApplyExplosionToRagdoll(unitRagdoll.GetComponent<Unit_Graphics>(), _impactForce);
    }
    
    /// <summary>
    /// Changes the transforms and set active/inactive objects of the ragdoll to match with the original.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="clone"></param>
    private void MatchGraphics(Transform original, Transform clone)
    {
        foreach (Transform originalChild in original)
        {
            Transform cloneChild = clone.Find(originalChild.name);
            bool originalIsActive = originalChild.gameObject.activeSelf;
            
            if (cloneChild == null)
                continue; // No more child
            
            cloneChild.gameObject.SetActive(originalIsActive);
            
            if(!originalIsActive)
                continue; // Not enabled
            
            cloneChild.position = originalChild.position;
            cloneChild.rotation = originalChild.rotation;

            Renderer originalRenderer = originalChild.GetComponent<Renderer>();
            
            if (originalRenderer)
                cloneChild.GetComponent<Renderer>().material = _applyDeadMaterial && _deadMaterial ? _deadMaterial : originalRenderer.material;
            
            MatchGraphics(originalChild, cloneChild);
        }
    }
    
    /// <summary>
    /// Adds an explosion force to the ragdoll's bones.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="force"></param>
    private void ApplyExplosionToRagdoll(Unit_Graphics graphics, float force)
    {
        //Vector3 randomDir = new (Random.Range(-1f,1f),0, Random.Range(-1f,1f));
        //Vector3 explosionPosition = transform.position + randomDir;
        
        // foreach (Transform child in root)
        // {
        //     if(child.TryGetComponent(out Rigidbody rb))
        //         rb.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
        //     
        //     ApplyExplosionToRagdoll(child, explosionForce, explosionRange);
        // }
        
        if(graphics.torso.TryGetComponent(out Rigidbody rb))
            rb.AddForceAtPosition(graphics.transform.up * force/2f + graphics.transform.forward * -force, rb.centerOfMass, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Creates a coby of the dead unit's weapon and drop it on the floor.
    /// </summary>
    /// <param name="originalUnit"></param>
    private void DropWeaponOnTheFloor(Unit originalUnit)
    {
        GameObject originalPhysics = originalUnit.weaponHolder.weaponGraphics.GetComponent<Weapon>().physics;
        
        if(!originalPhysics)
            return; // Not physiqued
        
        GameObject originalWeapon = originalUnit.weaponHolder.weaponGraphics.gameObject;
        GameObject instantiatedWeapon = Instantiate(
            originalWeapon,
            originalWeapon.transform.position,
            originalWeapon.transform.rotation);
        GameObject instantiatedPhysics = instantiatedWeapon.GetComponent<Weapon>().physics;
        instantiatedPhysics.SetActive(true);
        Rigidbody rb = instantiatedWeapon.AddComponent<Rigidbody>();
        
        Vector3 randomDir = new (Random.Range(-1f,1f),0, Random.Range(-1f,1f));
        rb.AddExplosionForce(50f, instantiatedWeapon.transform.position + randomDir, 10f);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        CreateRagdoll();
        unit.gameObject.SetActive(false);
    }
}