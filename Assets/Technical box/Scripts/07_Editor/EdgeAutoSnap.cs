using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using static M__Managers;

[ExecuteInEditMode]
public class EdgeAutoSnap : BaseAutoSnap
{
    [SerializeField] private bool flipVisuals;

    [Header("REFERENCES")]

    [SerializeField] private Transform edgeTransform;

    [HideInInspector] public Edge edgeEntity; // Note : Let it serializable to be dirty.
    [HideInInspector] public Tile parentTile; // Note : Let it serializable to be dirty.

    private Vector3 positionBeforeMovement;
    private bool firstFrame = true;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

#if UNITY_EDITOR

    private void OnDestroy()
    {
        if (!IsInEditor())
            return; // Not editor mode
        if (!_board)
            return; // Exit prefab mode

        //board.edgeGrid.RemoveEdge(edgeEntity);
        EditorUtility.SetDirty(_board);
    }

#endif

    // ======================================================================
    // INHERITED
    // ======================================================================

    protected override void CheckGridPosition()
    {
        positionBeforeMovement = GetPositionBeforeMovement();
        base.CheckGridPosition();
    }

    protected override bool IsOnValidPosition() => IsTileUnder() && !IsOtherEdgeAtPosition();

    protected override void MoveObject(Coordinates coordinates)
    {
        transform.position = new Vector3(
            Utils.RoundToHalf(transform.position.x), 
            0, 
            Utils.RoundToHalf(transform.position.z));
    }
    
    protected override void SetParameters()
    {
        base.SetParameters();
        edgeEntity = GetComponent<Edge>();
        transform.parent = _board.transform;
        transform.hasChanged = true;
    }

    protected override void SetParametersDirty()
    {
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(_board);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns true if it's tile under the element.
    /// </summary>
    /// <returns></returns>
    private bool IsTileUnder() => Physics.OverlapSphere(transform.position, .1f)
            .Any(collider => collider.GetComponent<Tile>());

    private bool IsOtherEdgeAtPosition() => Physics.OverlapSphere(edgeTransform.position, .1f)
            .Where(collider => collider.GetComponentInParent<Edge>())
            .Select(collider => collider.GetComponentInParent<Edge>())
            .Any(edge => edge != edgeEntity);

    /// <summary>
    /// Auto rotates the edge entity.
    /// </summary>
    private void MoveCoverOnBorder()
    {
        if (firstFrame)
        {
            firstFrame = false;
            return; // Quickfix for the first frame
        }

        Vector3 positionOnSnap = positionBeforeMovement - transform.position;

        float absX = Mathf.Abs(positionOnSnap.x);
        float absZ = Mathf.Abs(positionOnSnap.z);

        if (absX > absZ)
        {
            edgeTransform.transform.localPosition = new Vector3(
                .5f * Mathf.Sign(positionOnSnap.x),
                transform.localPosition.y,
                0);
        }
        else
        {
            edgeTransform.transform.localPosition = new Vector3(
                0,
                transform.localPosition.y,
                .5f * Mathf.Sign(positionOnSnap.z));
        }

        AutoRotateCover();
    }

    /// <summary>
    /// Rotates the cover depending on the border (and the visual flipping).
    /// </summary>
    private void AutoRotateCover()
    {
        if (edgeTransform.localPosition.x % 1 == 0)
            edgeTransform.eulerAngles = Vector3.up * 90f;
        else
            edgeTransform.eulerAngles = Vector3.zero;

        if (flipVisuals)
            edgeTransform.eulerAngles += Vector3.up * 180f;
    }

}
