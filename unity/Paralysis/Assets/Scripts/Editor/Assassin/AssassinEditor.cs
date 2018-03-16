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
        serializedObject.ApplyModifiedProperties();
    }
}
