using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.U2D;

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(AnimationController), true)]
[CanEditMultipleObjects]
public class AnimEditor : Editor
{
    // Default Properties for Animator
    SerializedProperty CharacterClass, CharacterSkin, DebugLogging;

    // Properties for Animation
    SerializedProperty AnimPlayType, StartAnimDuration, DefaultAnimDuration, EndAnimDuration;

    void OnEnable()
    {
        // Default Properties
        CharacterClass = serializedObject.FindProperty("CharacterClass");
        CharacterSkin = serializedObject.FindProperty("CharacterSkin");
        DebugLogging = serializedObject.FindProperty("DebugLogging");

        // Properties for Animation
        AnimPlayType = serializedObject.FindProperty("AnimPlayType");
        StartAnimDuration = serializedObject.FindProperty("StartAnimDuration");
        DefaultAnimDuration = serializedObject.FindProperty("DefaultAnimDuration");
        EndAnimDuration = serializedObject.FindProperty("EndAnimDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(CharacterClass, new GUIContent("Character class"), true);
        EditorGUILayout.PropertyField(CharacterSkin, new GUIContent("Character skin"), true);
        EditorGUILayout.PropertyField(DebugLogging, new GUIContent("Debug Logging"), true);

        // Draw Header
        GUILayout.BeginHorizontal();
        GUILayout.Label("Animation", EditorStyles.boldLabel);
        GUILayout.Label("Play Type", EditorStyles.boldLabel);
        GUILayout.Label("Speed Start", EditorStyles.boldLabel);
        GUILayout.Label("Speed Default", EditorStyles.boldLabel);
        GUILayout.Label("Speed End", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        // get all existing EnumTypes and set arraysize to length of enum
        Array animStates = Enum.GetValues(typeof(AnimationController.AnimationTypes));
        AnimPlayType.arraySize = animStates.Length;
        StartAnimDuration.arraySize = animStates.Length;
        DefaultAnimDuration.arraySize = animStates.Length;
        EndAnimDuration.arraySize = animStates.Length;

        // Build path to Animations
        string AtlasPath = "Animations/" + CharacterClass.stringValue + "/" + CharacterSkin.stringValue + "/";
        string AnimationTypeName = "";

        // loop through every enum state for Animation
        bool StartAnimAtlasFound, DefaultAnimAtlasFound, EndAnimAtlasFound;
        for (int i = 0; i < animStates.Length; i++)
        {
            // Check for Atlasses
            AnimationTypeName = ((AnimationController.AnimationTypes)i).ToString();
            StartAnimAtlasFound = Resources.Load<Sprite>(AtlasPath + AnimationTypeName + "Start") != null;
            DefaultAnimAtlasFound = Resources.Load<Sprite>(AtlasPath + AnimationTypeName) != null;
            EndAnimAtlasFound = Resources.Load<Sprite>(AtlasPath + AnimationTypeName + "End") != null;

            // Start Horizontal Drawing
            GUILayout.BeginHorizontal();

            // Disable Group if Atlas is not present in Resources
            if (!DefaultAnimAtlasFound)
                EditorGUI.BeginDisabledGroup(true);

            // Draw Label and AnimationPlayType
            EditorGUILayout.PropertyField(AnimPlayType.GetArrayElementAtIndex(i), new GUIContent(AnimationTypeName), true);

            // Draw Start Animation Duration
            if (!StartAnimAtlasFound && DefaultAnimAtlasFound) GUI.enabled = false;
            EditorGUILayout.PropertyField(StartAnimDuration.GetArrayElementAtIndex(i), new GUIContent(""), true);
            if (!StartAnimAtlasFound && DefaultAnimAtlasFound) GUI.enabled = true;

            // Draw Default Animation Duration
            EditorGUILayout.PropertyField(DefaultAnimDuration.GetArrayElementAtIndex(i), new GUIContent(""), true);

            // Draw End Animation Duration
            if (!EndAnimAtlasFound && DefaultAnimAtlasFound) GUI.enabled = false;
            EditorGUILayout.PropertyField(EndAnimDuration.GetArrayElementAtIndex(i), new GUIContent(""), true);
            if (!EndAnimAtlasFound && DefaultAnimAtlasFound) GUI.enabled = true;

            // End Disable Group if Atlas is not present in Resources
            if (!DefaultAnimAtlasFound)
                EditorGUI.EndDisabledGroup();

            // End Horizontal Drawing
            GUILayout.Space(0);
            GUILayout.EndHorizontal();
        }

        // Apply Changes
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}