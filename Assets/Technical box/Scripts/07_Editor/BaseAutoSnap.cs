using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public abstract class BaseAutoSnap : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private Vector3 gizmoSize = Vector3.one;
    [SerializeField] private Vector3 gizmoOffset = Vector3.zero;

    private enum SnapOn { Tile, Edge, Vertex }
    [SerializeField] private SnapOn snapOn = SnapOn.Tile;
    
    public bool isLocated = true; // Note : Let it serializable to be dirty.

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
#if UNITY_EDITOR
    protected void Awake()
    {
        if (!IsInEditor())
            return;

        SetParameters();
    }

    protected void Update()
    {
        if (!IsInEditor())
            return; // Not in editor
        if (!transform.hasChanged)
            return; // Didnt move

        CheckGridPosition();
        SetParametersDirty();
    }

    protected virtual void OnDrawGizmos()
    {
        if (!IsInEditor())
            return; // In editor
        if (isLocated)
            return; // Is located

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position + gizmoOffset, gizmoSize);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check if the tile is snapping somewhere
    /// </summary>
    protected virtual void CheckGridPosition()
    {
        Coordinates coordinates;

        switch (snapOn)
        {
            case SnapOn.Tile:
               coordinates = new Coordinates(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.z));
                break;
            case SnapOn.Edge:
                coordinates = new Coordinates(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.z));
                break;
            case SnapOn.Vertex:
                coordinates = new Coordinates(
                    Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.z));                
                break;
            default:
                coordinates = null;
                break;
        }
        
        MoveObject(coordinates);

        if (IsOnValidPosition())
        {
            isLocated = true;
        }
        else
        {
            isLocated = false;
        }

        transform.hasChanged = false;
    }

    /// <summary>
    /// Move the object and setup parameters if necessary.
    /// </summary>
    protected abstract void MoveObject(Coordinates coordinates);

    /// <summary>
    /// Set the base parameters of the script.
    /// </summary>
    protected virtual void SetParameters() => transform.hasChanged = true;

    /// <summary>
    /// Return true if the object is placed at a valid position.
    /// </summary>
    /// <returns></returns>
    protected abstract bool IsOnValidPosition();

    /// <summary>
    /// Set necessitating parameters dirty. UTIL: If you want a parameter to be dirty, let it be serializable.
    /// </summary>
    protected abstract void SetParametersDirty();

    /// <summary>
    ///  Return true if application is not in play mode, play mode transition or prefab mode.
    /// </summary>
    /// <returns></returns>
    protected bool IsInEditor()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return false; // Play mode
        if (PrefabStageUtility.GetCurrentPrefabStage())
            return false; // Prefab mode

        return true;
    }

    /// <summary>
    /// Get the object position before the movement
    /// </summary>
    protected Vector3 GetPositionBeforeMovement() => transform.position;
#endif
}
