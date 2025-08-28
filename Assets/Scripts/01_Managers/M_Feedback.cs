using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class M_Feedback : MonoBehaviour
{
    [Header("REFERENCES")]
    
    public static M_Feedback instance;
    
    public event EventHandler<Tile> OnFreeTile;
    public event EventHandler<List<Tile>> OnMovableTile;
    public event EventHandler<Tile> OnOccupiedTile;
    public event EventHandler<C__Character> OnHoverEnemy;
    public event EventHandler<C__Character> OnHoverAlly;
    public event EventHandler OnHoverItself;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_Feedback in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }

    private void Start()
    {
        _input.OnTileEnter += Input_OnTileEnter;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Actions happening if the pointer overlaps a free tile.
    /// </summary>
    /// <param name="tile"></param>
    private void FreeTileHovered(Tile tile)
    {
        C__Character currentCharacter = _characters.current;

        OnFreeTile?.Invoke(this, tile);
        
        List<Tile> currentPathfinding = Pathfinding.GetPath(
            currentCharacter.tile,
            tile,
            Pathfinding.TileInclusion.WithStartAndEnd,
            new MovementRules(
                currentCharacter.move.walkableTiles, 
                currentCharacter.move.GetTraversableCharacterTiles(), 
                currentCharacter.move.useDiagonalMovement));

        if (currentPathfinding.Count == 0)
            return; // No path
        
        OnMovableTile?.Invoke(this, currentPathfinding);
    }
    
    /// <summary>
    /// Actions happening if the pointer overlaps a occupied by a character.
    /// </summary>
    /// <param name="tile"></param>
    private void OccupiedTileHovered(Tile tile)
    {
        C__Character currentCharacter = _characters.current;
        C__Character currentTarget = tile.character;
        
        OnOccupiedTile?.Invoke(this, tile);
        
        if (currentCharacter.team.IsAllyOf(currentTarget)) // Character or allie
        {
            if (currentCharacter == currentTarget)
                OnHoverItself?.Invoke(this, EventArgs.Empty);
            else
                OnHoverAlly?.Invoke(this, currentTarget);
        }
        else // Enemy
            OnHoverEnemy?.Invoke(this, currentTarget);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Input_OnTileEnter(object sender, Tile tile)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.move.CanWalkAt(tile.coordinates) || !currentCharacter.CanPlay()) 
            return; // Can't go on this tile or can't play
        
        bool pointedCharacterIsVisible = !_rules.IsFogOfWar() || currentCharacter.look.VisibleTiles().Contains(tile);

        if (tile.IsOccupiedByCharacter() && pointedCharacterIsVisible)
            OccupiedTileHovered(tile);
        else
            FreeTileHovered(tile);
    }
}
