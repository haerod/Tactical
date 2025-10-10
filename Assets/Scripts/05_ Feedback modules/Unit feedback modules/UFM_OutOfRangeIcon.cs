using System;
using UnityEngine;
using static M__Managers;

/// <summary>
/// Shows when the current unit is out of range of this unit on its interface.
/// </summary>
public class UFM_OutOfRangeIcon : MonoBehaviour
{
    [SerializeField] private GameObject outOfRangeIcon;
    [SerializeField] private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
    }

    private void OnDisable()
    {
        InputEvents.OnTileEnter -= InputEvents_OnTileEnter;
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
        U__Unit currentUnit = _units.current;
        
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
}