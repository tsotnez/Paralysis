using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsManager : MainMenuManager {

    [Header("Volume Sliders")]
    public Slider effectsSlider;
    public Slider voicesSlider;
    public Slider ambientSlider;
    public Slider musicSlider;


    // Use this for initialization
    protected override void Start () {
        Init();

        //Adding event listeners to audio volume sliders
        effectsSlider.onValueChanged.AddListener(delegate { setVolume(effectsSlider); });
        voicesSlider.onValueChanged.AddListener(delegate { setVolume(voicesSlider); });
        ambientSlider.onValueChanged.AddListener(delegate { setVolume(ambientSlider); });
        musicSlider.onValueChanged.AddListener(delegate { setVolume(musicSlider); });

        setVolume(effectsSlider);
        setVolume(voicesSlider);
        setVolume(ambientSlider);
        setVolume(musicSlider);
    }

	protected override void Update () {
    }
    
    public void setVolume(Slider target)
    {
        //Set Value texts text
        int value = Mathf.RoundToInt(target.value * 100);
        target.transform.Find("Value").gameObject.GetComponent<Text>().text = value.ToString();

        //TODO: set actual volume for audio channel
    }

}
