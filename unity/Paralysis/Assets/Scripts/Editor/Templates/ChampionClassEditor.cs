using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChampionClassController))] 
public class ChampionClassEditor : Editor {

    SerializedProperty WhatIsGround, WhatIsFallThrough, WhatToHit, MoveSpeed, MoveSpeedWhileAttacking, MoveSpeedWhileBlocking, JumpForce, JumpAttackRadius, JumpAttackForce,
        DashSpeed, DashStaminaCost, CanDashForward, ComboExpire, ClassName, MaxJumpTime, JumpDivisor, DJumpDivisor, CharacterSkills;

    void OnEnable()
    {
        //Get properties from script
        WhatIsGround = serializedObject.FindProperty("m_WhatIsGround");
        WhatIsFallThrough = serializedObject.FindProperty("m_fallThroughMask");
        WhatToHit = serializedObject.FindProperty("m_whatToHit");
        MoveSpeed =  serializedObject.FindProperty("m_MaxSpeed");
        MoveSpeedWhileAttacking = serializedObject.FindProperty("m_MoveSpeedWhileAttacking");
        MoveSpeedWhileBlocking = serializedObject.FindProperty("m_MoveSpeedWhileBlocking");

        JumpForce = serializedObject.FindProperty("m_initialJumpVelocity");
        MaxJumpTime = serializedObject.FindProperty("m_maxJumpTime");
        JumpDivisor = serializedObject.FindProperty("m_JumpDivisor");
        DJumpDivisor = serializedObject.FindProperty("m_DoubleJumpDivisor");

        JumpAttackRadius = serializedObject.FindProperty("m_jumpAttackRadius");
        JumpAttackForce = serializedObject.FindProperty("m_jumpAttackForce");
        DashSpeed = serializedObject.FindProperty("m_dashSpeed");
        DashStaminaCost = serializedObject.FindProperty("m_dashStaminaCost");
        CanDashForward = serializedObject.FindProperty("m_CanDashForward");
        ComboExpire = serializedObject.FindProperty("ComboExpire");
        ClassName = serializedObject.FindProperty("className");
        CharacterSkills = serializedObject.FindProperty("CharacterSkills");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(WhatIsGround, new GUIContent("What Is ground"), true);
        EditorGUILayout.PropertyField(WhatIsFallThrough, new GUIContent("Can Fall Through"), true);
        EditorGUILayout.PropertyField(ClassName, new GUIContent("Class Name"), true);

        GUILayout.Label("Speeds", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Running Speed");
        GUILayout.Label("while Attacking");
        GUILayout.Label("while Blocking");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(MoveSpeed, new GUIContent(""), true);
        EditorGUILayout.PropertyField(MoveSpeedWhileAttacking, new GUIContent(""), true);
        EditorGUILayout.PropertyField(MoveSpeedWhileBlocking, new GUIContent(""), true);
        GUILayout.EndHorizontal();

        GUILayout.Label("Jumping", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Force");
        GUILayout.Label("Max Time");
        GUILayout.Label("Stamina");
        GUILayout.Label("Divisor");
        GUILayout.Label("D Jump Divisor");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(JumpForce, new GUIContent(""), true);
        EditorGUILayout.PropertyField(MaxJumpTime, new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_Jump"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(JumpDivisor, new GUIContent(""), true);
        EditorGUILayout.PropertyField(DJumpDivisor, new GUIContent(""), true);
        GUILayout.EndHorizontal();

        GUILayout.Label("Jump Attack", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Force");
        GUILayout.Label("Radius");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(JumpAttackForce, new GUIContent(""), true);
        EditorGUILayout.PropertyField(JumpAttackRadius, new GUIContent(""), true);
        GUILayout.EndHorizontal();

        GUILayout.Label("Dashing", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        GUILayout.Label("Stamina Cost");
        GUILayout.Label("Can Dash Forward");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(DashSpeed, new GUIContent(""), true);
        EditorGUILayout.PropertyField(DashStaminaCost, new GUIContent(""), true);
        EditorGUILayout.PropertyField(CanDashForward, new GUIContent(""), true);
        GUILayout.EndHorizontal();

        GUILayout.Label("Attacks & Skills", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(ComboExpire, new GUIContent("Time Combo reset"), true);
        EditorGUILayout.PropertyField(WhatToHit, new GUIContent("Which Layers to hit"), true);

        // skills
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MeleeSkills"), new GUIContent("Melee Skills"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RangeSkills"), new GUIContent("Range Skills"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
