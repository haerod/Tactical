using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class M_Feedback : MonoBehaviour
{
    [Header("CURSORS")]

    [SerializeField] private Texture2D aimCursor;
    [SerializeField] private Texture2D noLineOfSightCursor;
    [SerializeField] private Texture2D cantGoCursor;
    [SerializeField] private Texture2D healCursor;
    
    [Header("COVERS")]
    
    [SerializeField] private int coverFeedbackRange = 2;
    [SerializeField] private Color coveredColour = Color.blue;
    [SerializeField] private Color uncoveredColour = Color.red;

    [Header("REFERENCES")]
    
    public F_ViewLines viewLines;
    [SerializeField] private F_CoversHolder coverHolder;
    
    public static M_Feedback instance;
    public enum CursorType { Regular, AimAndInSight, OutAimOrSight, OutMovement, Heal } // /!\ If add/remove a cursor, update the SetCursor method

    [HideInInspector] public List<Tile> walkableTiles;
    [HideInInspector] public List<Tile> attackableTiles;

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
        _input.OnTileExit += Input_OnTileExit;
        _input.OnTileEnter += Input_OnTileEnter;
        _input.OnChangeClickActivation += Input_ChangeClickActivation;
        
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
    
    /// <summary>
    /// Returns the cover feedback range.
    /// </summary>
    /// <returns></returns>
    public int GetCoverFeedbackRange() => coverFeedbackRange;
    
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
        
        // Disable fight
        ShowCharacterCoverFeedbacks(tile.coordinates);
        
        // Get pathfinding
        List<Tile> currentPathfinding = Pathfinding.GetPath(
            currentCharacter.tile,
            tile,
            Pathfinding.TileInclusion.WithStartAndEnd,
            new MovementRules(currentCharacter.move.walkableTiles, currentCharacter.move.GetTraversableCharacterTiles(), currentCharacter.move.useDiagonalMovement));

        if (currentPathfinding.Count == 0)
        {
            SetCursor(CursorType.OutMovement);
            return; // No path
        }
        
        OnMovableTile?.Invoke(this, currentPathfinding);

        bool tileInMoveRange = currentCharacter.move.CanMoveTo(tile);

        // Set cursor
        SetCursor(tileInMoveRange ? CursorType.Regular : CursorType.OutMovement);
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

        ShowTargetCoverFeedbacks(currentTarget);
        
        if (!currentCharacter.look.HasSightOn(tile))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Character not in sight
        }
        
        if (currentCharacter.team.IsAllyOf(currentTarget)) // Character or allie
        {
            SetCursor(CursorType.Regular);

            if (currentCharacter == currentTarget)
            {
                OnHoverItself?.Invoke(this, EventArgs.Empty);
                return; // Same character
            }
            OnHoverAlly?.Invoke(this, currentTarget);
            
            if(!currentCharacter.actions.HasHealAction())
                return; // Character can't heal
            if (currentTarget.health.IsFullLife())
                return; // Target is full life
            
            SetCursor(CursorType.Heal);
        }
        else // Enemy
        {
            OnHoverEnemy?.Invoke(this, currentTarget);
            
            if(!currentCharacter.attack.AttackableTiles().Contains(currentTarget.tile))
                return;
            
            SetCursor(CursorType.AimAndInSight);
        }
    }
    
    /// <summary>
    /// Sets cursor to its new appearance.
    /// </summary>
    /// <param name="type"></param>
    private void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Regular:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.AimAndInSight:
                Cursor.SetCursor(aimCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutAimOrSight:
                Cursor.SetCursor(noLineOfSightCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutMovement:
                Cursor.SetCursor(cantGoCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.Heal:
                Cursor.SetCursor(healCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            default:
                break;
        }
    }
    
    /// <summary>
    /// Shows cover feedbacks of a character.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    private void ShowCharacterCoverFeedbacks(Coordinates centerCoordinates) => 
        coverHolder.DisplayCoverFeedbacksAround(centerCoordinates, _characters.current.cover.GetAllCoverInfosInRangeAt(centerCoordinates, GetCoverFeedbackRange()));

    /// <summary>
    /// Shows the cover feedback of a targeted character.
    /// </summary>
    /// <param name="targetCharacter"></param>
    private void ShowTargetCoverFeedbacks(C__Character targetCharacter) => 
        coverHolder.DisplayTargetCoverFeedback(targetCharacter.cover.GetCoverStateFrom(_characters.current));
    
    /// <summary>
    /// Hides cover feedbacks.
    /// </summary>
    private void HideCoverFeedbacks() => coverHolder.HideCoverFeedbacks();
    
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
    
    private void Input_OnTileExit(object sender, Tile tile)
    {
        SetCursor(CursorType.OutMovement);
        HideCoverFeedbacks();
    }

    private void Input_OnTileEnter(object sender, Tile tile)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.move.CanWalkAt(tile.coordinates) || !currentCharacter.CanPlay()) 
        {
            SetCursor(CursorType.OutMovement);
            return; // Can't go on this tile or can't play
        }
        
        bool pointedCharacterIsVisible = !_rules.enableFogOfWar || currentCharacter.look.VisibleTiles().Contains(tile);

        if (tile.IsOccupiedByCharacter() && pointedCharacterIsVisible)
            OnOccupiedTile(tile);
        else
            OnFreeTile(tile);
    }
    
    private void Input_ChangeClickActivation(object sender, bool canClickValue)
    {
        if(!canClickValue)
            SetCursor(CursorType.Regular);
    }
}
