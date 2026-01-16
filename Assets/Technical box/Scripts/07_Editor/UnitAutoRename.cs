using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;

[ExecuteInEditMode]
public class UnitAutoRename : MonoBehaviour
{
    [SerializeField] private string playableCharacterDesignation = "PC";
    [SerializeField] private string notPlayableCharacterDesignation = "NPC";
    
    [Header("REFERENCES")]
    [SerializeField] private Renderer rend1;
    [SerializeField] private Renderer rend2;
    
    private U__Unit current;

    private bool previousBehavior;
    private Team previousTeam;
    private string previousName;
    private string previousTeamName;
    private Material previousMainTeamMaterial;
    private Material previousSecondaryTeamMaterial;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

#if UNITY_EDITOR
    
    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
            return;
        }

        current = GetComponent<U__Unit>();

        previousBehavior = current.behavior.playable;
        previousTeam = current.unitTeam;
        previousName = current.unitName;

        EditorUtility.SetDirty(this);
    }

    private void Update()
    {
        if (!AreModifications()) 
            return; // No modifications
        
        AutoAssignMaterials();
        AutoRename();
        UpdateModificationValues();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check for modifications in the name, team or behavior.
    /// </summary>
    /// <returns></returns>
    private bool AreModifications() => previousBehavior != current.behavior.playable 
        || previousTeam != current.unitTeam
        || previousName != current.unitName
        || previousMainTeamMaterial != current.unitTeam.mainMaterial
        || previousSecondaryTeamMaterial != current.unitTeam.secondaryMaterial
        || previousTeamName != current.unitTeam.name;

    /// <summary>
    /// Update the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousBehavior = current.behavior.playable;
        previousTeam = current.unitTeam;
        previousName = current.unitName;
        previousMainTeamMaterial = current.unitTeam.mainMaterial;
        previousSecondaryTeamMaterial = current.unitTeam.secondaryMaterial;
        previousTeamName = current.unitTeam.name;
        EditorUtility.SetDirty(this);

    }

    /// <summary>
    /// Rename the element and assign the colors.
    /// </summary>
    private void AutoRename()
    {
        if (current.unitTeam)
        {
            current.name = string.Format("{0} ({2}) - Team {1}",
                current.unitName,
                current.unitTeam.name,
                current.behavior.playable ? playableCharacterDesignation : notPlayableCharacterDesignation);
        }
        EditorUtility.SetDirty(current.gameObject); // Save the unit's modifications
        EditorUtility.SetDirty(current.move); // Save the unit's modifications
    }

    /// <summary>
    /// Assigns automatically the team's materials.
    /// </summary>
    private void AutoAssignMaterials()
    {
        Team team = current.unitTeam;
        
        if(!team)
        {
            Debug.LogError(transform.parent.name + " doesn't have a team. Please assign a team.", transform.parent.gameObject);
            return; // No team assigned
        }

        if (rend1 && team.mainMaterial)
        {
            rend1.material = team.mainMaterial;
            EditorUtility.SetDirty(rend1);
        }
        if (rend2 && team.secondaryMaterial)
        {
            rend2.material = team.secondaryMaterial;
            EditorUtility.SetDirty(rend2);
        }
    }
#endif
}
