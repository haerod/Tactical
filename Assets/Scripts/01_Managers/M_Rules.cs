using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static M__Managers;

public class M_Rules : MonoBehaviour
{
    [Header("VICTORY RULES")]
    
    public VictoryCondition victoryCondition = VictoryCondition.Deathmatch;
    public enum VictoryCondition { Deathmatch,}
    
    [Header("FOG OF WAR")]
    
    [SerializeField] private F_FogOfWar fogOfWar;
    public enum VisibleInFogOfWar { InView, Allies, Everybody}
    public VisibleInFogOfWar visibleInFogOfWar = VisibleInFogOfWar.Allies;
    
    public static M_Rules instance;
    
    public event EventHandler OnVictory;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_Rules in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns true if there is an enabled fog of war.
    /// </summary>
    /// <returns></returns>
    public bool IsFogOfWar() => fogOfWar && fogOfWar.gameObject.activeInHierarchy;

    /// <summary>
    /// Checks if it's currently victory.
    /// </summary>
    public bool IsVictory()
    {
        if (_characters.GetEnemiesOf(_characters.current).Count > 0)
            return false; // Not victory
        
        OnVictory?.Invoke(null, EventArgs.Empty);
        _input.SetActivePlayerInput(false);
        return true;

    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}

// [Serializable]
// public class TeamPlayOrder
// {
//     [SerializeField] private Team team;
//     [SerializeField] private List<C__Character> charactersPlayOrder;
//
//     public TeamPlayOrder(Team team, C__Character newCharacter)
//     {
//         this.team = team;
//         charactersPlayOrder = new List<C__Character>();
//         charactersPlayOrder.Add(newCharacter);
//     }
//     public Team GetTeam() => team;
//     public List<C__Character> GetCharactersPlayOrder() => charactersPlayOrder;
//     
//     /// <summary>
//     /// Returns the first unit of the team's characters play order.
//     /// </summary>
//     /// <returns></returns>
//     public C__Character FirstUnit() => charactersPlayOrder.First();
//     
//     /// <summary>
//     /// Returns the next unit of this team who can, or null if nobody exists or can.
//     /// </summary>
//     /// <param name="currentCharacter"></param>
//     /// <returns></returns>
//     public C__Character GetNextTeamPlayableUnit(C__Character currentCharacter)
//     {
//         List<C__Character> teamPlayableCharacter = charactersPlayOrder
//             .Where(chara => chara.CanPlay())
//             .ToList();
//         
//         return teamPlayableCharacter.Count == 0 ? null : teamPlayableCharacter.Next(currentCharacter);
//     }
//     
//     /// <summary>
//     /// Returns the next unit of the team, or null if nobody exist.
//     /// </summary>
//     /// <param name="unit"></param>
//     /// <returns></returns>
//     public C__Character GetNextTeamUnit(C__Character unit) => charactersPlayOrder.Count == 0 ? null : charactersPlayOrder.Next(unit);
//
//     /// <summary>
//     /// Returns true if is any unit alive in the team.
//     /// </summary>
//     /// <returns></returns>
//     public bool IsAliveUnit() => charactersPlayOrder.Count > 0;
//     
//     /// <summary>
//     /// Removes a character from charactersPlayOrder.
//     /// </summary>
//     /// <param name="characterToRemove"></param>
//     public void RemoveCharacter(C__Character characterToRemove) => charactersPlayOrder.Remove(characterToRemove);
//     
//     /// <summary>
//     /// Adds a character to charactersPlayOrder.
//     /// </summary>
//     /// <param name="characterToAdd"></param>
//     public void AddCharacter(C__Character characterToAdd) => charactersPlayOrder.Add(characterToAdd);
//}