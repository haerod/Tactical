using UnityEngine;
using System.Collections;
using static M__Managers;

public class M_Feedback : MonoBehaviour
{
    [Header("CURSORS")]

    [SerializeField] private Texture2D aimCursor = null;
    [SerializeField] private Texture2D noLineOfSightCursor = null;
    [SerializeField] private Texture2D outOfActionPointsCursor = null;

    [Header("REFERENCES")]

    public MoveLine_Feedback line;
    public Square_Feedback square;
    [SerializeField] private GameObject actionEffectPrefab = null;
    public static M_Feedback instance;
    
    public enum CursorType { Regular, AimOrInSight, OutAimOrSight, OutActionPoints } // /!\ If add/remove a cursor, update the SetCusror method

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
    public void DisableFeedbacks()
    {
        square.DisableSquare();
        line.DisableLines();
        _ui.DisableActionCostText();
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
            case CursorType.AimOrInSight:
                Cursor.SetCursor(aimCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutAimOrSight:
                Cursor.SetCursor(noLineOfSightCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutActionPoints:
                Cursor.SetCursor(outOfActionPointsCursor, new Vector2(16, 16), CursorMode.Auto);
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
        ActionEffect_Feedback insta = Instantiate(actionEffectPrefab, transform).GetComponent<ActionEffect_Feedback>();
        insta.SetText(text);
        insta.PositionAt(referenceTarget);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
