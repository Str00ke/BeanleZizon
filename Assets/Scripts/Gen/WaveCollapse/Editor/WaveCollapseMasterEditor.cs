using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveCollapseMaster))]
public class WaveCollapseMasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Set surroundings"))
        {
            WFCSetSurroundings.ShowMyEditor();
        }
    }
}
