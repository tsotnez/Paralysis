using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manager for settings. Derives from MainMenu Manager to handle page switching. Also handles saving the settings
/// into player prefs and applying settings in game.
/// </summary>
public class SettingsManager : MainMenuManager {

    [Header("Graphic Settings")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public bool fullscreen = true;

    [Header("Sound Options")]
    public AudioMixer mixer;
    public Slider effectsSlider;
    public Slider voicesSlider;
    public Slider ambientSlider;
    public Slider musicSlider;


    // Use this for initialization
    protected override void Start ()
    {
        Init();

        initGraphicsSettings();
        initAudioSettings();

    }

    protected override void Update() {}

    #region initialization
    private void initGraphicsSettings()
    {
        //Adding options to resolution dropdown
        Resolution[] res = Screen.resolutions;
        List<string> options = new List<string>();
        foreach (Resolution item in res)
        {
            options.Add(item.width + " x " + item.height);
        }
        resolutionDropdown.AddOptions(options);

        //Add listeners to graphics settings
        resolutionDropdown.onValueChanged.AddListener(delegate { setResolution(resolutionDropdown.value); });
        fullscreenToggle.onValueChanged.AddListener(delegate { setFullscreen(fullscreenToggle.isOn); });

        //Read and set resolution
        string resName = PlayerPrefs.GetInt("ScreenWidth", 800) + " x " + PlayerPrefs.GetInt("ScreenHeight", 600);
        int indexOfResName = options.IndexOf(resName);


        if (indexOfResName > -1)
            resolutionDropdown.value = indexOfResName; //Saved resolution still available
        else
        {
            //Try again with 800 x 600 resolution, if not available
            resName = "800 x 600";
            indexOfResName = options.IndexOf(resName);

            if (indexOfResName == -1) // 800 x 600 not available as well, so use first available
                resolutionDropdown.value = 0;
        }

        setResolution(resolutionDropdown.value);

        //Read and set fullscreen
        fullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen", 1));
        fullscreenToggle.Set(fullscreen, true);
    }

    private void initAudioSettings()
    {
        //Adding event listeners to audio volume sliders
        effectsSlider.onValueChanged.AddListener(delegate { setVolume(effectsSlider); });
        voicesSlider.onValueChanged.AddListener(delegate { setVolume(voicesSlider); });
        ambientSlider.onValueChanged.AddListener(delegate { setVolume(ambientSlider); });
        musicSlider.onValueChanged.AddListener(delegate { setVolume(musicSlider); });

        //Read volume setting from player prefs and set sliders accordingly
        ambientSlider.value = PlayerPrefs.GetFloat("AmbientVolume", -20);
        voicesSlider.value = PlayerPrefs.GetFloat("VoicesVolume", -20);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", -20);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", -20);

        setVolume(effectsSlider);
        setVolume(voicesSlider);
        setVolume(ambientSlider);
        setVolume(musicSlider);
    }
    #endregion

    #region Listeners
    /// <summary>
    /// Listeners for onValueChanged by Volume Sliders in Sound settings
    /// </summary>
    public void setVolume(Slider target)
    {
        //Set Value texts text
        float value = Mathf.Round(target.value);
        target.transform.Find("Value").gameObject.GetComponent<Text>().text = (value + 80).ToString(); //Convert value so it is between 0 and 100

        string targetVariable = "";

        if (target == effectsSlider)
            targetVariable = "EffectsVolume";
        else if (target == voicesSlider)
            targetVariable = "VoicesVolume";
        else if (target == ambientSlider)
            targetVariable = "AmbientVolume";
        else if (target == musicSlider)
            targetVariable = "MusicVolume";

        //Save set value to player prevs
        PlayerPrefs.SetFloat(targetVariable, value);
        //Set actual volume
        mixer.SetFloat(targetVariable, value);
    }

    /// <summary>
    /// Listener for onValueChanged by Resolution dropdown in graphic settings
    /// </summary>
    public void setResolution(int index)
    {
        string value = resolutionDropdown.options[index].text;

        string[] splitted = value.Split('x');

        int width = Convert.ToInt32(splitted[0].Trim());
        int height = Convert.ToInt32(splitted[1].Trim());
        Screen.SetResolution(width, height, fullscreen);

        //Save to player prefs
        PlayerPrefs.SetInt("ScreenWidth", width);
        PlayerPrefs.SetInt("ScreenHeight", height);
    }

    /// <summary>
    /// Listener for onValueChanged by Fullscreen toggle in graphic settings
    /// </summary>
    public void setFullscreen(bool value)
    {
        fullscreen = value;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen);

        //Save to player prefs, 0 = false, 1 = true
        PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(fullscreen));
    }
    #endregion
}
