using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlchemistController))]
class AlchemistEditor : ChampionClassEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Alchemist Specific", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("GoBasicAttack"), new GUIContent("Standart Bolt"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("GoSkill1_Frostbolt"), new GUIContent("Frostbolt"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CastTime_Skill1"), new GUIContent("Cast Time Skill 1"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("GoSkill3_Stun"), new GUIContent("Stunbolt"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CastTime_Skill3"), new GUIContent("Cast Time Skill 3"), true);
        serializedObject.ApplyModifiedProperties();
    }
}