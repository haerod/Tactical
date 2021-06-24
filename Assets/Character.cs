using UnityEngine;
using System.Collections;
using static M__Managers;

public class Character : MonoBehaviour
{
    public Move move;
    public ActionPoints actionPoints;
    public CharacterBehaviour behaviour;

    public Tile GetTile()
    {
        return _terrain.GetTile(move.x, move.y);
    }
}
