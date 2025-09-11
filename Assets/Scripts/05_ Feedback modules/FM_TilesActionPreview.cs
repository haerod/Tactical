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
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        A__Action.OnAnyActionStart += Action_OnAnyActionStart;
        A__Action.OnAnyActionEnd += Action_OnAnyActionEnd;
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
    private void ShowMovementArea(List<Tile> tilesToShow) => tilesToShow
            .ForEach(t =>
            {
                t.SetMaterial(Tile.TileMaterial.Blue);
                walkableTiles.Add(t);
            });

    /// <summary>
    /// Shows the tiles a character can attack.
    /// </summary>
    private void ShowAttackableTiles(List<Tile> tilesToShow) => tilesToShow.
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
        if (!_units.current.behavior.playable)
            return; // NPC
        
        HideMovementArea();
        HideAttackableTiles();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if (!startingUnit.behavior.playable) 
            return; // NPC
        if(!startingUnit.CanPlay())
            return; // Can't play

        if (startingUnit.actions.HasMoveAction())
            ShowMovementArea(startingUnit.move.movementArea);

        if (startingUnit.actions.HasAttackAction())
        {
            startingUnit.weaponHolder.OnWeaponChange += WeaponsHolder_OnWeaponChanged;
            ShowAttackableTiles(startingUnit.attack.AttackableTiles());
        }
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if (!endingUnit.behavior.playable) 
            return; // NPC
        
        endingUnit.weaponHolder.OnWeaponChange -= WeaponsHolder_OnWeaponChanged;
        
        HideTilesFeedbacks();
    }
    
    private void WeaponsHolder_OnWeaponChanged(object sender, Weapon newWeapon)
    {
        if (!_units.current.actions.HasAttackAction())
            return; // Can't attack

        HideAttackableTiles();
        ShowAttackableTiles(_units.current.attack.AttackableTiles());
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        HideTilesFeedbacks();
    }
    
    private void Action_OnAnyActionStart(object sender, U__Unit startingActionUnit)
    {
        HideTilesFeedbacks();
    }
    
    private void Action_OnAnyActionEnd(object sender, U__Unit endingActionUnit)
    {
        if (!endingActionUnit.behavior.playable) 
            return; // NPC
        if(!endingActionUnit.CanPlay())
            return; // Can't play

        if (endingActionUnit.actions.HasMoveAction())
            ShowMovementArea(endingActionUnit.move.movementArea);

        if (endingActionUnit.actions.HasAttackAction())
            ShowAttackableTiles(endingActionUnit.attack.AttackableTiles());
    }

}
