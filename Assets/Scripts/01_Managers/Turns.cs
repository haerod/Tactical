using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;

public static class Turns
{
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// End the character's turn and pass to the next one (depending the rules).
    /// </summary>
    public static void EndTurn()
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
    public static void SwitchToAnotherTeamPlayableCharacter()
    {
        C__Character current = _characters.current;
        C__Character next = _characters
            .GetCharacterList()
            .FirstOrDefault(c => c.team == current.team && c != current && c.behavior.playable);

        if(next)
            _characters.NewCurrentCharacter(next);
    }

    /// <summary>
    /// End the turn of all the playable characters of the team and pass to the next one to play.
    /// </summary>
    public static void EndAllPlayableCharactersTurn()
    {
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
    private static void NextTeam()
    {
        TeamPlayOrder currentTeamPlayOrder = GetTeamPlayOrder(_characters.current);
        TeamPlayOrder newTeamPlayOrder = _rules.GetTeamPlayOrders().Next(currentTeamPlayOrder);
        
        newTeamPlayOrder
            .GetCharactersPlayOrder()
            .ForEach(character => character.SetCanPlayValue(true));
        
        _characters.NewCurrentCharacter(newTeamPlayOrder.FirstCharacter());
    }
    
    /// <summary>
    /// Return the next character who haves to play in the team, depending on the rules' play order .
    /// Return null if nobody can play.
    /// </summary>
    /// <returns></returns>
    private static C__Character NextCharacterInTheTeam()
    {
        C__Character currentCharacter = _characters.current;
        TeamPlayOrder currentTeamPlayOrder = GetTeamPlayOrder(currentCharacter);
        C__Character nextCharacter = currentTeamPlayOrder.NextCharacter(currentCharacter);

        return nextCharacter ? nextCharacter : null;
    }
    
    /// <summary>
    /// Return the character's Team play order of the Rules
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private static TeamPlayOrder GetTeamPlayOrder(C__Character character) => _rules.GetTeamPlayOrders().FirstOrDefault(tpo => tpo.GetTeam() == character.team);
    
    /// <summary>
    /// Check if it's currently victory.
    /// </summary>
    private static bool IsVictory() => _characters.IsFinalTeam(_characters.current);

    /// <summary>
    /// Enable victory screen and do the other things happening during victory
    /// </summary>
    private static void Victory()
    {
        C__Character current = _characters.current;

        _ui.SetActivePlayerUI_Turn(false);
        _ui.EnableEndScreen(current);

        current.HideTilesFeedbacks();

        _input.SetActiveClick(false);
        _input.ClearFeedbacksAndValues();
    }
}
