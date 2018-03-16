using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArcherController))]
public class ArcherEditor : ChampionClassEditor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Archer Specific", EditorStyles.boldLabel);
        serializedObject.ApplyModifiedProperties();
    }
}
