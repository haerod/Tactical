using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class FM_TilesActionPreview : MonoBehaviour
{
    [HideInInspector] public List<Tile> walkableTiles;
    [HideInInspector] public List<Tile> attackableTiles;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        _rules.OnVictory += Rules_OnVictory;
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
        
        startingCharacter.move.OnMovementStart += Move_OnMovementStart;
        startingCharacter.move.OnMovementEnd += Move_OnMovementEnd;
        startingCharacter.attack.OnAttackStart += Attack_OnAttackStart;
        startingCharacter.weaponHolder.OnWeaponChange += WeaponsHolder_OnWeaponChanged;
        
        ShowMovementArea(startingCharacter.move.movementArea);
        ShowAttackableTiles(startingCharacter.attack.AttackableTiles());
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if (!endingCharacter.behavior.playable) 
            return; // NPC
        
        endingCharacter.move.OnMovementStart -= Move_OnMovementStart;
        endingCharacter.move.OnMovementEnd -= Move_OnMovementEnd;
        endingCharacter.attack.OnAttackStart -= Attack_OnAttackStart;
        endingCharacter.weaponHolder.OnWeaponChange -= WeaponsHolder_OnWeaponChanged;
        
        HideTilesFeedbacks();
    }
    
    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        C__Character currentCharacter = _characters.current;
        
        ShowMovementArea(currentCharacter.move.movementArea);
        ShowAttackableTiles(currentCharacter.attack.AttackableTiles());
    }
    
    private void WeaponsHolder_OnWeaponChanged(object sender, Weapon newWeapon)
    {
        HideAttackableTiles();
        ShowAttackableTiles(_characters.current.attack.AttackableTiles());
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }

    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }

}
