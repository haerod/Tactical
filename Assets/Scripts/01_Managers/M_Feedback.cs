using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class M_Feedback : MonoBehaviour
{
    [Header("COVERS")]
    
    [SerializeField] private Color coveredColour = Color.blue;
    [SerializeField] private Color uncoveredColour = Color.red;

    [Header("REFERENCES")]
    
    public F_ViewLines viewLines;
    [SerializeField] private F_CoversHolder coverHolder;
    
    public static M_Feedback instance;
    
    public event EventHandler<Tile> OnFreeTileEvent;
    public event EventHandler<List<Tile>> OnMovableTile;
    public event EventHandler<Tile> OnOccupiedTileEvent;
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
        
        _characters.GetCharacterList()
            .ForEach(character => DisplayCharacterCoverState(character, character.cover.GetCoverState()));
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Show visible elements of the fog of war.
    /// </summary>
    public void ShowVisibleElements(List<Tile> visibleTiles) => SetFogVisualsActive(true, visibleTiles);
    
    /// <summary>
    /// Enables/disables the view lines on border tiles and enables/disables fog mask.
    /// </summary>
    public void SetFogVisualsActive(bool value, List<Tile> tilesInView = null)
    {
        if (!_rules.enableFogOfWar) 
            return; // No fog of war

        if (value)
        {
            viewLines.EnableViewLines(tilesInView);
            DisplayCharactersInFogOfWar(tilesInView);
        }
        else
        {
            viewLines.DisableViewLines();
        }
    }
    
    // Cover feedbacks
    // ===============
    
    /// <summary>
    /// Returns the feedback's covered colour.
    /// </summary>
    /// <returns></returns>
    public Color GetCoveredColour() => coveredColour;
    
    /// <summary>
    /// Returns the feedback's uncovered colour.
    /// </summary>
    /// <returns></returns>
    public Color GetUncoveredColour() => uncoveredColour;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Actions happening if the pointer overlaps a free tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnFreeTile(Tile tile)
    {
        OnFreeTileEvent?.Invoke(this, tile);
        
        C__Character currentCharacter = _characters.current;
        
        // Get pathfinding
        List<Tile> currentPathfinding = Pathfinding.GetPath(
            currentCharacter.tile,
            tile,
            Pathfinding.TileInclusion.WithStartAndEnd,
            new MovementRules(currentCharacter.move.walkableTiles, currentCharacter.move.GetTraversableCharacterTiles(), currentCharacter.move.useDiagonalMovement));

        if (currentPathfinding.Count == 0)
            return; // No path
        
        OnMovableTile?.Invoke(this, currentPathfinding);
    }
    
    /// <summary>
    /// Actions happening if the pointer overlaps an occupied tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnOccupiedTile(Tile tile)
    {
        OnOccupiedTileEvent?.Invoke(this, tile);
        
        C__Character currentCharacter = _characters.current;
        C__Character currentTarget = tile.character;
        
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
    
    /// <summary>
    /// Shows and hides characters in fog of war.
    /// </summary>
    /// <param name="visibleTiles"></param>
    private void DisplayCharactersInFogOfWar(List<Tile> visibleTiles)
    {
        List<C__Character> visibleCharacters = _characters.GetCharacterList()
            .Where(c =>
               {
                   if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Everybody)
                       return true;
                   else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Allies)
                       return visibleTiles.Contains(c.tile) || c.unitTeam == _characters.current.unitTeam;
                   else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.InView)
                       return visibleTiles.Contains(c.tile);
                   else
                       Debug.LogError("No rule, please add one here.");

                   return false;
               })
            .ToList();

        // Shows visible characters
        visibleCharacters
            .ForEach(c => c.anim.SetVisualActives(true));

        // Hides invisible characters
        _characters.GetCharacterList()
            .Except(visibleCharacters)
            .ToList()
            .ForEach(c => c.anim.SetVisualActives(false));
    }
    
    /// <summary>
    /// Displays the cover state of the character on its world UI (hover it).
    /// </summary>
    /// <param name="character"></param>
    /// <param name="coverInfo"></param>
    private void DisplayCharacterCoverState(C__Character character, CoverInfo coverInfo)
    {
        if(coverInfo == null)
            character.unitUI.HideCoverState();
        else
            character.unitUI.DisplayCoverState(
                coverInfo.GetCoverType(), 
                coverInfo.GetIsCovered() ? _feedback.GetCoveredColour() : _feedback.GetUncoveredColour());
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Input_OnTileEnter(object sender, Tile tile)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.move.CanWalkAt(tile.coordinates) || !currentCharacter.CanPlay()) 
            return; // Can't go on this tile or can't play
        
        bool pointedCharacterIsVisible = !_rules.enableFogOfWar || currentCharacter.look.VisibleTiles().Contains(tile);

        if (tile.IsOccupiedByCharacter() && pointedCharacterIsVisible)
            OnOccupiedTile(tile);
        else
            OnFreeTile(tile);
    }
}
