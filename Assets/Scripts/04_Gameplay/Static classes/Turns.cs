using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;
using System.Drawing.Printing;
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
    /// Ends the unit's turn and passes to the next one (depending on the rules).
    /// </summary>
    public static void EndTurn()
    {
        if (IsVictory()) // Victory
        {
            OnVictory?.Invoke(null, EventArgs.Empty);
            _input.SetActivePlayerInput(false);
            return;
        }
        
        // Choose the next unit or pass to another team.
        C__Character nextTeamUnit = NextUnitInTheTeam();

        if (nextTeamUnit) // Another character in the team
            _characters.NewCurrentCharacter(nextTeamUnit);
        else
            NextTeam();
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Switches to the next unit of the team.
    /// </summary>
    private static void SwitchToNextTeamUnit()
    {
        C__Character current = _characters.current;
        C__Character next = GetTeamPlayOrder(current).GetNextTeamUnit(current);

        if(next && next != current)
            _characters.NewCurrentCharacter(next);
    }
    
    /// <summary>
    /// Ends the turn of all the playable units of the team and passes to the next one to play.
    /// </summary>
    private static void EndAllPlayableUnitsTurn()
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
        
        _characters.NewCurrentCharacter(newTeamPlayOrder.FirstUnit());
    }
    
    /// <summary>
    /// Returns the next unit which haves to play in the team, depending on the rules' play order.
    /// Return null if nobody can play.
    /// </summary>
    /// <returns></returns>
    private static C__Character NextUnitInTheTeam()
    {
        C__Character currentCharacter = _characters.current;
        TeamPlayOrder currentTeamPlayOrder = GetTeamPlayOrder(currentCharacter);
        C__Character nextCharacter = currentTeamPlayOrder.GetNextTeamUnit(currentCharacter);

        return nextCharacter ? nextCharacter : null;
    }
    
    /// <summary>
    /// Returns the unit's Team play order of the Rules
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private static TeamPlayOrder GetTeamPlayOrder(C__Character unit) => _rules
        .GetTeamPlayOrders()
        .FirstOrDefault(tpo => tpo.GetTeam() == unit.unitTeam);
    
    /// <summary>
    /// Checks if it's currently victory.
    /// </summary>
    private static bool IsVictory() => _characters.IsFinalTeam(_characters.current);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private static void Input_OnEndTurnInput(object sender, EventArgs e)
    {
        EndAllPlayableUnitsTurn();
    }
    
    private static void Input_OnChangeCharacterInput(object sender, EventArgs e)
    {
        SwitchToNextTeamUnit();
    }
    
}
