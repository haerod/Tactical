using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_TileToReachFeedback : MonoBehaviour
{
    [SerializeField] private GameObject tileToReachFeedbackPrefab;
    [SerializeField] private Color gizmoColor = Color.yellow;
    
    private List<GameObject> tileToReachFeedbacks = new();
    private M_Level level;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        if(_Level.victoryCondition != M_Level.VictoryCondition.ReachZone)
            return; // Not the good victory condition
        
        Show();
    }
    
    private void OnDrawGizmos()
    {
        if(_Level.victoryCondition != M_Level.VictoryCondition.ReachZone)
            return; // Not the good victory condition
        
        Gizmos.color = gizmoColor;
        
        if (!level)
            level = _Level;

        level.tilesToReach.ForEach(tile => Gizmos.DrawCube(tile.transform.position + Vector3.up * .05f, new Vector3(1f, .1f, 1f)));
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the feedbacks.
    /// </summary>
    private void Show()
    {
        Hide();
        
        for (int i = 0; i < _Level.tilesToReach.Count; i++)
        {
            if(tileToReachFeedbacks.Count > i)
                SetupTileToReachFeedback(tileToReachFeedbacks[i], _Level.tilesToReach[i]);
            else
                AddNewFeedback(_Level.tilesToReach[i]);
        }
    }
    
    /// <summary>
    /// Hides the feedbacks.
    /// </summary>
    private void Hide() => tileToReachFeedbacks.ForEach(feedback => feedback.SetActive(false));
    
    /// <summary>
    /// Positions the given feedback at the given tile position and enables it. 
    /// </summary>
    /// <param name="feedback"></param>
    /// <param name="tile"></param>
    private void SetupTileToReachFeedback(GameObject feedback, Tile tile)
    {
        feedback.transform.position = tile.transform.position;
        feedback.SetActive(true);
    }
    
    /// <summary>
    /// Instantiates a feedback at the given tile position.
    /// </summary>
    /// <param name="tile"></param>
    private void AddNewFeedback(Tile tile) => tileToReachFeedbacks
        .Add(Instantiate(tileToReachFeedbackPrefab, tile.transform.position, Quaternion.identity, transform));
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
