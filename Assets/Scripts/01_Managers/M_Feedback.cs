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

    public F_MoveLine line;
    public F_SelectionSquare square;
    public F_ViewLines viewLines;
    [SerializeField] private F_CoversHolder coverHolder;
    [SerializeField] private GameObject actionEffectPrefab;
    
    public static M_Feedback instance;
    public enum CursorType { Regular, AimAndInSight, OutAimOrSight, OutMovement, Heal } // /!\ If add/remove a cursor, update the SetCursor method

    [HideInInspector] public List<Tile> walkableTiles;
    [HideInInspector] public List<Tile> attackableTiles;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_Feedback in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    private void Start()
    {
        _input.OnTileExit += InputOnTileExit;
        _input.OnTileEnter += InputOnTileEnter;
        _input.OnChangeClickActivation += Input_ChangeClickActivation;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Shows the tiles of the movement area.
    /// </summary>
    public void ShowMovementArea(List<Tile> tilesToShow) =>
        tilesToShow
            .ForEach(t =>
            {
                t.SetMaterial(Tile.TileMaterial.Blue);
                walkableTiles.Add(t);
            });

    /// <summary>
    /// Resets the tiles skin and clears the movement area tiles list.
    /// </summary>
    public void HideMovementArea()
    {
        foreach (Tile tile in walkableTiles)
        {
            tile.ResetTileSkin();
        }

        walkableTiles.Clear();
    }

    /// <summary>
    /// Shows the tiles a character can attack.
    /// </summary>
    public void ShowAttackableTiles(List<Tile> tilesToShow) =>
        tilesToShow.
            ForEach(t => {
                t.SetMaterial(Tile.TileMaterial.Red);
                attackableTiles.Add(t);
            });

    /// <summary>
    /// Resets the tiles skin and clears the attackable tiles list.
    /// </summary>
    public void HideAttackableTiles()
    {
        attackableTiles
            .ForEach(t => t.ResetTileSkin());

        attackableTiles.Clear();
    }

    /// <summary>
    /// Disables feedbacks for moving (selection square, direction lines)
    /// </summary>
    public void HideMovementFeedbacks()
    {
        square.DisableSquare();
        line.DisableLines();
    }

    /// <summary>
    /// Show visible elements of the fog of war.
    /// </summary>
    public void ShowVisibleElements(List<Tile> visibleTiles) => SetFogVisualsActive(true, visibleTiles);

    /// <summary>
    /// Instantiates an action effect feedback prefab over the target object.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="referenceTarget"></param>
    public void ActionEffectFeedback(string text, Transform referenceTarget)
    {
        F_ActionEffect insta = Instantiate(actionEffectPrefab, transform).GetComponent<F_ActionEffect>();
        insta.SetText(text);
        insta.PositionAt(referenceTarget);
    }
    
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
    /// Actions happening if the pointer overlaps an occupied tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnOccupiedTile(Tile tile)
    {
        HideMovementFeedbacks();

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
            _ui.HidePercentText();
            SetCursor(CursorType.Regular);

            if (currentCharacter == currentTarget) 
                return; // Same character
            if(!currentCharacter.actions.HasHealAction())
                return; // Character can't heal
            if (currentTarget.health.IsFullLife())
                return; // Target is full life
            
            SetCursor(CursorType.Heal);
        }
        else // Enemy
        {
            SetCursor(CursorType.AimAndInSight);
            _ui.ShowPercentText(currentCharacter.attack.GetPercentToTouch(
                currentCharacter.look.GetTilesOfLineOfSightOn(tile.coordinates).Count,
                currentTarget.cover.GetCoverProtectionValueFrom(currentTarget.look)));        
        }
    }

    /// <summary>
    /// Actions happening if the pointer overlaps a free tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnFreeTile(Tile tile)
    {
        C__Character currentCharacter = _characters.current;
        
        // Disable fight
        ShowCharacterCoverFeedbacks(tile.coordinates);
        _ui.HidePercentText();
        
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

        bool tileInMoveRange = currentCharacter.move.CanMoveTo(tile);

        // Show movement feedbacks (square and line)
        square.SetSquare(tile.transform.position, tileInMoveRange);
        line.SetLines(
            currentPathfinding,
            currentCharacter.move.movementRange, 
            tile);

        // Set cursor
        SetCursor(tileInMoveRange ? CursorType.Regular : CursorType.OutMovement);
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
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputOnTileExit(object sender, Tile tile)
    {
        SetCursor(CursorType.OutMovement);
        HideCoverFeedbacks();
    }

    private void InputOnTileEnter(object sender, Tile tile)
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
