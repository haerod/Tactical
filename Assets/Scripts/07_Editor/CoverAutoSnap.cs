using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CoverAutoSnap : BaseAutoSnap
{
    [SerializeField] private bool flipVisuals;

    [Header("REFERENCES")]

    [SerializeField] private Transform coverTransform;

    [HideInInspector] public M_Board board; // Note : Let it serializable to be dirty.
    [HideInInspector] public Cover cover; // Note : Let it serializable to be dirty.
    [HideInInspector] public Tile parentTile; // Note : Let it serializable to be dirty.

    private Vector3 positionBeforeMovement;
    private bool firstFrame = true;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void OnDestroy()
    {
        if (!IsInEditor())
            return; // Not in editor mode
        if (!board)
            return; // Exit prefab mode

        RemoveFromManager();
    }

    // ======================================================================
    // INHERITED
    // ======================================================================

    protected override void CheckGridPosition()
    {
        positionBeforeMovement = GetPositionBeforeMovement();
        base.CheckGridPosition();
    }

    protected override void AddToManager()
    {
        Tile validTile = GetTileUnder();

        if (!validTile)
            return; // No valid tile

        validTile.AddCover(cover);
        parentTile = validTile;

        cover.SetCoverPosition(new Vector2(
            coverTransform.position.x, 
            coverTransform.position.z));

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(cover);
    }

    protected override bool IsOnValidPosition()
    {
        Tile validTile = GetTileUnder();
        Cover otherCover = GetOtherCoverAtPosition();

        if (!validTile || otherCover)
        {
            isLocated = false;
            return false;
        }

        return true;
    }

    protected override void MoveObject(Coordinates coordinates)
    {
        transform.position = new Vector3(coordinates.x, 0, coordinates.y);
        MoveCoverOnBorder();
    }

    protected override void SetParameters()
    {
        base.SetParameters();
        board = FindAnyObjectByType<M_Board>();
        cover = GetComponent<Cover>();
        transform.parent = board.transform;
    }

    protected override void RemoveFromManager()
    {
        if (parentTile)
        {
            parentTile.RemoveCover(cover);
            EditorUtility.SetDirty(this);
        }
    }

    protected override void SetParametersDirty()
    {
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Get the tile under the character, if it can walk on.
    /// </summary>
    /// <returns></returns>
    private Tile GetTileUnder()
    {
        Collider[] colliderTileArray = Physics.OverlapSphere(transform.position, .1f);

        foreach (Collider colliderTile in colliderTileArray)
        {
            Tile testedTile = colliderTile.GetComponent<Tile>();

            if (!testedTile)
                continue; // No tile

            return testedTile;
        }

        return null;
    }

    private Cover GetOtherCoverAtPosition()
    {
        Collider[] colliderCoverArray = Physics.OverlapSphere(coverTransform.position, .1f);

        foreach (Collider colliderCover in colliderCoverArray)
        {
            Cover testedCover = colliderCover.GetComponentInParent<Cover>();

            if (!testedCover)
                continue; // No cover
            if (testedCover == cover)
                continue; // Same cover

            return testedCover;
        }

        return null;
    }

    /// <summary>
    /// Auto rotate the cover.
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
            coverTransform.transform.localPosition = new Vector3(
                .5f * Mathf.Sign(positionOnSnap.x),
                transform.localPosition.y,
                0);
        }
        else
        {
            coverTransform.transform.localPosition = new Vector3(
                0,
                transform.localPosition.y,
                .5f * Mathf.Sign(positionOnSnap.z));
        }

        AutoRotateCover();
    }

    /// <summary>
    /// Rotate the cover depending the border (and the visual flipping).
    /// </summary>
    private void AutoRotateCover()
    {
        if (coverTransform.localPosition.x % 1 == 0)
            coverTransform.eulerAngles = Vector3.up * 90f;
        else
            coverTransform.eulerAngles = Vector3.zero;

        if (flipVisuals)
            coverTransform.eulerAngles += Vector3.up * 180f;
    }

}
