using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Create a unit ragdoll
/// </summary>
public class FM_UnitRagdoll : MonoBehaviour
{
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
    
    private void CreateRagdoll(U__Unit deadUnit)
    {
        U__Unit unitRagdoll = Instantiate(
            unitRagdollPrefab,
            deadUnit.transform.position,
            deadUnit.transform.rotation).GetComponent<U__Unit>();
        
        Transform originalRootBone = deadUnit.anim.transform.GetChild(2);
        Transform ragdollRootBone= unitRagdoll.anim.transform.GetChild(2);
        
        unitRagdoll.weaponHolder.SetCurrentWeapon(deadUnit.weaponHolder.GetCurrentWeapon());
        unitRagdoll.team.team = deadUnit.unitTeam;
        unitRagdoll.team.SetTeamMaterials();
        
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, 10f);
        DropWeaponOnTheFloor(unitRagdoll);
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
            if(child.TryGetComponent(out Rigidbody rigidbody))
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            
            ApplyExplosionToRagdoll(child, explosionForce, explosionRange);
        }
    }
    
    private void DropWeaponOnTheFloor(U__Unit unitRagdoll)
    {
        GameObject dropWeapon = unitRagdoll.weaponHolder.GetCurrentWeaponGraphics().gameObject;
        Vector3 randomDir = new (Random.Range(-1f,1f),0, Random.Range(-1f,1f));
        
        dropWeapon.transform.parent = null;
        
        if(dropWeapon.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddExplosionForce(50f, transform.position + randomDir, 10f);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Health_OnAnyDeath(object sender, U__Unit deadUnit)
    {
        CreateRagdoll(deadUnit);
        deadUnit.gameObject.SetActive(false);
    }
}