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
    /// Disable visual feedbacks (selection square, direction lines, action cost text)
    /// </summary>
    public void DisableFreeTileFeedbacks()
    {
        square.DisableSquare();
        line.DisableLines();
    }
    
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
    public void SetViewLinesActive(bool value, List<Tile> tilesInView = null)
    {
        if (value)
        {
            viewLines.EnableViewLines(tilesInView);
        }
        else
        {
            viewLines.DisableViewLines();
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
