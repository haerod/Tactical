using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class M_Rules : MonoBehaviour
{
    [Header("PLAY ORDER")] 
    
    [SerializeField] private List<TeamPlayOrder> teamsPlayOrder; 
    
    [Header("FOG OF WAR")]
    
    public bool enableFogOfWar = true;

    [Header("VISION")]

    public int percentReductionByDistance = 5;

    public enum SeeAnShotThrough { Everybody, Nobody, AlliesOnly}
    public SeeAnShotThrough canSeeAndShotThrough = SeeAnShotThrough.Everybody;
    public enum VisibleInFogOfWar { InView, Allies, Everybody}
    public VisibleInFogOfWar visibleInFogOfWar = VisibleInFogOfWar.Allies;
    
    public static M_Rules instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_Rules in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public List<TeamPlayOrder> GetTeamPlayOrders() => teamsPlayOrder;
    
    /// <summary>
    /// Return the first character of the play order.
    /// </summary>
    /// <returns></returns>
    public C__Character GetFirstCharacter() => teamsPlayOrder[0].FirstCharacter();
    
    /// <summary>
    /// Add a character to the Team play order.
    /// </summary>
    /// <param name="characterToAdd"></param>
    public void AddCharacter(C__Character characterToAdd)
    {
        TeamPlayOrder teamToAdd = teamsPlayOrder.FirstOrDefault(tpo => tpo.GetTeam() == characterToAdd.unitTeam);
        
        if(teamToAdd != null)
            teamsPlayOrder
                .FirstOrDefault(tpo => tpo.GetTeam() == characterToAdd.unitTeam)
                ?.AddCharacter(characterToAdd);
        else
            teamsPlayOrder.Add(new TeamPlayOrder(characterToAdd.unitTeam, characterToAdd));
        
        EditorUtility.SetDirty(this);
    }
    
    /// <summary>
    /// Remove a character from the Team play order
    /// </summary>
    /// <param name="characterToRemove"></param>
    public void RemoveCharacter(C__Character characterToRemove)
    {
        teamsPlayOrder
            .FirstOrDefault(tpo => tpo.GetTeam() == characterToRemove.unitTeam)
            ?.RemoveCharacter(characterToRemove);
        
        EditorUtility.SetDirty(this);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}

[Serializable]
public class TeamPlayOrder
{
    [SerializeField] private Team team;
    [SerializeField] private List<C__Character> charactersPlayOrder;

    public TeamPlayOrder(Team team, C__Character newCharacter)
    {
        this.team = team;
        charactersPlayOrder = new List<C__Character>();
        charactersPlayOrder.Add(newCharacter);
    }
    public Team GetTeam() => team;
    public List<C__Character> GetCharactersPlayOrder() => charactersPlayOrder;
    
    /// <summary>
    /// Return the first character of the team's characters play order.
    /// </summary>
    /// <returns></returns>
    public C__Character FirstCharacter() => charactersPlayOrder.First();
    
    /// <summary>
    /// Return the next character of this team, or null if it's the last.
    /// </summary>
    /// <param name="currentCharacter"></param>
    /// <returns></returns>
    public C__Character NextCharacter(C__Character currentCharacter) => currentCharacter == charactersPlayOrder.Last() ? null : charactersPlayOrder.Next(currentCharacter);

    /// <summary>
    /// Remove a character from charactersPlayOrder.
    /// </summary>
    /// <param name="characterToRemove"></param>
    public void RemoveCharacter(C__Character characterToRemove) => charactersPlayOrder.Remove(characterToRemove);
    
    /// <summary>
    /// Add a character to charactersPlayOrder.
    /// </summary>
    /// <param name="characterToAdd"></param>
    public void AddCharacter(C__Character characterToAdd) => charactersPlayOrder.Add(characterToAdd);
}