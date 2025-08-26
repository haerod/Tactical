using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;
using UnityEngine;

public static class Turns
{
    // ======================================================================
    // INITIALIZE
    // ======================================================================

    public static event EventHandler OnVictory;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start()
    {
        _input.OnChangeCharacterInput += Input_OnChangeCharacterInput;
        _input.OnEndTurnInput += Input_OnEndTurnInput;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Ends the character's turn and passes to the next one (depending on the rules).
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
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Switches to another playable character of the team.
    /// </summary>
    private static void SwitchToAnotherTeamPlayableCharacter()
    {
        C__Character current = _characters.current;
        C__Character next = _characters
            .GetCharacterList()
            .FirstOrDefault(c => c.unitTeam == current.unitTeam && c != current && c.behavior.playable);

        if(next)
            _characters.NewCurrentCharacter(next);
    }

    /// <summary>
    /// Ends the turn of all the playable characters of the team and passes to the next one to play.
    /// </summary>
    private static void EndAllPlayableCharactersTurn()
    {
        _characters.GetCharacterList()
            .ForEach(c => c.SetCanPlayValue(false));

        EndTurn();
    }
    
    /// <summary>
    /// Starts the next team turn, allowing them to play.
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
    /// Returns the next character which haves to play in the team, depending on the rules' play order.
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
    /// Returns the character's Team play order of the Rules
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private static TeamPlayOrder GetTeamPlayOrder(C__Character character) => _rules.GetTeamPlayOrders().FirstOrDefault(tpo => tpo.GetTeam() == character.unitTeam);
    
    /// <summary>
    /// Checks if it's currently victory.
    /// </summary>
    private static bool IsVictory() => _characters.IsFinalTeam(_characters.current);

    /// <summary>
    /// Enables victory screen and do the other things happening during victory
    /// </summary>
    private static void Victory()
    {
        OnVictory?.Invoke(null, EventArgs.Empty);
        
        C__Character current = _characters.current;
        
        current.HideTilesFeedbacks();

        _input.SetActiveClick(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private static void Input_OnEndTurnInput(object sender, EventArgs e)
    {
        EndAllPlayableCharactersTurn();
    }

    private static void Input_OnChangeCharacterInput(object sender, EventArgs e)
    {
        if (!_characters.current.behavior.playable) 
            return; // NPC turn

        SwitchToAnotherTeamPlayableCharacter();
    }
}
