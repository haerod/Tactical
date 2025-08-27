using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class F_TilesActionPreview : MonoBehaviour
{
    [HideInInspector] public List<Tile> walkableTiles;
    [HideInInspector] public List<Tile> attackableTiles;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        A_Move.OnAnyMovementEnd += Move_OnAnyMovementEnd;
        UI_WeaponSelectionButton.OnAnyWeaponChanged += WeaponSelectionButton_OnAnyWeaponChanged;
        
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        Turns.OnVictory += Turns_OnVictory;
        A_Attack.OnAnyAttackStart += Attack_OnAnyAttackStart;
        A_Move.OnAnyMovementStart += Move_OnAnyMovementStart;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the tiles of the movement area.
    /// </summary>
    private void ShowMovementArea(List<Tile> tilesToShow) =>
        tilesToShow
            .ForEach(t =>
            {
                t.SetMaterial(Tile.TileMaterial.Blue);
                walkableTiles.Add(t);
            });

    /// <summary>
    /// Shows the tiles a character can attack.
    /// </summary>
    private void ShowAttackableTiles(List<Tile> tilesToShow) =>
        tilesToShow.
            ForEach(t => {
                t.SetMaterial(Tile.TileMaterial.Red);
                attackableTiles.Add(t);
            });
    
    /// <summary>
    /// Resets the tiles skin and clears the movement area tiles list.
    /// </summary>
    private void HideMovementArea()
    {
        walkableTiles
            .ForEach(t => t.ResetTileSkin());

        walkableTiles.Clear();
    }

    /// <summary>
    /// Resets the tiles skin and clears the attackable tiles list.
    /// </summary>
    private void HideAttackableTiles()
    {
        attackableTiles
            .ForEach(t => t.ResetTileSkin());

        attackableTiles.Clear();
    }
    
    /// <summary>
    /// Clears the feedbacks on the movable tiles and the attackable tiles and clears the linked lists.
    /// </summary>
    private void HideTilesFeedbacks()
    {
        if (!_characters.current.behavior.playable)
            return; // NPC
        
        HideMovementArea();
        HideAttackableTiles();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if (!startingCharacter.behavior.playable) 
            return; // NPC
        if(!startingCharacter.CanPlay()) 
            return; // Can't play
        
        ShowMovementArea(startingCharacter.move.MovementArea());
        ShowAttackableTiles(startingCharacter.attack.AttackableTiles());
    }
    
    private void Move_OnAnyMovementEnd(object sender, EventArgs e)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.behavior.playable)
            return; // NPC
        if(!currentCharacter.CanPlay()) 
            return; // Can't play
        
        ShowMovementArea(currentCharacter.move.MovementArea());
        ShowAttackableTiles(currentCharacter.attack.AttackableTiles());
    }
    
    private void WeaponSelectionButton_OnAnyWeaponChanged(object sender, Weapon e)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.behavior.playable)
            return; // NPC
        if(!currentCharacter.CanPlay()) 
            return; // Can't play

        HideAttackableTiles();
        ShowAttackableTiles(currentCharacter.attack.AttackableTiles());
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        HideTilesFeedbacks();
    }
    
    private void Turns_OnVictory(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }

    private void Attack_OnAnyAttackStart(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }
    
    private void Move_OnAnyMovementStart(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }

}
