using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;


public class FM_CoversHolder : MonoBehaviour
{
    [SerializeField] private int coverFeedbackRange = 2;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject coverFeedbackPrefab;

    private List<FM_CoverWorld> coverFeedbacks;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GenerateCoverFeedbacks();
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
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
        coverFeedbacks = new List<FM_CoverWorld>();
        
        for (int i = 0; i < Mathf.Pow(coverFeedbackRange*2+1, 2); i++)
        {
            FM_CoverWorld newCoverWorld= Instantiate(coverFeedbackPrefab, transform).GetComponent<FM_CoverWorld>();
            coverFeedbacks.Add(newCoverWorld);
            newCoverWorld.Hide();
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if (!startingCharacter.behavior.playable) 
            return; // NPC
        
        startingCharacter.move.OnMovementStart += Move_OnMovementStart;
        InputEvents.OnFreeTileEnter += InputEvents_OnFreeTileEnter;
        InputEvents.OnCharacterEnter += InputEvents_OnCharacterEnter;
        InputEvents.OnNoTile += InputEvents_OnNoTile;
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if (!endingCharacter.behavior.playable) 
            return; // NPC
        
        endingCharacter.move.OnMovementStart -= Move_OnMovementStart;
        InputEvents.OnFreeTileEnter -= InputEvents_OnFreeTileEnter;
        InputEvents.OnCharacterEnter -= InputEvents_OnCharacterEnter;
        InputEvents.OnNoTile -= InputEvents_OnNoTile;
    }
    
    private void InputEvents_OnFreeTileEnter(object sender, Tile freeTile)
    {
        DisplayCoverFeedbacksAround(
            freeTile.coordinates, 
            _characters.current.cover.GetAllCoverInfosInRangeAt(freeTile.coordinates, coverFeedbackRange));
    }
    
    private void InputEvents_OnCharacterEnter(object sender, C__Character hoveredCharacter)
    {
        if(!_characters.current.look.CharactersVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
            
        DisplayTargetCoverFeedback(hoveredCharacter.cover.GetCoverStateFrom(_characters.current));
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
