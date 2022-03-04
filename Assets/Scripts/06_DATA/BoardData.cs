using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New board", menuName = "Board", order = 1)]
public class BoardData : ScriptableObject
{
    public List<TileData> board;
}

[System.Serializable]
public class TileData
{
    public int x;
    public int y;
    public Tile.Type type;
}