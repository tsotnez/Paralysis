using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamAllocationPlayer : MonoBehaviour {

    public UserControl.PlayerNumbers playerNumber;
    public UserControl.InputDevice inputDevice = UserControl.InputDevice.XboxController;

    public int teamNumber = 0;

    private float lastValue = 0;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        switch (inputDevice)
        {
            case UserControl.InputDevice.XboxController:
                checkXboxInput();
                break;
            case UserControl.InputDevice.KeyboardMouse:
                checkKeyboardInput();
                break;
        }
    }

    void checkXboxInput()
    {
        if (lastValue == 0)
        {
            if (Input.GetAxis("Trinket1_Xbox" + playerNumber.ToString()) > 0) // D pad left
                switchToTeam(1);
            else if (Input.GetAxis("Trinket2_Xbox" + playerNumber.ToString()) < 0) // d pad right
                switchToTeam(-1);
        }
        //Both are same axis
        lastValue = Input.GetAxis("Trinket2_Xbox" + playerNumber.ToString());
    }

    void checkKeyboardInput()
    {
        if (lastValue == 0)
        {
            if (Input.GetAxis("Horizontal") > 0) // d
                switchToTeam(1);
            else if (Input.GetAxis("Horizontal") < 0) // a
                switchToTeam(-1);
        }
        lastValue = Input.GetAxis("Horizontal");
    }

    //Switches player to given team
    void switchToTeam(int team)
    {
        teamNumber += team;
        if (teamNumber < -1)
            teamNumber = -1;
        else if (teamNumber > 1)
            teamNumber = 1;

        if (teamNumber == -1)
            transform.SetParent(GameObject.Find("Team1").transform, false);
        else if(teamNumber == 0)
            transform.SetParent(GameObject.Find("Neutral").transform, false);
        else if(teamNumber == 1)
            transform.SetParent(GameObject.Find("Team2").transform, false);
    }
}
