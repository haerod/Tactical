using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    /// <param name="fromCharacter"></param>
    /// <param name="toTile"></param>
    /// <returns></returns>
    public List<Coordinates> GetAllCoversCoordinatesFrom(C__Character fromCharacter, Tile toTile)
    {
        List<Coordinates> lineOfSight = fromCharacter.look.GetCoordinatesOfLineOfSightOn(toTile);
        List<Coordinates> coordinatesToReturn = new List<Coordinates>();

        if (lineOfSight.Count == 0)
            return coordinatesToReturn; // No line of sight
        
        foreach (Coordinates coordinates in lineOfSight)
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
    
    /// <summary>
    /// Return the closest coordinate or null if isn't.
    /// </summary>
    /// <param name="characterAttacker"></param>
    /// <returns></returns>
    public Coordinates GetClosestCoverCoordinatesFrom(C__Character characterAttacker) => GetAllCoversCoordinatesFrom(characterAttacker, c.tile)
            .LastOrDefault();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
