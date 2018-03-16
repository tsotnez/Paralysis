using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlchemistController))]
class AlchemistEditor : ChampionClassEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Alchemist Specific", EditorStyles.boldLabel);
        serializedObject.ApplyModifiedProperties();
    }
}