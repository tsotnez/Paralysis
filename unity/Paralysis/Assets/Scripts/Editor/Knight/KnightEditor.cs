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

        //GUILayout.Label("Knight Specific", EditorStyles.boldLabel);
        //serializedObject.ApplyModifiedProperties();
    }
}
