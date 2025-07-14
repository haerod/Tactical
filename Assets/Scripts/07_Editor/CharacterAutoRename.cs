using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;

[ExecuteInEditMode]
public class CharacterAutoRename : MonoBehaviour
{
    private C__Character current;

    private bool previousBehavior;
    private Team previousTeam;
    private string previousName;
    private string previousTeamName;
    private Material previousMainTeamMatrial;
    private Material previousSecondaryTeamMaterial;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
            return;
        }

        current = GetComponent<C__Character>();

        previousBehavior = current.behavior.playable;
        previousTeam = current.team;
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
        || previousTeam != current.infos.team 
        || previousName != current.unitName
        || previousMainTeamMatrial != current.team.mainMaterial
        || previousSecondaryTeamMaterial != current.team.secondaryMaterial
        || previousTeamName != current.team.name;

    /// <summary>
    /// Update the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousBehavior = current.behavior.playable;
        previousTeam = current.infos.team;
        previousName = current.unitName;
        previousMainTeamMatrial = current.team.mainMaterial;
        previousSecondaryTeamMaterial = current.team.secondaryMaterial;
        previousTeamName = current.team.name;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Rename the element and assign the colors.
    /// </summary>
    private void AutoRename()
    {
        if (current.team)
        {
            current.name = string.Format("{0} ({2}) - Team {1}",
                current.unitName,
                current.team.name,
                current.behavior.playable ? "PC" : "NPC");
        }

        EditorUtility.SetDirty(current.gameObject); // Save the character modifications
        EditorUtility.SetDirty(current.move); // Save the character modifications
    }

    /// <summary>
    /// Assigns automatically the team's materials.
    /// </summary>
    private void AutoAssignMaterials()
    {
        current.infos.SetTeamMaterials();
    }
}
