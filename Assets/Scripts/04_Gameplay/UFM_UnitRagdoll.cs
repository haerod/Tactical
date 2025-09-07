using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Create a unit ragdoll
/// </summary>
public class UFM_UnitRagdoll : MonoBehaviour
{
    [SerializeField] private GameObject unitRagdollPrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        U_Health.OnAnyDeath += Health_OnAnyDeath;
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
        Vector3 randomDir = new (Random.Range(-1f,1f),0, Random.Range(-1f,1f));
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, transform.position + randomDir, 10f);
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
    
    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if(child.TryGetComponent(out Rigidbody rigidbody))
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            
            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
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