using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static M__Managers;
using System;

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
    /// End the character's turn and pass to the next one (depending the rules).
    /// </summary>
    public void EndTurn()
    {
        if (IsVictory()) // Victory
        {
            Victory();
        }
        else // Choose the next character or pass to another team.
        {
            C__Character nextTeamCharacter = NextCharacterInTheTeam();

            if (nextTeamCharacter) // Another character in the team
            {
                _characters.NewCurrentCharacter(nextTeamCharacter);
            }
            else
            {
                NextTeam();
            }
        }
    }

    /// <summary>
    /// Switch to another playable character of the team.
    /// </summary>
    public void SwitchToAnotherTeamPlayableCharacter()
    {
        C__Character current = _characters.current;
        C__Character next = _characters.GetCharacterList()
            .Where(c => c.team == current.team && c != current && c.behavior.playable == true)
            .FirstOrDefault();

        if(next)
            _characters.NewCurrentCharacter(next);
    }

    /// <summary>
    /// End the turn of all the playable characters of the team and pass to the next one to play.
    /// </summary>
    public void EndAllPlayableCharactersTurn()
    {
        C__Character current = _characters.current;

        _characters.GetCharacterList()
            .ForEach(c => c.SetCanPlayValue(false));

        EndTurn();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Start the next team turn, allowing them to play.
    /// </summary>
    private void NextTeam()
    {
        C__Character current = _characters.current;
        C__Character newCharacter;
        List<C__Character> newTeam = new List<C__Character>();

        if (_rules.botsPlay == M_Rules.BotsPlayOrder.BeforePlayableCharacters) // NPC play first
        {
            newTeam = _characters.GetCharacterList()
                .Where(c => c.team != current.team || c == current)
                .OrderBy(c => c.team.name)
                .ThenBy(c => c.behavior.playable)
                .ToList();

            newCharacter = newTeam.Next(current);
        }
        else // PC play first
        {
            newTeam = _characters.GetCharacterList()
                            .Where(c => c.team != current.team || c == current)
                            .OrderBy(c => c.team.name)
                            .ThenByDescending(c => c.behavior.playable)
                            .ToList();

            newCharacter = newTeam.Next(current);
        }

        newTeam = _characters.GetCharacterList()
            .Where(c => c.team == newCharacter.team)
            .ToList();

        newTeam
            .ForEach(character => character.SetCanPlayValue(true));

        _characters.NewCurrentCharacter(newCharacter);
    }
    
    /// <summary>
    /// Return the next character who haves to play in the team, depending the rules.
    /// Return null if nobody can play.
    /// </summary>
    /// <returns></returns>
    private C__Character NextCharacterInTheTeam()
    {
        C__Character current = _characters.current;
        List<C__Character> group = new List<C__Character>();
        bool currentIsPlayable = current.behavior.playable;

        if (currentIsPlayable) // PC
        {
            group = _characters
                .GetTeamPC(current)
                .Where(c => c.CanPlay())
                .ToList();
        }
        else // NPC
        {
            group = _characters
                .GetTeamNPC(current)
                .Where(c => c.CanPlay())
                .ToList();
        }

        if (group.Where(c => c != current).ToList().Count == 0) // No character playable after this one.
        {
            if (currentIsPlayable && _rules.botsPlay == M_Rules.BotsPlayOrder.AfterPlayableCharacters) // PC
            {
                return _characters
                    .GetTeamNPC(current)
                    .Where(c => c.CanPlay())
                    .FirstOrDefault(); // EXIT: Return the first NPC or null
            }
            else if (!currentIsPlayable && _rules.botsPlay == M_Rules.BotsPlayOrder.BeforePlayableCharacters) // NPC
            {
                return _characters
                       .GetTeamPC(current)
                       .Where(c => c.CanPlay())
                       .FirstOrDefault(); // EXIT: Return the first NPC or null
            }

            return null; // EXIT: Nobody can play next.
        }

        return group.Next(group.IndexOf(current));
    }

    /// <summary>
    /// Check if it's currently victory.
    /// </summary>
    private bool IsVictory()
    {
        return _characters.IsFinalTeam(_characters.current);
    }

    /// <summary>
    /// Enable victory screen and do the other things happening during victory
    /// </summary>
    private void Victory()
    {
        C__Character current = _characters.current;

        _ui.SetActivePlayerUI_Turn(false);
        _ui.EnableEndScreen(current);

        current.HideTilesFeedbacks();

        _input.SetActiveClick(false);
        _input.ClearFeedbacksAndValues();
    }
}
