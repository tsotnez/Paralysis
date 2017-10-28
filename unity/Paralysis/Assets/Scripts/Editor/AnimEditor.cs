using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System;

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(AnimationController), true)]
[CanEditMultipleObjects]
public class AnimEditor : Editor
{
    SerializedProperty animations, looping, customSpeeds, AnimationSpeed, CharacterClass, CharacterSkin, CharacterName, atlasses;

    float lastSpeed;
    public string path;
    DirectoryInfo levelDirectoryPath;

    void OnEnable()
    {
        animations = serializedObject.FindProperty("AnimationType");
        atlasses = serializedObject.FindProperty("Atlasses");
        looping = serializedObject.FindProperty("AnimationLoop");
        CharacterClass = serializedObject.FindProperty("CharacterClass");
        CharacterSkin = serializedObject.FindProperty("CharacterSkin");
        AnimationSpeed = serializedObject.FindProperty("GeneralSpeed");
        customSpeeds = serializedObject.FindProperty("AnimationDuration");

        lastSpeed = AnimationSpeed.floatValue;
        path = Application.dataPath + "/Animations/chars/" + CharacterClass.stringValue + "/" + CharacterSkin.stringValue + "/";
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(CharacterClass, new GUIContent("CharacterClass"), true);
        EditorGUILayout.PropertyField(CharacterSkin, new GUIContent("CharacterSkin"), true);
        EditorGUILayout.PropertyField(AnimationSpeed, new GUIContent("AnimationSpeed"), true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Animation");
        GUILayout.Label("Loop");
        GUILayout.Label("Speed");
        GUILayout.Label("Atlasses");
        GUILayout.EndHorizontal();

        animations = serializedObject.FindProperty("AnimationType");
        looping = serializedObject.FindProperty("AnimationLoop");
        atlasses = serializedObject.FindProperty("Atlasses");

        // get all existing EnumTypes
        var animStates = Enum.GetValues(typeof(AnimationController.AnimatorStates));

        // set arraysize to length of enum
        atlasses.arraySize = animStates.Length;
        looping.arraySize = animStates.Length;
        animations.arraySize = animStates.Length;
        customSpeeds.arraySize = animStates.Length;

        // loop through every enum state
        int count = 0;
        foreach (var animState in animStates)
        {
            bool directoryFound = Directory.Exists(path + animState.ToString());

            GUILayout.BeginHorizontal();
            if (!directoryFound)
                EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(looping.GetArrayElementAtIndex(count), new GUIContent(animState.ToString()), true);
            EditorGUILayout.PropertyField(customSpeeds.GetArrayElementAtIndex(count), new GUIContent(""), true);
            EditorGUILayout.PropertyField(atlasses.GetArrayElementAtIndex(count), new GUIContent(""), true);

            if (!directoryFound)
                EditorGUI.EndDisabledGroup();

            GUILayout.Space(0);
            GUILayout.EndHorizontal();

            try
            {
                animations.GetArrayElementAtIndex(count).enumValueIndex = Convert.ToInt32((AnimationController.AnimatorStates)Enum.Parse(typeof(AnimationController.AnimatorStates), animState.ToString(), true));
            }
            catch (ArgumentException) { } // Ignore

            if (lastSpeed != AnimationSpeed.floatValue)
                customSpeeds.GetArrayElementAtIndex(count).floatValue = AnimationSpeed.floatValue;

            count++;
        }

        serializedObject.ApplyModifiedProperties();
        lastSpeed = AnimationSpeed.floatValue;
        serializedObject.Update();
    }
}