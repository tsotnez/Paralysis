using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArcherController))]
public class ArcherInspector : ChampionClassEditor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Archer Specific", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("arrowPrefab"), new GUIContent("Standart Arrow"), true);
    }
}
