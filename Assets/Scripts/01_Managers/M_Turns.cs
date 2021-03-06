using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static M__Managers;

public class M_Turns : MonoBehaviour
{
    public static M_Turns instance;

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
            Debug.LogError("There is more than one M_TurnManager in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Pass to the next player's turn, or NPC of the team (depending the Rules).
    /// </summary>
    public void EndCurrentTeamsTurn()
    {
        // New character
        C__Character current = NextAllyNPCOrEnemy();

        if (!current)
        {
            Victory();
            return; // EXIT : There is no character in another team, Victory for this team !
        }

        _characters.NewCurrentCharacter(current);
    }

    /// <summary>
    /// Pass to the playbale teammates turn.
    /// </summary>
    public void ChangeTeamCharacter()
    {
        // Old character
        C__Character current = NextAllyNPCOrEnemy();

        current = _turns.NextPlayableCharacterInTeam();
        if (!current) return;

        _characters.NewCurrentCharacter(current);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Get the next character in the team.
    /// Return null if there is no playable teammate.
    /// </summary>
    private  C__Character NextPlayableCharacterInTeam()
    {
        C__Character current = _characters.currentCharacter;

        // Find the playble teamates
        List<C__Character> team = _characters.characters
            .Where(o => o.Team() == current.Team() && o.behavior.playable)
            .ToList();
        
        if (team.Count <= 1) return null; // EXIT : There is no playble teammates.

        return team.Next(team.IndexOf(current));
    }

    /// <summary>
    /// Get the first character of the next team or NPC in ally team (depending Rules).
    /// Return null if there is no enemy in next teams.
    /// </summary>
    /// <returns></returns>
    private C__Character NextAllyNPCOrEnemy()
    {
        //NB : M_Characters previously orders the characters by teams/PC/NPC.

        C__Character current = _characters.currentCharacter;

        // Current is playable
        // ===================

        if(current.behavior.playable)
        {
            // Get everybody except other PC of the team (current character || NPC in team || all other teams' characters)
            List<C__Character> charas = _characters.characters
                .Where(o => (o == current) || (o.Team() == current.Team() && o.behavior.playable == false) || (o.Team() != current.Team()))
                .ToList();

            if (charas.Count <= 1) return null; // EXIT : There is only this character in the list.

            return charas.Next(charas.IndexOf(current)); // EXIT : Return the next ally NPC or enemy (PC or NPC).
        }

        // Current is a NPC
        // ================

        if (_characters.characters.Count <= 1) return null; // EXIT : There is only this character in the list.

        // Just return the next in the list.
        return _characters.characters.Next(_characters.characters.IndexOf(current));
    }

    /// <summary>
    /// Enable victory screen and do the other things happening during victory
    /// </summary>
    private void Victory()
    {
        C__Character current = _characters.currentCharacter;

        _ui.SetTurnPlayerUIActive(false);
        _ui.EnableEndScreen(current);

        current.ClearTilesFeedbacks();

        _input.SetClick(false);
        _input.ClearFeedbacksAndValues();
    }

}
