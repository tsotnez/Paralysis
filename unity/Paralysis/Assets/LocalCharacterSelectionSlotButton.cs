using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class LocalCharacterSelectionSlotButton : MonoBehaviour {

    //True while this button is selected (only one at a time for a champion and 2 for a trinket)
    public bool currentlySelected = false;

    private Image img;
    private Image portrait;
    private Text text;

    private UserControl.PlayerNumbers TargetPlayerNumber; //For which player

    [SerializeField]
    private PlayerTargetValue targetValue; //Will this Button define trinket, champion or skin

    public GameObject Champion;
    public Trinket.Trinkets trinket;

    private enum PlayerTargetValue
    {
        Trinket, Champion, Skin
    }

    private void Start()
    {
        img = GetComponent<Image>();
        portrait = transform.parent.Find("portrait").GetComponent<Image>();
        text = GetComponentInChildren<Text>();

        switch(GetComponent<EventSystemGroup>().EventSystemID) //Set player number depending on eventsystemID
        {
            case 1:
                TargetPlayerNumber = UserControl.PlayerNumbers.Player1;
                break;
            case 2:
                TargetPlayerNumber = UserControl.PlayerNumbers.Player2;
                break;
            case 3:
                TargetPlayerNumber = UserControl.PlayerNumbers.Player3;
                break;
            case 4:
                TargetPlayerNumber = UserControl.PlayerNumbers.Player4;
                break;
        }
    }

    /// <summary>
    /// Lose currently selected status
    /// </summary>
    public void loseFocus()
    {
        currentlySelected = false;
        text.enabled = false;
    }

    /// <summary>
    /// Show frame image and transition to colored image
    /// </summary>
    public void showFrame()
    {
        img.enabled = true;
        transform.parent.Find("portrait").GetComponent<LocalChampionSelectionPortrait>().switchTo(1);
    }

    /// <summary>
    /// Hideframe image and transition to shadow image
    /// </summary>
    public void hideFrame()
    {
        img.enabled = false;
        transform.parent.Find("portrait").GetComponent<LocalChampionSelectionPortrait>().switchTo(0);
    }

    /// <summary>
    /// Set the champion behind this button as selected champion and show preview by calling methods from manager class
    /// </summary>
    public void onClick()
    {
        LocalCharacterSelectionSlotButton prevSelected = GameObject.FindObjectsOfType<LocalCharacterSelectionSlotButton>().FirstOrDefault(x =>
        x.TargetPlayerNumber == this.TargetPlayerNumber && x.targetValue == this.targetValue && x.currentlySelected);

        if (prevSelected != null)
            prevSelected.loseFocus();

        currentlySelected = true;

        text.enabled = true;
        switch (targetValue)
        {
            case PlayerTargetValue.Trinket:
                GameObject.FindObjectOfType<LocalChampionSelectionManager>().setTrinket(TargetPlayerNumber, trinket);
                break;
            case PlayerTargetValue.Champion:
                GameObject.FindObjectOfType<LocalChampionSelectionManager>().setChampion(TargetPlayerNumber, Champion);
                break;
            case PlayerTargetValue.Skin:
                break;
        }
    }

}
