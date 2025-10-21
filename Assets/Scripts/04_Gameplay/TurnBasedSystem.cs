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
    
    /// <summary>
    /// Returns the next playable teammate, or itself if nobody can.
    /// </summary>
    /// <returns></returns>
    public U__Unit GetNextPlayableTeammate()
    {
        U__Unit currentUnit = _units.current;
        return _units.GetUnitsOf(currentUnit.unitTeam)
            .Where(unit => unit == currentUnit || unit.CanPlay())
            .ToList()
            .Next(currentUnit);
    }
    
    /// <summary>
    /// Returns the previous playable teammate, or itself if nobody can.
    /// </summary>
    /// <returns></returns>
    public U__Unit GetPreviousPlayableTeammate()
    {
        U__Unit currentUnit = _units.current;
        return _units.GetUnitsOf(currentUnit.unitTeam)
            .Where(unit => unit == currentUnit || unit.CanPlay())
            .ToList()
            .Previous(currentUnit);
    }
    
    /// <summary>
    /// Returns the first unit of the first team of the play order.
    /// </summary>
    /// <returns></returns>
    public U__Unit GetFirstUnit() => _units
        .GetUnitsOf(_units.GetTeamPlayOrder().First())
        .First();
    
    /// <summary>
    /// Returns the next unit in the team.
    /// </summary>
    /// <returns></returns>
    public U__Unit GetNextTeamUnit()
    {
        U__Unit nextTeamUnit = _units.GetAlliesOf(_units.current)
            .Next(_units.current);
        
        return nextTeamUnit != _units.current ? nextTeamUnit : null;
    }
    
    /// <summary>
    /// Returns the next team playing.
    /// </summary>
    public Team GetNextTeam() => _units.GetTeamPlayOrder()
        .Next(_units.current.unitTeam);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
