using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using static M__Managers;

[CustomEditor(typeof(M_Board))]
public class M_BoardCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        M_Board board = (M_Board)target;

        if (GUILayout.Button("Bake"))
        {
            board.BakeBoard();
            //EditorUtility.SetDirty(board);
        }
        DrawDefaultInspector();
    }
}
