using UnityEngine;
using System.Collections;
using static M__Managers;

public class M_Feedback : MonoSingleton<M_Feedback>
{
    [Header("CURSORS")]

    [SerializeField] private Texture2D aimCursor = null;
    [SerializeField] private Texture2D outAimCursor = null;
    [SerializeField] private Texture2D outActionPointsCursor = null;

    [Header("REFERENCES")]

    public MoveLine_Feedback line;
    public Square_Feedback square;
    [SerializeField] private GameObject actionEffectPrefab = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void DisableFeedbacks()
    {
        square.DisableSquare();
        line.DisableLines();
        _ui.DisableActionCostText();
    }

    public enum CursorType { Regular, Aim, OutAim, OutActionPoints}
    public void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Regular:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Aim:
                Cursor.SetCursor(aimCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutAim:
                Cursor.SetCursor(outAimCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutActionPoints:
                Cursor.SetCursor(outActionPointsCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            default:
                break;
        }
    }

    public void ActionEffectFeedback(string text, Transform referenceTarget)
    {
        TextEffect_Feedback insta = Instantiate(actionEffectPrefab, transform).GetComponent<TextEffect_Feedback>();
        insta.SetText(text);
        insta.PositionAt(referenceTarget);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
