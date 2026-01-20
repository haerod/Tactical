using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;


public class Module_CoversHolder : MonoBehaviour
{
    [SerializeField] private int coverFeedbackRange = 2;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject coverFeedbackPrefab;

    private List<Module_CoverWorld> coverFeedbacks;
    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GenerateCoverFeedbacks();
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
    }

    private void OnDisable()
    {
        if(currentUnit)
            currentUnit.move.OnMovementStart -= Move_OnMovementStart;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays all cover feedbacks around a central coordinates.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    /// <param name="coverInfos"></param>
    private void DisplayCoverFeedbacksAround(Coordinates centerCoordinates, List<CoverInfo> coverInfos)
    {
        for (int i = 0; i < coverFeedbacks.Count; i++)
        {
            if (i < coverInfos.Count)
                coverFeedbacks[i].DisplayAt(centerCoordinates, coverInfos[i]);
            else
                coverFeedbacks[i].Hide();
        }
    }
    
    /// <summary>
    /// Displays the cover infos of a character pointed by the current character.
    /// </summary>
    /// <param name="coverInfo"></param>
    private void DisplayTargetCoverFeedback(CoverInfo coverInfo)
    {
        HideCoverFeedbacks();
        coverFeedbacks[0].Display(coverInfo);
    }
    
    /// <summary>
    /// Hides all the cover feedbacks.
    /// </summary>
    private void HideCoverFeedbacks() => coverFeedbacks
        .ForEach(c => c.Hide());
    
    /// <summary>
    /// Instantiates the cover feedbacks before use it (pooling).
    /// </summary>
    private void GenerateCoverFeedbacks()
    {
        coverFeedbacks = new List<Module_CoverWorld>();
        
        for (int i = 0; i < Mathf.Pow(coverFeedbackRange*2+1, 2); i++)
        {
            Module_CoverWorld newCoverWorld= Instantiate(coverFeedbackPrefab, transform).GetComponent<Module_CoverWorld>();
            coverFeedbacks.Add(newCoverWorld);
            newCoverWorld.Hide();
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if (!startingUnit.behavior.playable) 
            return; // NPC

        currentUnit = startingUnit;
        startingUnit.move.OnMovementStart += Move_OnMovementStart;
        InputEvents.OnFreeTileEnter += InputEvents_OnFreeTileEnter;
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnNoTile += InputEvents_OnNoTile;
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if (!endingUnit.behavior.playable) 
            return; // NPC
        
        endingUnit.move.OnMovementStart -= Move_OnMovementStart;
        InputEvents.OnFreeTileEnter -= InputEvents_OnFreeTileEnter;
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
        InputEvents.OnNoTile -= InputEvents_OnNoTile;
        currentUnit = null;
    }
    
    private void InputEvents_OnFreeTileEnter(object sender, Tile freeTile)
    {
        DisplayCoverFeedbacksAround(
            freeTile.coordinates, 
            _units.current.cover.GetAllCoverInfosInRangeAt(freeTile.coordinates, coverFeedbackRange));
    }
    
    private void InputEvents_OnUnitEnter(object sender, Unit hoveredCharacter)
    {
        if(!_units.current.look.UnitsVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
            
        DisplayTargetCoverFeedback(hoveredCharacter.cover.GetCoverStateFrom(_units.current));
    }
    
    private void InputEvents_OnNoTile(object sender, EventArgs e)
    {
        HideCoverFeedbacks();
    }
    
    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        HideCoverFeedbacks();
    }
}