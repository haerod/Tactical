using UnityEngine;
using System.Collections;
using static M__Managers;

public class M_Feedbacks : MonoSingleton<M_Feedbacks>
{
    [Header("MOVE FEEDBACKS")]

    public MoveLine_Feedback line;
    public Square_Feedback square;

    [Header("CURSORS")]

    [SerializeField] private Texture2D aimCursor = null;
    [SerializeField] private Texture2D outAimCursor = null;

    public void DisableFeedbacks()
    {
        square.DisableSquare();
        line.DisableLines();
        _ui.DisableActionCostText();
    }

    public enum CursorType { Regular, Aim, OutAim}
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
            default:
                break;
        }
    }
}
