using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class C_Cover : MonoBehaviour
{
    [SerializeField] private List<TileType> coveringTileTypes;
    
    [Header("REFERENCES")]
    [SerializeField] private C__Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Return all coordinates of covers (over or between tiles) from a character.
    /// </summary>
    /// <param name="characterViewer"></param>
    /// <returns></returns>
    public List<Vector2Int> GetAllCoversCoordinatesFrom(C__Character characterViewer)
    {
        List<Vector2Int> lineOfSight = characterViewer.look.GetCoordinatesOfLineOfSightOn(c.tile);
        List<Vector2Int> coordinatesToReturn = new List<Vector2Int>();
        
        foreach (Vector2Int coordinates in lineOfSight)
        {
            if(coordinates == lineOfSight.Last())
                continue; // Last coordinate
            
            Tile tileAtCoordinates = _board.GetTileAtCoordinates(coordinates);

            if (tileAtCoordinates) // Get covers on tiles
                if (coveringTileTypes.Contains(tileAtCoordinates.type))
                    coordinatesToReturn.Add(coordinates);
            
            if (!_board.GetCoverBetweenAdjacentCoordinates(coordinates, lineOfSight.Next(coordinates))) // Get covers between tiles
                continue; // No cover between tiles
            
            coordinatesToReturn.Add(coordinates);
        }

        return coordinatesToReturn;
    }
    
    public Vector2Int GetClosestCoverCoordinatesFrom(C__Character characterAttacker) => GetAllCoversCoordinatesFrom(characterAttacker).Last();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
