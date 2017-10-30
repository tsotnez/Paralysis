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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("standartArrowPrefab"), new GUIContent("Standart Arrow"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpAttackArrowPrefab"), new GUIContent("Jump Attack Arrow"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("greatArrowPrefab"), new GUIContent("Great Arrow"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("trapPrefab"), new GUIContent("Trap"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
