﻿using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Dropdown), true)]
    [CanEditMultipleObjects]
    public class DropdownEditor : SelectableEditor
    {
        SerializedProperty m_Template;
        SerializedProperty m_CaptionText;
        SerializedProperty m_CaptionImage;
        SerializedProperty m_ItemText;
        SerializedProperty m_ItemImage;
        SerializedProperty m_OnSelectionChanged;
        SerializedProperty m_SelectSelfOnClose;
        SerializedProperty m_HideOnSelectItem;
        SerializedProperty m_SelectItemsOnPointerEnter;
        SerializedProperty m_Value;
        SerializedProperty m_Options;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Template = serializedObject.FindProperty("m_Template");
            m_CaptionText = serializedObject.FindProperty("m_CaptionText");
            m_CaptionImage = serializedObject.FindProperty("m_CaptionImage");
            m_ItemText = serializedObject.FindProperty("m_ItemText");
            m_ItemImage = serializedObject.FindProperty("m_ItemImage");
            m_OnSelectionChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_SelectSelfOnClose = serializedObject.FindProperty("m_SelectSelfOnClose");
            m_HideOnSelectItem = serializedObject.FindProperty("m_HideOnSelectItem");
            m_SelectItemsOnPointerEnter = serializedObject.FindProperty("m_SelectItemsOnPointerEnter");
            m_Value = serializedObject.FindProperty("m_Value");
            m_Options = serializedObject.FindProperty("m_Options");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Template);
            EditorGUILayout.PropertyField(m_CaptionText);
            EditorGUILayout.PropertyField(m_CaptionImage);
            EditorGUILayout.PropertyField(m_ItemText);
            EditorGUILayout.PropertyField(m_ItemImage);
            EditorGUILayout.PropertyField(m_SelectSelfOnClose);
            EditorGUILayout.PropertyField(m_HideOnSelectItem);
            EditorGUILayout.PropertyField(m_SelectItemsOnPointerEnter);
            EditorGUILayout.PropertyField(m_Value);
            EditorGUILayout.PropertyField(m_Options);
            EditorGUILayout.PropertyField(m_OnSelectionChanged);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
