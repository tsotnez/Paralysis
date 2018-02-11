using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChampionClassController))] 
public class ChampionClassEditor : Editor {

    SerializedProperty WhatIsGround, WhatIsFallThrough, WhatToHit, MoveSpeed, MoveSpeedWhileAttacking, MoveSpeedWhileBlocking, JumpForce, JumpAttackRadius, JumpAttackForce,
        DashSpeed, DashStaminaCost, CanDashForward, ComboExpire, ClassName, DoubleJumpDivisor, JumpAcceleration, MaxJumpTime;

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
        JumpAcceleration = serializedObject.FindProperty("m_jumpMaxAccel");
        MaxJumpTime = serializedObject.FindProperty("m_maxJumpTime");
        DoubleJumpDivisor = serializedObject.FindProperty("m_doubleJumpDivsor");

        JumpAttackRadius = serializedObject.FindProperty("m_jumpAttackRadius");
        JumpAttackForce = serializedObject.FindProperty("m_jumpAttackForce");
        DashSpeed = serializedObject.FindProperty("m_dashSpeed");
        DashStaminaCost = serializedObject.FindProperty("m_dashStaminaCost");
        CanDashForward = serializedObject.FindProperty("CanDashForward");
        ComboExpire = serializedObject.FindProperty("ComboExpire");
        ClassName = serializedObject.FindProperty("className");

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
        GUILayout.Label("Jump force");
        GUILayout.Label("Max Jump Accel");
        GUILayout.Label("Max Jump Time");
        GUILayout.Label("DoubleJump Divisor");
        GUILayout.Label("JumpAttack Force");
        GUILayout.Label("JumpAttack Radius");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(JumpForce, new GUIContent(""), true);
        EditorGUILayout.PropertyField(JumpAcceleration, new GUIContent(""), true);
        EditorGUILayout.PropertyField(MaxJumpTime, new GUIContent(""), true);
        EditorGUILayout.PropertyField(DoubleJumpDivisor, new GUIContent(""), true);
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

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name of the Skill     ");
        GUILayout.Label("Delay");
        GUILayout.Label("Stamina");
        GUILayout.Label("DMG");
        GUILayout.Label("CD");
        GUILayout.EndHorizontal();

        //List all Skills in horizontal rows
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_BasicAttack1"), new GUIContent("Basic Attack 1"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_BasicAttack1"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_BasicAttack1"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_BasicAttack1"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_BasicAttack2"), new GUIContent("Basic Attack 2"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_BasicAttack2"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_BasicAttack2"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_BasicAttack2"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_BasicAttack3"), new GUIContent("Basic Attack 3"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_BasicAttack3"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_BasicAttack3"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_BasicAttack3"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_JumpAttack"), new GUIContent("Jump Attack"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_JumpAttack"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_JumpAttack"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_JumpAttack"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_Skill1"), new GUIContent("Skill 1"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_Skill1"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_Skill1"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_Skill1"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_Skill2"), new GUIContent("Skill 2"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_Skill2"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_Skill2"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_Skill2"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_Skill3"), new GUIContent("Skill 3"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_Skill3"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_Skill3"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_Skill3"), new GUIContent(""), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delay_Skill4"), new GUIContent("Skill 4"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stamina_Skill4"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage_Skill4"), new GUIContent(""), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown_Skill4"), new GUIContent(""), true);
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
