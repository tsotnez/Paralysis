using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Parent class for all manager classes used for UI only scenes (e.g. Main menu)
/// </summary>
public abstract class UIManager : MonoBehaviour {

    protected MyStandaloneInputModule module;

	// Use this for initialization
	protected virtual void Start () {

        //Get input Module
        module = FindObjectOfType<MyStandaloneInputModule>();

        //Switch to controller Controls if a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == "Controller (XBOX 360 For Windows)"))
        {
            module.SetControllingPlayerInputDevice(UserControl.InputDevice.XboxController);
        }
        else
        {
            module.SetControllingPlayerInputDevice(UserControl.InputDevice.KeyboardMouse);
        }
    }

    protected virtual void Update ()
    {
        //Switch input method depending on whether a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == "Controller (XBOX 360 For Windows)"))
        {
            if (module.ControllingPlayerInputDevice == UserControl.InputDevice.KeyboardMouse)
            {
                gotoController();
            }
        }
        else if (module.ControllingPlayerInputDevice == UserControl.InputDevice.XboxController)
        {
            //Switch back to keyboard and mouse input       
            gotoKeyboard();
        }
    }

    protected virtual void gotoKeyboard()
    {
        module.SetControllingPlayerInputDevice(UserControl.InputDevice.KeyboardMouse);
    }

    protected virtual void gotoController()
    {
        module.SetControllingPlayerInputDevice(UserControl.InputDevice.XboxController);
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }
}
