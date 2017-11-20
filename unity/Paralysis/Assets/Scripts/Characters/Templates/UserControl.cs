using UnityEngine;

public abstract class UserControl : MonoBehaviour
{
    public InputDevice inputDevice = InputDevice.KeyboardMouse; //Whick input device is used?
    public PlayerNumbers playerNumber = PlayerNumbers.Player1;
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

    protected bool m_Trinket1;
    protected bool m_Trinket2;


    protected float lastVerticalValue = 0; //Saves the last value for the jump-sticks horizontal input


    public enum InputDevice
    {
        XboxController,
        KeyboardMouse,
        Ps4Controller
    }

    public enum PlayerNumbers
    {
        Player1, Player2, Player3, Player4
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
                CheckInputForXboxController();
                break;
            case InputDevice.KeyboardMouse:
                CheckInputForKeyboardAndMouse();
                break;
            case InputDevice.Ps4Controller:
                CheckInputForPs4Controller();
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
        if (m_Trinket1 && typeof(UseTrinket) == m_Character.Trinket1.GetType().BaseType)
        {
            ((UseTrinket)m_Character.Trinket1).Use(m_Stats); 
            
        }
        if (m_Trinket2 && typeof(UseTrinket) == m_Character.Trinket2.GetType().BaseType)
        {
            ((UseTrinket)m_Character.Trinket2).Use(m_Stats);
        }

        //Do things only when not stunned, defensive or being knocked back
        if (!m_Stats.stunned && !m_Stats.knockedBack)
        {
            m_Character.Move(move);

            if (!defensive)
            {
                // Pass all parameters to the character control script.
                m_Character.Jump(m_Jump);
                m_Character.BasicAttack(m_Attack);

                if (m_Skill1) m_Character.Skill1();
                if (m_Skill2) m_Character.Skill2();
                if (m_Skill3) m_Character.Skill3();
                if (m_Skill4) m_Character.Skill4();
                if (dash != 0) m_Character.StartCoroutine(m_Character.Dash(dash));
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
        m_Trinket1 = false;
        m_Trinket2 = false;
        dash = 0;
    }

    #region Keyboard & Mouse

    /// <summary>
    /// Checks inputs for Keyboard and Mouse
    /// </summary>
    protected virtual void CheckInputForKeyboardAndMouse()
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

        if (!m_Trinket1)
        {
            m_Trinket1 = Input.GetButtonDown("Trinket1");
        }

        if (!m_Trinket2)
        {
            m_Trinket2 = Input.GetButtonDown("Trinket2");
        }

        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = Input.GetButtonDown("Jump");
        }
        if (!m_Stats.stunned && !m_Stats.knockedBack)
            defensive = Input.GetButton("Defensive");
        else defensive = false;

        m_Character.ManageDefensive(defensive);
    }

    #endregion

    #region XBox

    /// <summary>
    /// Checks input for Xbox Controllers
    /// </summary>
    protected virtual void CheckInputForXboxController()
    {
        //Set move to 0, 1 or -1 so character will move full speed or 0
        move = Input.GetAxis("Horizontal_Xbox" + playerNumber.ToString());
        if (move > 0) move = 1;
        else if (move < 0) move = -1;
        else move = 0;

        if (dash == 0)
        {
            if (Input.GetButtonDown("DashLeft_Xbox" + playerNumber.ToString())) dash = -1;
            else if (Input.GetButtonDown("DashRight_Xbox" + playerNumber.ToString())) dash = 1;
        }

        if (!m_Attack)
        {
            m_Attack = Input.GetAxis("BasicAttack_Xbox" + playerNumber.ToString()) < 0;
        }

        if (!m_Skill1)
        {
            m_Skill1 = Input.GetButtonDown("Skill1_Xbox" + playerNumber.ToString());
        }

        if (!m_Skill2)
        {
            m_Skill2 = Input.GetButtonDown("Skill2_Xbox" + playerNumber.ToString());
        }

        if (!m_Skill3)
        {
            m_Skill3 = Input.GetButtonDown("Skill3_Xbox" + playerNumber.ToString());
        }

        if (!m_Skill4)
        {
            m_Skill4 = Input.GetButtonDown("Skill4_Xbox" + playerNumber.ToString());
        }

        if (!m_Trinket1)
        {
            m_Trinket1 = Input.GetAxis("Trinket1_Xbox" + playerNumber.ToString()) < 0;
        }

        if (!m_Trinket2)
        {
            m_Trinket2 = Input.GetAxis("Trinket2_Xbox" + playerNumber.ToString()) > 0;
        }

        if (!m_Jump)
        {
            if (lastVerticalValue >= 0)
                m_Jump = Input.GetAxis("RightStickVertical_Xbox" + playerNumber.ToString()) < 0;
        }
        if (!m_Stats.stunned && !m_Stats.knockedBack)
            defensive = Input.GetAxis("RightStickVertical_Xbox" + playerNumber.ToString()) > 0;
        else defensive = false;

        lastVerticalValue = Input.GetAxis("RightStickVertical_Xbox" + playerNumber.ToString()); // Save last horizontal input to prevent player from spamming jumps. He needs to move the stick back in his standart position to be able to jump again
        m_Character.ManageDefensive(defensive);
    }

    #endregion

    #region PS4

    protected virtual void CheckInputForPs4Controller()
    {
        //Set move to 0, 1 or -1 so character will move full speed or 0
        move = Input.GetAxis("Horizontal_Ps4" + playerNumber.ToString());
        if (move > 0) move = 1;
        else if (move < 0) move = -1;
        else move = 0;

        if (dash == 0)
        {
            if (Input.GetButtonDown("DashLeft_Ps4" + playerNumber.ToString())) dash = -1;
            else if (Input.GetButtonDown("DashRight_Ps4" + playerNumber.ToString())) dash = 1;
        }

        if (!m_Attack)
        {
            m_Attack = Input.GetAxis("BasicAttack_Ps4" + playerNumber.ToString()) < 0;
        }

        if (!m_Skill1)
        {
            m_Skill1 = Input.GetButtonDown("Skill1_Ps4" + playerNumber.ToString());
        }

        if (!m_Skill2)
        {
            m_Skill2 = Input.GetButtonDown("Skill2_Ps4" + playerNumber.ToString());
        }

        if (!m_Skill3)
        {
            m_Skill3 = Input.GetButtonDown("Skill3_Ps4" + playerNumber.ToString());
        }

        if (!m_Skill4)
        {
            m_Skill4 = Input.GetButtonDown("Skill4_Ps4" + playerNumber.ToString());
        }

        if (!m_Jump)
        {
            if (lastVerticalValue >= 0)
                m_Jump = Input.GetAxis("RightStickVertical_Ps4" + playerNumber.ToString()) < 0;
        }
        if (!m_Stats.stunned && !m_Stats.knockedBack)
            defensive = Input.GetAxis("RightStickVertical_Ps4" + playerNumber.ToString()) > 0;
        else defensive = false;

        lastVerticalValue = Input.GetAxis("RightStickVertical_Ps4" + playerNumber.ToString()); // Save last horizontal input to prevent player from spamming jumps. He needs to move the stick back in his standart position to be able to jump again
        m_Character.ManageDefensive(defensive);
    }

    #endregion
}