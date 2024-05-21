using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Feedback : MonoBehaviour
{
    [Header("CURSORS")]

    [SerializeField] private Texture2D aimCursor = null;
    [SerializeField] private Texture2D noLineOfSightCursor = null;
    [SerializeField] private Texture2D cantGoCursor = null;

    [Header("REFERENCES")]

    public F_MoveLine line;
    public F_SelectionSquare square;
    public F_ViewLines viewLines;
    [SerializeField] private GameObject actionEffectPrefab = null;
    public static M_Feedback instance;
    
    public enum CursorType { Regular, AimAndInSight, OutAimOrSight, OutMovement } // /!\ If add/remove a cursor, update the SetCusror method

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

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Show the tiles of the movement area.
    /// </summary>
    public void ShowMovementArea(List<Tile> tilesToShow) =>
        tilesToShow
            .ForEach(t =>
            {
                t.SetMaterial(Tile.TileMaterial.Grey);
                walkableTiles.Add(t);
            });

    /// <summary>
    /// Reset the tiles skin and clear the movement area tiles list.
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
    /// Show the tiles a character can attack.
    /// </summary>
    public void ShowAttackableTiles(List<Tile> tilesToShow) =>
        tilesToShow.
            ForEach(t => {
                t.SetMaterial(Tile.TileMaterial.Red);
                attackableTiles.Add(t);
            });

    /// <summary>
    /// Reset the tiles skin and clear the attackable tiles list.
    /// </summary>
    public void HideAttackableTiles()
    {
        attackableTiles
            .ForEach(t => t.ResetTileSkin());

        attackableTiles.Clear();
    }

    /// <summary>
    /// Disable visual feedbacks (selection square, direction lines, action cost text)
    /// </summary>
    public void DisableFreeTileFeedbacks()
    {
        square.DisableSquare();
        line.DisableLines();
    }

    /// <summary>
    /// Show visible elements of the fog of war.
    /// </summary>
    public void ShowVisibleElements(List<Tile> visibleTiles) => _feedback.SetFogVisualsActive(true, visibleTiles);

    /// <summary>
    /// Set cursor to its new appearance.
    /// </summary>
    /// <param name="type"></param>
    public void SetCursor(CursorType type)
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
            default:
                break;
        }
    }

    /// <summary>
    /// Instantiate an action effect feedback prefab over the target object.
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
    /// Enable/disable the view lines on border tiles and enable/disable fog mask.
    /// </summary>
    public void SetFogVisualsActive(bool value, List<Tile> tilesInView = null)
    {
        if (!_rules.enableFogOfWar) return; // EXIT : No fog of war

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

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Show and hide characters in fog of war.
    /// </summary>
    /// <param name="visibleTiles"></param>
    private void DisplayCharactersInFogOfWar(List<Tile> visibleTiles)
    {
        List<C__Character> visibleCharacters = _characters.characters
            .Where(c =>
               {
                   if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Everybody)
                       return true;
                   else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Allies)
                       return visibleTiles.Contains(c.tile) || c.team == _characters.current.team;
                   else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.InView)
                       return visibleTiles.Contains(c.tile);
                   else
                       Debug.LogError("No rule, please add one here.");

                   return false;
               })
            .ToList();

        // Show visible characters
        visibleCharacters
            .ForEach(c => c.anim.SetVisualActives(true));

        // Hide invisible characters
        _characters.characters
            .Except(visibleCharacters)
            .ToList()
            .ForEach(c => c.anim.SetVisualActives(false));
    }
}
