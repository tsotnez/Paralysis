using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InfantryContoller))]
public class InfantryEditor : ChampionClassEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Infantry Specific", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Skill1_Chain"), new GUIContent("Skill 1 Chain"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
