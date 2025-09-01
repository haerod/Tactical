using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;
using System.Drawing.Printing;
using UnityEngine;

public class TurnBasedSystem : MonoBehaviour
{
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public C__Character GetNextUnit()
    {
        C__Character currentUnit = _characters.current;
        C__Character nextUnit = NextPlayableUnit();
        
        return nextUnit ? nextUnit : NextPlayableUnit();
    }
    
    /// <summary>
    /// Returns the first character of the first team of the play order.
    /// </summary>
    /// <returns></returns>
    public C__Character GetFirstCharacter() => _characters
        .GetUnitsOf(_characters.GetTeamsPlayOrder().First())
        .First();
    
    public C__Character NextTeamUnit()
    {
        C__Character nextTeamUnit = _characters.GetAlliesOf(_characters.current)
            .Next(_characters.current);
        
        return nextTeamUnit != _characters.current ? nextTeamUnit : null;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts the next team turn, allowing them to play.
    /// </summary>
    private Team GetNextTeam() => _characters.GetTeamsPlayOrder()
        .Next(_characters.current.unitTeam);
    
    /// <summary>
    /// Returns the next unit which haves to play, or null if nobody can.
    /// </summary>
    /// <returns></returns>
    private C__Character NextPlayableUnit()
    {
        C__Character currentUnit = _characters.current;
        C__Character nextUnit = _characters.GetAlliesOf(currentUnit)
            .Where(unit => unit != currentUnit)
            .FirstOrDefault(unit => unit.CanPlay());

        if (nextUnit)
            return nextUnit;
        
        return _characters.GetUnitsOf(GetNextTeam())
            .FirstOrDefault();
    }
    
    /// <summary>
    /// Returns true if is another unit in the given unit team. Else returns false.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool IsAnotherUnitOfTheSameTeam(C__Character unit) =>_characters.GetUnitsList()
        .Where(testedUnit => testedUnit != unit)
        .FirstOrDefault(testedUnit => testedUnit.team.IsAllyOf(unit));
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
