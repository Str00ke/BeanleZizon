using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonData))]
internal sealed class DungeonDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawBakeButton();
        DrawClearButton();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }

    private void DrawBakeButton()
    {
        if (GUILayout.Button("BAKE"))
        {
            DungeonTool.BakeDungeonData();
        }
    }

    private void DrawClearButton()
    {
        if (GUILayout.Button("clear"))
        {
            DungeonTool.ClearDungeonData();
        }
    }
}