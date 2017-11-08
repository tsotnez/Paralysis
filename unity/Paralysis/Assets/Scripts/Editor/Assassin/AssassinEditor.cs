using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AssassinController))]
public class AssassinEditor : ChampionClassEditor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Assassin Specific", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DoubleJumpForce"), new GUIContent("Double Jump Force"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPrefab"), new GUIContent("Skill 4 Projectile"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ambushAttack_damage"), new GUIContent("Ambush Attack Damage"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
