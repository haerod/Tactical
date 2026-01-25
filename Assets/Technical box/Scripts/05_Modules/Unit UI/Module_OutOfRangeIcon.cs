using System;
using UnityEngine;
using static M__Managers;

/// <summary>
/// Shows when the current unit is out of range of this unit on its interface.
/// </summary>
public class Module_OutOfRangeIcon : MonoBehaviour
{
    [SerializeField] private GameObject outOfRangeIcon;
    [SerializeField] private Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Shows the icon.
    /// </summary>
    private void Show() => outOfRangeIcon.SetActive(true);
    
    /// <summary>
    /// Hides the icon.
    /// </summary>
    private void Hide() => outOfRangeIcon.SetActive(false);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit.team.IsTeammateOf(unit))
            return; // Teammate
        if(!currentUnit.look.CanSee(unit))
            return; // Not visible
        if(!currentUnit.behavior.playable)
            return; // NPC
        if(!currentUnit.move.movementArea.Contains(enteredTile))
            return; // Tile not in movement area
        if(unit.look.visibleTiles.Contains(enteredTile))
            Hide();
        else
            Show();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
}