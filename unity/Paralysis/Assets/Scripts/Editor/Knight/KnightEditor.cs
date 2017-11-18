using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KnightController))]
public class KnightEditor : ChampionClassEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Knight Specific", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Skill4_Spear"), new GUIContent("Skill 4 Projectile"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
