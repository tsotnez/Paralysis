using UnityEngine;

public abstract class UserControl : MonoBehaviour
{
    public InputDevice inputDevice = InputDevice.KeyboardMouse; // Whick input device is used?
    public PlayerNumbers playerNumber = PlayerNumbers.Player1;
    protected ChampionClassController GoCharacter;
    protected CharacterStats CharStats;
    protected float lastVerticalValue = 0;                      // Saves the last value for the jump-sticks horizontal input

    protected bool inputJump;
    protected bool inputAttack;
    protected int inputDashDirection = 0;
    protected bool inputDown;
    protected float inputMove = 0;
    protected bool inputSkill1;
    protected bool inputSkill2;
    protected bool inputSkill3;
    protected bool inputSkill4;
    protected bool inputTrinket1;
    protected bool inputTrinket2;

    protected PhotonView photonV;

    #region Enums

    public enum InputDevice
    {
        XboxController, KeyboardMouse, Ps4Controller, AI
    }

    public enum PlayerNumbers
    {
        Player1, Player2, Player3, Player4
    }

    #endregion

    #region Default

    protected void Awake()
    {
        GoCharacter = GetComponent<ChampionClassController>();
        CharStats = GetComponent<CharacterStats>();
        photonV = GetComponent<PhotonView>();

        if(!PhotonNetwork.offlineMode && !photonV.isMine)
        {
            enabled = false;
        }
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
            case InputDevice.AI:
                //TODO
                break;
        }

        //If the player isn't falling through, then check blocking
        if(!CheckFallThrough())
        {
            GoCharacter.ManageDefensive(inputDown);
        }

    }

    protected void FixedUpdate()
    {
        if (!CharStats.CharacterDied)
        {
            CallMethods();
            ResetValues();
        }
    }

    #endregion

    /// <summary>
    /// Calls the Methods inside of a champion class controller, depending on given Input
    /// </summary>
    protected virtual void CallMethods()
    {
        if (inputTrinket1 && typeof(UseTrinket) == GoCharacter.Trinket1.GetType().BaseType)
        {
            ((UseTrinket)GoCharacter.Trinket1).Use(CharStats);

        }
        if (inputTrinket2 && typeof(UseTrinket) == GoCharacter.Trinket2.GetType().BaseType)
        {
            ((UseTrinket)GoCharacter.Trinket2).Use(CharStats);
        }

        // Always send input to Move --> Move handles all effects by itself
        GoCharacter.Move(inputMove);

        //Do things only when not stunned, defensive or being knocked back
        if (!CharStats.stunned && !CharStats.knockedBack && !GoCharacter.dashing)
        {
            // Pass all parameters to the character control script.
            if (inputAttack)
            {
                if (GoCharacter.doubleJumped)
                {
                    StartCoroutine(GoCharacter.JumpAttack());
                }
                else
                {
                    GoCharacter.BasicAttack();
                }
            }

            if (inputSkill1) GoCharacter.Skill1();
            if (inputSkill2) GoCharacter.Skill2();
            if (inputSkill3) GoCharacter.Skill3();
            if (inputSkill4) GoCharacter.Skill4();

            if (!inputDown)
            {
                GoCharacter.Jump(inputJump);
                if (inputDashDirection != 0) GoCharacter.StartCoroutine(GoCharacter.Dash(inputDashDirection));
            }
            
        }
    }

    /// <summary>
    /// Checks to see if the player has tapped fall through button
    /// Returns whether or not the character actually fell through
    /// </summary>
    protected virtual bool CheckFallThrough()
    {
        //Occurs whent the last input was down and the play selects
        //dash in any direcction
        if (inputDown && inputDashDirection != 0)
        {
            return GoCharacter.CheckFallThrough();
        }
        return false;
    }

    protected virtual void ResetValues()
    {
        inputAttack = false;
        inputJump = false;
        inputSkill1 = false;
        inputSkill2 = false;
        inputSkill3 = false;
        inputSkill4 = false;
        inputTrinket1 = false;
        inputTrinket2 = false;
        inputDashDirection = 0;
    }

    #region Keyboard & Mouse

    /// <summary>
    /// Checks inputs for Keyboard and Mouse
    /// </summary>
    protected virtual void CheckInputForKeyboardAndMouse()
    {
        inputMove = Input.GetAxis("Horizontal");
        if (inputDashDirection == 0)
        {
            if (Input.GetButtonDown("DashLeft")) inputDashDirection = -1;
            else if (Input.GetButtonDown("DashRight")) inputDashDirection = 1;
        }

        if (!inputAttack)
        {
            inputAttack = Input.GetButtonDown("Fire1");
        }

        if (!inputSkill1)
        {
            inputSkill1 = Input.GetButtonDown("Skill1");
        }

        if (!inputSkill2)
        {
            inputSkill2 = Input.GetButtonDown("Skill2");
        }

        if (!inputSkill3)
        {
            inputSkill3 = Input.GetButtonDown("Skill3");
        }

        if (!inputSkill4)
        {
            inputSkill4 = Input.GetButtonDown("Skill4");
        }

        if (!inputTrinket1)
        {
            inputTrinket1 = Input.GetButtonDown("Trinket1");
        }

        if (!inputTrinket2)
        {
            inputTrinket2 = Input.GetButtonDown("Trinket2");
        }

        if (!inputJump)
        {
            // Read the jump input in Update so button presses aren't missed.
            inputJump = Input.GetButtonDown("Jump");
        }
        if (!CharStats.stunned && !CharStats.knockedBack)
            inputDown = Input.GetButton("Defensive");
        else inputDown = false;
    }

    #endregion

    #region XBox

    /// <summary>
    /// Checks input for Xbox Controllers
    /// </summary>
    protected virtual void CheckInputForXboxController()
    {
        //Set move to 0, 1 or -1 so character will move full speed or 0
        inputMove = Input.GetAxis("Horizontal_Xbox" + playerNumber.ToString());
        if (inputMove > 0) inputMove = 1;
        else if (inputMove < 0) inputMove = -1;
        else inputMove = 0;

        if (inputDashDirection == 0)
        {
            if (Input.GetButtonDown("DashLeft_Xbox" + playerNumber.ToString())) inputDashDirection = -1;
            else if (Input.GetButtonDown("DashRight_Xbox" + playerNumber.ToString())) inputDashDirection = 1;
        }

        if (!inputAttack)
        {
            inputAttack = Input.GetAxis("BasicAttack_Xbox" + playerNumber.ToString()) < 0;
        }

        if (!inputSkill1)
        {
            inputSkill1 = Input.GetButtonDown("Skill1_Xbox" + playerNumber.ToString());
        }

        if (!inputSkill2)
        {
            inputSkill2 = Input.GetButtonDown("Skill2_Xbox" + playerNumber.ToString());
        }

        if (!inputSkill3)
        {
            inputSkill3 = Input.GetButtonDown("Skill3_Xbox" + playerNumber.ToString());
        }

        if (!inputSkill4)
        {
            inputSkill4 = Input.GetButtonDown("Skill4_Xbox" + playerNumber.ToString());
        }

        if (!inputTrinket1)
        {
            inputTrinket1 = Input.GetAxis("Trinket1_Xbox" + playerNumber.ToString()) < 0;
        }

        if (!inputTrinket2)
        {
            inputTrinket2 = Input.GetAxis("Trinket2_Xbox" + playerNumber.ToString()) > 0;
        }

        if (!inputJump)
        {
            if (lastVerticalValue >= 0)
                inputJump = Input.GetAxis("RightStickVertical_Xbox" + playerNumber.ToString()) < 0;
        }
        if (!CharStats.stunned && !CharStats.knockedBack)
            inputDown = Input.GetAxis("RightStickVertical_Xbox" + playerNumber.ToString()) > 0;
        else inputDown = false;

        lastVerticalValue = Input.GetAxis("RightStickVertical_Xbox" + playerNumber.ToString()); // Save last horizontal input to prevent player from spamming jumps. He needs to move the stick back in his standart position to be able to jump again
    }

    #endregion

    #region PS4

    protected virtual void CheckInputForPs4Controller()
    {
        //Set move to 0, 1 or -1 so character will move full speed or 0
        inputMove = Input.GetAxis("Horizontal_Ps4" + playerNumber.ToString());
        if (inputMove > 0) inputMove = 1;
        else if (inputMove < 0) inputMove = -1;
        else inputMove = 0;

        if (inputDashDirection == 0)
        {
            if (Input.GetButtonDown("DashLeft_Ps4" + playerNumber.ToString())) inputDashDirection = -1;
            else if (Input.GetButtonDown("DashRight_Ps4" + playerNumber.ToString())) inputDashDirection = 1;
        }

        if (!inputAttack)
        {
            inputAttack = Input.GetAxis("BasicAttack_Ps4" + playerNumber.ToString()) < 0;
        }

        if (!inputSkill1)
        {
            inputSkill1 = Input.GetButtonDown("Skill1_Ps4" + playerNumber.ToString());
        }

        if (!inputSkill2)
        {
            inputSkill2 = Input.GetButtonDown("Skill2_Ps4" + playerNumber.ToString());
        }

        if (!inputSkill3)
        {
            inputSkill3 = Input.GetButtonDown("Skill3_Ps4" + playerNumber.ToString());
        }

        if (!inputSkill4)
        {
            inputSkill4 = Input.GetButtonDown("Skill4_Ps4" + playerNumber.ToString());
        }

        if (!inputJump)
        {
            if (lastVerticalValue >= 0)
                inputJump = Input.GetAxis("RightStickVertical_Ps4" + playerNumber.ToString()) < 0;
        }
        if (!CharStats.stunned && !CharStats.knockedBack)
            inputDown = Input.GetAxis("RightStickVertical_Ps4" + playerNumber.ToString()) > 0;
        else inputDown = false;

        lastVerticalValue = Input.GetAxis("RightStickVertical_Ps4" + playerNumber.ToString()); // Save last horizontal input to prevent player from spamming jumps. He needs to move the stick back in his standart position to be able to jump again
    }

    #endregion
}