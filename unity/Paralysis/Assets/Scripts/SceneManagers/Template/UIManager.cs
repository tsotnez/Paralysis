using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Parent class for all manager classes used for UI only scenes (e.g. Main menu)
/// </summary>
public abstract class UIManager : MonoBehaviour {

	// Use this for initialization
	protected virtual void Start () {
        //Switch to controller Controls if a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == "Controller (XBOX 360 For Windows)"))
        {
            MyStandaloneInputModule module = FindObjectOfType<MyStandaloneInputModule>();
            module.verticalAxis = "Vertical_XboxPlayer1";
            module.horizontalAxis = "Horizontal_XboxPlayer1";
            module.submitButton = "Skill4_XboxPlayer1";
        }
    }
}
