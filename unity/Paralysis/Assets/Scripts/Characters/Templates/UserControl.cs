using UnityEngine;

public abstract class UserControl : MonoBehaviour
{
    public InputDevice inputDevice = InputDevice.KeyboardMouse; //Whick input device is used?
    protected ChampionClassController m_Character;
    protected CharacterStats m_Stats;
    protected bool m_Jump;
    protected bool m_Attack;
    protected int dash = 0;
    protected bool defensive;
    protected float move = 0;

    protected bool m_Skill1;
    protected bool m_Skill2;
    protected bool m_Skill3;
    protected bool m_Skill4;



    public enum InputDevice
    {
        XboxController,
        KeyboardMouse,
        Ps4Controller
    }

    protected void Awake()
    {
        m_Character = GetComponent<ChampionClassController>();
        m_Stats = GetComponent<CharacterStats>();
    }

    protected void Update()
    {
        //Call Input method depending on Input device
        switch (inputDevice)
        {
            case InputDevice.XboxController:
                checkInputForXboxController();
                break;
            case InputDevice.KeyboardMouse:
                checkInputForKeyboardAndMouse();
                break;
            case InputDevice.Ps4Controller:
                break;
        }
    }

    protected void FixedUpdate()
    {
        CallMethods();
        ResetValues();
    }

    /// <summary>
    /// Calls the Methods inside of a champion class controller, depending on given Input
    /// </summary>
    protected virtual void CallMethods()
    {
        //Do things only when not stunned, defensive or being knocked back
        if (!m_Stats.stunned && !m_Stats.knockedBack)
        {
            m_Character.Move(move);

            if (!defensive)
            {
                // Pass all parameters to the character control script.
                m_Character.jump(m_Jump);
                m_Character.basicAttack(m_Attack);

                if (m_Skill1) m_Character.skill1();
                if (m_Skill2) m_Character.skill2();
                if (m_Skill3) m_Character.skill3();
                if (m_Skill4) m_Character.skill4();
                if (dash != 0) m_Character.StartCoroutine(m_Character.dash(dash));
            }
        }
    }

    protected virtual void ResetValues()
    {
        m_Attack = false;
        m_Jump = false;
        m_Skill1 = false;
        m_Skill2 = false;
        m_Skill3 = false;
        m_Skill4 = false;
        dash = 0;
    }

    /// <summary>
    /// Checks inputs for Keyboard and Mouse
    /// </summary>
    protected virtual void checkInputForKeyboardAndMouse()
    {
        move = Input.GetAxis("Horizontal");
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
        if (!m_Stats.stunned && !m_Stats.knockedBack)
            defensive = Input.GetButton("Defensive");
        else defensive = false;

        m_Character.manageDefensive(defensive);
    }
    /// <summary>
    /// Checks input for Xbox Controllers
    /// </summary>
    protected virtual void checkInputForXboxController()
    {
        move = Input.GetAxis("Horizontal_Xbox");
        if (dash == 0)
        {
            if (Input.GetButtonDown("DashLeft_Xbox")) dash = -1;
            else if (Input.GetButtonDown("DashRight_Xbox")) dash = 1;
        }

        if (!m_Attack)
        {
            m_Attack = Input.GetAxis("BasicAttack_Xbox") < 0;
        }

        if (!m_Skill1)
        {
            m_Skill1 = Input.GetButtonDown("Skill1_Xbox");
        }

        if (!m_Skill2)
        {
            m_Skill2 = Input.GetButtonDown("Skill2_Xbox");
        }

        if (!m_Skill3)
        {
            m_Skill3 = Input.GetButtonDown("Skill3_Xbox");
        }

        if (!m_Skill4)
        {
            m_Skill4 = Input.GetButtonDown("Skill4_Xbox");
        }

        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = Input.GetAxis("RightStickVertical_Xbox") < 0;
        }
        if (!m_Stats.stunned && !m_Stats.knockedBack)
            defensive = Input.GetAxis("RightStickVertical_Xbox") > 0;
        else defensive = false;

        m_Character.manageDefensive(defensive);
    }
}