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
    // Default Properties for Animator
    SerializedProperty GeneralAnimationSpeed, CharacterClass, CharacterSkin, CharacterName;
    // Properties for Animation-Dictionarys
    SerializedProperty Animations, Atlasses, Looping, CustomSpeeds;
    // Properties for AnimationEnd-Dictionarys
    SerializedProperty EndAnimations, EndAtlasses;

    public string path;
    float lastSpeed;
    DirectoryInfo levelDirectoryPath;

    void OnEnable()
    {
        // Default Properties
        CharacterClass = serializedObject.FindProperty("CharacterClass");
        CharacterSkin = serializedObject.FindProperty("CharacterSkin");
        GeneralAnimationSpeed = serializedObject.FindProperty("GeneralSpeed");

        // Dictionary Properties for Animation
        Animations = serializedObject.FindProperty("AnimationType");
        Atlasses = serializedObject.FindProperty("Atlasses");
        Looping = serializedObject.FindProperty("AnimationLoop");
        CustomSpeeds = serializedObject.FindProperty("AnimationDuration");

        // Dictionary Properties for EndAnimation
        EndAnimations = serializedObject.FindProperty("EndAnimationType");
        EndAtlasses = serializedObject.FindProperty("EndAtlasses");

        lastSpeed = GeneralAnimationSpeed.floatValue;
        path = Application.dataPath + "/Animations/chars/" + CharacterClass.stringValue + "/" + CharacterSkin.stringValue + "/";
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(CharacterClass, new GUIContent("Character class"), true);
        EditorGUILayout.PropertyField(CharacterSkin, new GUIContent("Character skin"), true);
        EditorGUILayout.PropertyField(GeneralAnimationSpeed, new GUIContent("General animation speed"), true);

        // Dictionary Properties for Animation
        Animations = serializedObject.FindProperty("AnimationType");
        Atlasses = serializedObject.FindProperty("Atlasses");
        Looping = serializedObject.FindProperty("AnimationLoop");
        CustomSpeeds = serializedObject.FindProperty("AnimationDuration");

        // Dictionary Properties for EndAnimation
        EndAnimations = serializedObject.FindProperty("EndAnimationType");
        EndAtlasses = serializedObject.FindProperty("EndAtlasses");

        // get all existing EnumTypes
        var animStates = Enum.GetValues(typeof(AnimationController.AnimatorStates));

        // set arraysize to length of enum
        Atlasses.arraySize = animStates.Length;
        Looping.arraySize = animStates.Length;
        Animations.arraySize = animStates.Length;
        CustomSpeeds.arraySize = animStates.Length;

        EndAtlasses.arraySize = animStates.Length;
        EndAnimations.arraySize = animStates.Length;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Animation");
        GUILayout.Label("Loop");
        GUILayout.Label("Speed");
        GUILayout.Label("Atlasses");
        GUILayout.EndHorizontal();

        // loop through every enum state for Animation
        int count = 0;
        foreach (var animState in animStates)
        {
            bool directoryFound = Directory.Exists(path + animState.ToString());

            GUILayout.BeginHorizontal();
            if (!directoryFound)
                EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(Looping.GetArrayElementAtIndex(count), new GUIContent(animState.ToString()), true);
            EditorGUILayout.PropertyField(CustomSpeeds.GetArrayElementAtIndex(count), new GUIContent(""), true);
            EditorGUILayout.PropertyField(Atlasses.GetArrayElementAtIndex(count), new GUIContent(""), true);

            if (!directoryFound)
                EditorGUI.EndDisabledGroup();

            GUILayout.Space(0);
            GUILayout.EndHorizontal();

            try
            {
                Animations.GetArrayElementAtIndex(count).enumValueIndex = Convert.ToInt32((AnimationController.AnimatorStates)Enum.Parse(typeof(AnimationController.AnimatorStates), animState.ToString(), true));
            }
            catch (ArgumentException) { } // Ignore

            if (lastSpeed != GeneralAnimationSpeed.floatValue)
                CustomSpeeds.GetArrayElementAtIndex(count).floatValue = GeneralAnimationSpeed.floatValue;
            count++;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("End-Animation");
        GUILayout.Label("End-Atlasses");
        GUILayout.EndHorizontal();

        // loop through every enum state for EndAnimation
        count = 0;
        foreach (var animState in animStates)
        {
            bool directoryFound = Directory.Exists(path + animState.ToString() + "End");

            GUILayout.BeginHorizontal();
            if (!directoryFound)
                EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(EndAtlasses.GetArrayElementAtIndex(count), new GUIContent(animState.ToString() + "-End"), true);

            if (!directoryFound)
                EditorGUI.EndDisabledGroup();

            GUILayout.Space(0);
            GUILayout.EndHorizontal();

            try
            {
                Animations.GetArrayElementAtIndex(count).enumValueIndex = Convert.ToInt32((AnimationController.AnimatorStates)Enum.Parse(typeof(AnimationController.AnimatorStates), animState.ToString(), true));
            }
            catch (ArgumentException) { } // Ignore

            if (lastSpeed != GeneralAnimationSpeed.floatValue)
                CustomSpeeds.GetArrayElementAtIndex(count).floatValue = GeneralAnimationSpeed.floatValue;
            count++;
        }

        serializedObject.ApplyModifiedProperties();
        lastSpeed = GeneralAnimationSpeed.floatValue;
        serializedObject.Update();
    }
}