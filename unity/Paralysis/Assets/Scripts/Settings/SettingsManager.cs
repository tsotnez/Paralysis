using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public Toggle fullscreenToggle;

    public Dropdown resolutionDropdown;

    public Dropdown textureQualityDropdown;

    public Dropdown antialiasingDropdown;

    public Dropdown vSyncDropdown;

    public Slider musicVolumeSlide;




    public Resolution[] resolutions;
    public GameSettings gameSettings;


    void OnEnable()
    {
        gameSettings = new GameSettings();

        fullscreenToggle.onValueChanged.AddListener(delegate {     OnFullscreenToggle(); });


        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });


        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });


        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });


        vSyncDropdown.onValueChanged.AddListener(delegate { OnVsyncChange(); });



        musicVolumeSlide.onValueChanged.AddListener(delegate { OnMusicVolumChange(); });





        resolutions = Screen.resolutions;

    }



    public void OnFullscreenToggle()
    {


        gameSettings.fullscreen  = Screen.fullScreen = fullscreenToggle.isOn;

    }


    public void OnResolutionChange()
    {



    }


    public void OnTextureQualityChange()
    {

        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;



    }

    public void OnAntialiasingChange()
    {

    }

    public void OnVsyncChange()
    {

    }


    public void OnMusicVolumChange()
    {


    }


    public void SaveSettings()
    {
        
    }


    public void LoadSetting()
    {
        
    }
}
