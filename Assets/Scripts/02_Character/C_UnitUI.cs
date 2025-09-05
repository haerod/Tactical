using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class C_UnitUI : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private C__Character c;
    [SerializeField] private UI_OutOfRangeIcon outOfRangeIcon;
    [SerializeField] private UI_OrientToCamera orientToCamera;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Orients the in-world UI to the camera.
    /// </summary>
    public void OrientToCamera() => orientToCamera.OrientToCamera();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void DisplayOutOfRangeIcon()
    {
        //Display();
        outOfRangeIcon.Display();
    }

    private void HideOutOfRangeIcon()
    {
        outOfRangeIcon.Hide();
    }
    
    /// <summary>
    /// Starts a waits for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits for "time" seconds and executes an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        C__Character currentUnit = _characters.current;
        
        if(currentUnit.team.IsTeammateOf(c))
            return; // Teammate
        if(!currentUnit.look.CanSee(c))
            return; // Not visible
        if(!currentUnit.behavior.playable)
            return; // NPC
        if(!currentUnit.move.movementArea.Contains(enteredTile))
            return; // Tile not in movement area
        if(c.look.visibleTiles.Contains(enteredTile))
            HideOutOfRangeIcon();
        else
            DisplayOutOfRangeIcon();
    }
}
