using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System;

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and prefab overrides.
[CustomEditor(typeof(AnimationController))]
[CanEditMultipleObjects]
public class AnimEditor : Editor {

	SerializedProperty animations, looping, customSpeeds, AnimationSpeed, CharacterClass, CharacterSkin, CharacterName, atlasses;


	float lastSpeed;
	public string path;
	DirectoryInfo levelDirectoryPath;
	void OnEnable () {
		animations = serializedObject.FindProperty ("AnimationType");

        atlasses = serializedObject.FindProperty("Atlasses");

        looping = serializedObject.FindProperty ("AnimationLoop");
		CharacterClass = serializedObject.FindProperty ("CharacterClass");
		CharacterSkin = serializedObject.FindProperty ("CharacterSkin");
		AnimationSpeed = serializedObject.FindProperty ("GeneralSpeed");
		customSpeeds = serializedObject.FindProperty ("AnimationDuration");

		lastSpeed = AnimationSpeed.floatValue;

		string path = "/Resources/Animations/chars/" + CharacterClass.stringValue + "/" + CharacterSkin.stringValue;
		 levelDirectoryPath = new DirectoryInfo(Application.dataPath + path);

	}


	public override void OnInspectorGUI() {
		serializedObject.Update ();
		EditorGUILayout.PropertyField(CharacterClass, new GUIContent("CharacterClass"), true);
		EditorGUILayout.PropertyField(CharacterSkin, new GUIContent("CharacterSkin"), true);
		EditorGUILayout.PropertyField(AnimationSpeed, new GUIContent("AnimationSpeed"), true);

        GUILayout.BeginHorizontal();
		GUILayout.Label( "Animation");
        GUILayout.Label( "Loop");
		GUILayout.Label( "Speed" );
        GUILayout.Label("Atlasses");
        GUILayout.EndHorizontal();

        animations = serializedObject.FindProperty ("AnimationType");
		looping = serializedObject.FindProperty ("AnimationLoop");
        atlasses = serializedObject.FindProperty("Atlasses");

        DirectoryInfo[] Animations = levelDirectoryPath.GetDirectories();
        atlasses.arraySize = Animations.Length;
        looping.arraySize = Animations.Length;
		animations.arraySize = Animations.Length;
		customSpeeds.arraySize = Animations.Length;

		for (int i = 0; i < Animations.Length; i++) {
			GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(looping.GetArrayElementAtIndex(i),  
				new GUIContent (Animations[i].Name), true);
            EditorGUILayout.PropertyField(customSpeeds.GetArrayElementAtIndex(i), 
				new GUIContent (""), true);
            EditorGUILayout.PropertyField(atlasses.GetArrayElementAtIndex(i),
                new GUIContent(""), true);
            GUILayout.Space(0);
			GUILayout.EndHorizontal();

            try
            {
                AnimationController.AnimatorStates xxx = (AnimationController.AnimatorStates)Enum.Parse(typeof(AnimationController.AnimatorStates), Animations[i].Name, true);
                animations.GetArrayElementAtIndex(i).enumValueIndex = (int)xxx;
            }
            catch (ArgumentException)
            {
                //Ignore
            }

            if (lastSpeed != AnimationSpeed.floatValue)
				customSpeeds.GetArrayElementAtIndex(i).floatValue = AnimationSpeed.floatValue;
		}
        serializedObject.ApplyModifiedProperties();
		lastSpeed = AnimationSpeed.floatValue;
        serializedObject.Update();

	}











}
