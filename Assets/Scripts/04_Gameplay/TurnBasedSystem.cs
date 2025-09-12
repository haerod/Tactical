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
    
    public U__Unit GetNextUnit()
    {
        U__Unit currentUnit = _units.current;
        U__Unit nextUnit = NextPlayableUnit();
        
        return nextUnit ? nextUnit : NextPlayableUnit();
    }
    
    /// <summary>
    /// Returns the first unit of the first team of the play order.
    /// </summary>
    /// <returns></returns>
    public U__Unit GetFirstUnit() => _units
        .GetUnitsOf(_units.GetTeamPlayOrder().First())
        .First();
    
    public U__Unit NextTeamUnit()
    {
        U__Unit nextTeamUnit = _units.GetAlliesOf(_units.current)
            .Next(_units.current);
        
        return nextTeamUnit != _units.current ? nextTeamUnit : null;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts the next team turn, allowing them to play.
    /// </summary>
    private Team GetNextTeam() => _units.GetTeamPlayOrder()
        .Next(_units.current.unitTeam);
    
    /// <summary>
    /// Returns the next unit which haves to play, or null if nobody can.
    /// </summary>
    /// <returns></returns>
    private U__Unit NextPlayableUnit()
    {
        U__Unit currentUnit = _units.current;
        U__Unit nextUnit = _units.GetAlliesOf(currentUnit)
            .Where(unit => unit != currentUnit)
            .FirstOrDefault(unit => unit.CanPlay());

        if (nextUnit)
            return nextUnit;
        
        return _units.GetUnitsOf(GetNextTeam())
            .FirstOrDefault();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
