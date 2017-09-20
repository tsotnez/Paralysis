using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UserControl : MonoBehaviour {

    protected ChampionClassController m_Character;
    protected CharacterStats m_Stats;
    protected bool m_Jump;
    protected bool m_Attack;
    protected int dash = 0;
    protected bool defensive;

    protected bool m_Skill1;
    protected bool m_Skill2;
    protected bool m_Skill3;
    protected bool m_Skill4;


    protected void Awake()
    {
        m_Character = GetComponent<ChampionClassController>();
        m_Stats = GetComponent<CharacterStats>();
    }


    protected void Update()
    {
        defensive = Input.GetButton("Defensive");

        if (dash == 0)
        {
            if (Input.GetButtonDown("DashLeft")) dash = -1;
            else if (Input.GetButtonDown("DashRight")) dash = 1;
        }

        if (!m_Attack)
        {
            m_Attack = Input.GetButtonDown("Fire1");
        }

        if (!m_Skill1)
        {
            m_Skill1 = Input.GetButtonDown("Skill1");
        }

        if (!m_Skill2)
        {
            m_Skill2 = Input.GetButtonDown("Skill2");
        }

        if (!m_Skill3)
        {
            m_Skill3 = Input.GetButtonDown("Skill3");
        }

        if (!m_Skill4)
        {
            m_Skill4 = Input.GetButtonDown("Skill4");
        }

        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = Input.GetButtonDown("Jump");
        }
    }


    protected void FixedUpdate()
    {
        //Do things only when not stunned, defensive or being knocked back
        if (!m_Stats.stunned && !m_Stats.defensive && !m_Stats.knockedBack)
        {
            float h = Input.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h);
            m_Character.jump(m_Jump);
            m_Character.basicAttack(m_Attack);

            if (m_Skill1) m_Character.skill1();
            if (m_Skill2) m_Character.skill2();
            if (m_Skill3) m_Character.skill3();
            if (m_Skill4) m_Character.skill4();
            if (dash != 0) m_Character.StartCoroutine(m_Character.dash(dash));

        }
        m_Stats.defensive = defensive;
        m_Attack = false;
        m_Jump = false;
        m_Skill1 = false;
        m_Skill2 = false;
        m_Skill3 = false;
        m_Skill4 = false;
        dash = 0;
    }
}
