using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Parent class for all manager classes used for UI only scenes (e.g. Main menu)
/// </summary>
public abstract class UIManager : MonoBehaviour {

    protected MyStandaloneInputModule module;

    /// <summary>
    /// Shows a messagebox to the player, fading in from the bottom
    /// </summary>
    public static IEnumerator showMessageBox(Canvas parent, string Message)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/MessageBox");
        prefab.transform.Find("Message").GetComponent<Text>().text = Message;

        GameObject MB = Instantiate(prefab, parent.transform, false);
        yield return new WaitForSeconds(3);
        Destroy(MB);
    }

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
        if (module == null)
            return;

        //Switch input method depending on whether a controller is connected
        if (Array.Exists(Input.GetJoystickNames(), x => x == "Controller (XBOX 360 For Windows)"))
        {
            if (MyStandaloneInputModule.ControllingPlayerInputDevice == UserControl.InputDevice.KeyboardMouse)
            {
                gotoController();
                StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Switched to Controller input."));
            }
        }
        else if (MyStandaloneInputModule.ControllingPlayerInputDevice == UserControl.InputDevice.XboxController)
        {
            //Switch back to keyboard and mouse input       
            gotoKeyboard();
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Switched to Keyboard and Mouse input."));
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
