using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Create a unit ragdoll
/// </summary>
public class Module_UnitRagdoll : MonoBehaviour
{
    [SerializeField] private bool dropWeapon;
    [SerializeField] private GameObject unitRagdollPrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyDeath += Health_OnAnyDeath;
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
    /// <param name="deadUnit"></param>
    private void CreateRagdoll(Unit deadUnit)
    {
        GameObject unitRagdoll = Instantiate(
            unitRagdollPrefab,
            deadUnit.transform.position,
            deadUnit.transform.rotation);
        
        Transform originalRootBone = deadUnit.graphics.rootBone;
        Unit_Graphics ragdollGraphics = unitRagdoll.GetComponent<Unit_Graphics>();
        Transform ragdollRootBone = ragdollGraphics.rootBone;
        
        AssignMaterials(deadUnit.graphics, ragdollGraphics);
        
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, 10f);
        if(dropWeapon)
            DropWeaponOnTheFloor(deadUnit);
    }
    
    /// <summary>
    /// Changes the transforms of the ragdoll to match with the original.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="clone"></param>
    private void MatchAllChildTransforms(Transform original, Transform clone)
    {
        foreach (Transform originalChild in original)
        {
            Transform cloneChild = clone.Find(originalChild.name);

            if (cloneChild == null)
                continue; // No more child
            
            cloneChild.position = originalChild.position;
            cloneChild.rotation = originalChild.rotation;

            MatchAllChildTransforms(originalChild, cloneChild);
        }
    }
    
    /// <summary>
    /// Adds an explosion force to the ragdoll's bones.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="explosionForce"></param>
    /// <param name="explosionRange"></param>
    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, float explosionRange)
    {
        Vector3 randomDir = new (Random.Range(-1f,1f),0, Random.Range(-1f,1f));
        Vector3 explosionPosition = transform.position + randomDir;
        
        foreach (Transform child in root)
        {
            if(child.TryGetComponent(out Rigidbody rb))
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            
            ApplyExplosionToRagdoll(child, explosionForce, explosionRange);
        }
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
    
    /// <summary>
    /// Copies original unit's materials to the ragdoll.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="ragdoll"></param>
    private void AssignMaterials(Unit_Graphics original, Unit_Graphics ragdoll)
    {
        for (int i = 0; i < original.skinnedMeshRenderers.Count; i++)
            ragdoll.skinnedMeshRenderers[i].materials = original.skinnedMeshRenderers[i].materials;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Health_OnAnyDeath(object sender, Unit deadUnit)
    {
        CreateRagdoll(deadUnit);
        deadUnit.gameObject.SetActive(false);
    }
}