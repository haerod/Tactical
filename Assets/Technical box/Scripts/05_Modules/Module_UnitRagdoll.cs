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