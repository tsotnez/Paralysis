using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class LocalCharacterSelectionSlotButton : MonoBehaviour {

    //True while this button is selected (only one at a time for a champion and 2 for a trinket)
    public bool currentlySelected = false;
    [HideInInspector]
    public int selectedPos = 0; //Order in which this button was selected (The second selected will be removed when selecting a different trinket)

    private Image img;
    private Image portrait;
    private Text text;
    private GameObject skinPreview;
    private ChampionSelectionManager manager;
    private GameObject OnClickAnimation; //GO to be instantiated when player choses a champion (--> epicness increased)

    private UserControl.PlayerNumbers TargetPlayerNumber; //For which player

    [SerializeField]
    private PlayerTargetValue targetValue = PlayerTargetValue.Trinket; //Will this Button define trinket, champion or skin

    public GameObject Champion;
    public Trinket.Trinkets trinket;

    private enum PlayerTargetValue
    {
        Trinket, Champion, Skin
    }

    void Start()
    {
        OnClickAnimation = Resources.Load<GameObject>("Prefabs/UI/LocalChampionSelection/ClickAnimation");

        if(targetValue == PlayerTargetValue.Champion && !PhotonNetwork.offlineMode)
            skinPreview = GameObject.Find("SkinPreviews").transform.Find(Champion.GetComponent<ChampionClassController>().className.ToString()).gameObject;
        img = GetComponent<Image>();
        portrait = transform.parent.Find("portrait").GetComponent<Image>();
        text = GetComponentInChildren<Text>();

        manager = FindObjectOfType<ChampionSelectionManager>();

        EventSystemGroup group = GetComponent<EventSystemGroup>();

        if (group != null)
        {
            switch (group.EventSystemID) //Set player number depending on eventsystemID
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
    }

    /// <summary>
    /// Checks if the current Champion/Trinket is also hovered over by another player and should therefore not transition to the shadow image.
    /// </summary>
    /// <returns></returns>
    private bool isSelectedByOtherPlayer()
    {
        foreach (LocalCharacterSelectionSlotButton button in transform.parent.GetComponentsInChildren<LocalCharacterSelectionSlotButton>())
        {
            if (button != this && (button.img.enabled || button.currentlySelected))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Lose currently selected status
    /// </summary>
    public void loseFocus()
    {
        currentlySelected = false;
        text.enabled = false;
        selectedPos = 0;
        if(!isSelectedByOtherPlayer() && targetValue == PlayerTargetValue.Champion)
            transform.parent.Find("portrait").GetComponent<LocalChampionSelectionPortrait>().switchTo(0);

        if (!PhotonNetwork.offlineMode)
            skinPreview.SetActive(false); //Hide skin preview
    }

    /// <summary>
    /// Show frame image and transition to colored image
    /// </summary>
    public void showFrame()
    {
        img.enabled = true;

        switch (targetValue)
        {
            case PlayerTargetValue.Trinket:
                showTrinketPopUp();
                break;
            case PlayerTargetValue.Champion:
                if (PhotonNetwork.offlineMode)
                    showSkillPopUps();
                LocalChampionSelectionPortrait portrait = transform.parent.Find("portrait").gameObject.GetComponent<LocalChampionSelectionPortrait>();
                portrait.switchTo(1);
                break;
            case PlayerTargetValue.Skin:
                break;
        }
    }

    /// <summary>
    /// Hideframe image and transition to shadow image
    /// </summary>
    public void hideFrame()
    {
        img.enabled = false;

        switch (targetValue)
        {
            case PlayerTargetValue.Trinket:
                transform.parent.parent.parent.Find("PopUpDesc").gameObject.SetActive(false);
                break;
            case PlayerTargetValue.Champion:
                if (PhotonNetwork.offlineMode)
                    transform.parent.parent.parent.Find("PopUps").gameObject.SetActive(false);
                if (!isSelectedByOtherPlayer() && !currentlySelected) //Only transition to shadow image if no other player higlights this button
                    transform.parent.Find("portrait").GetComponent<LocalChampionSelectionPortrait>().switchTo(0);
                break;
            case PlayerTargetValue.Skin:
                break;
        }
    }

    private void showTrinketPopUp()
    {
        Transform popup = transform.parent.parent.parent.Find("PopUpDesc");

        popup.Find("TrinketName").gameObject.GetComponent<Text>().text = ChampionAndTrinketDatabase.TrinketNames[trinket];
        popup.Find("Desc").gameObject.GetComponent<Text>().text = ChampionAndTrinketDatabase.TrinketDescriptions[trinket];

        popup.gameObject.SetActive(true);
    }

    private void showSkillPopUps()
    {
        Transform popup = transform.parent.parent.parent.Find("PopUps");
        Transform lore = popup.transform.Find("PopUpLore");
        Transform skills = popup.transform.Find("PopUpSkills");

        //Set Values for popups. Reading from championDatabase
        var dictionary = ChampionAndTrinketDatabase.database[Champion.GetComponent<ChampionClassController>().className];

        lore.Find("ChampionName").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Name].ToUpper();
        lore.Find("Lore").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Lore];

        //Set Skill Images and text
        setSkillInfos(skills, dictionary);

        popup.gameObject.SetActive(true);
    }

    private void setSkillInfos(Transform skills, Dictionary<ChampionAndTrinketDatabase.Keys, string> dictionary)
    {
        int counter = 1;
        foreach (Transform preview in skills)
        {
            switch (counter)
            {
                case 1:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + Champion.GetComponent<ChampionClassController>().className + "/Skill1");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill1];
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill1Text];
                    break;
                case 2:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + Champion.GetComponent<ChampionClassController>().className + "/Skill2");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill2];
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill2Text];
                    break;
                case 3:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + Champion.GetComponent<ChampionClassController>().className + "/Skill3");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill3];
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill3Text];
                    break;
                case 4:
                    preview.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AbilityImages/" + Champion.GetComponent<ChampionClassController>().className + "/Skill4");
                    preview.Find("SkillName").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill4];
                    preview.Find("Desc").gameObject.GetComponent<Text>().text = dictionary[ChampionAndTrinketDatabase.Keys.Skill4Text];
                    break;
            }
            counter++;
        }
    }

    /// <summary>
    /// Set the champion/trinket behind this button as selected champion/trinket and show preview by calling methods from manager class
    /// </summary>
    public void onClick()
    {
        if (currentlySelected == false)
        {
            //Lose focus on previously selected
            if (targetValue == PlayerTargetValue.Champion) //Theres only one currently selected if the button is supposed to set champion value
            {
                StartCoroutine(handleOnclickAnimation()); //Show on click aniamtion for champion

                LocalCharacterSelectionSlotButton prevSelected = GameObject.FindObjectsOfType<LocalCharacterSelectionSlotButton>().FirstOrDefault(x =>
                x.TargetPlayerNumber == this.TargetPlayerNumber && x.targetValue == this.targetValue && x.currentlySelected);

                if (prevSelected != null)
                    prevSelected.loseFocus();

                if (!PhotonNetwork.offlineMode)
                {
                    skinPreview.SetActive(true); //Show skin selection

                    //Find skinpreview with basic tag and preselect it
                    foreach (Transform child in skinPreview.transform.Find("Viewport").Find("Content"))
                    {
                        if(child.gameObject.tag == "Basic")
                        {
                            child.gameObject.GetComponent<ChampionSkinPreview>().onSelect();
                            child.gameObject.GetComponent<ChangeImageColor>().onClick();
                            break;
                        }
                    }
                }
                else
                    manager.setChampion(TargetPlayerNumber, Champion);
            }
            else if (targetValue == PlayerTargetValue.Trinket) //There are two currently selected if the button sets trinket value
            {
                //Get all currently selected trinket Buttons
                LocalCharacterSelectionSlotButton[] prevSelected = GameObject.FindObjectsOfType<LocalCharacterSelectionSlotButton>().Where(x =>
                x.TargetPlayerNumber == this.TargetPlayerNumber && x.targetValue == this.targetValue && x.currentlySelected).ToArray();

                //If no other trinket is selected, set trinket1
                if(prevSelected.Length == 0)
                    manager.setTrinket1(TargetPlayerNumber, trinket);
                //If there is another trinket selected already, set trinket2
                else if (prevSelected.Length == 1)
                {
                    manager.setTrinket2(TargetPlayerNumber, trinket);
                    prevSelected[0].selectedPos = 2;
                }
                //when there are already 2 trinkets selected, let the one selected first lose focus and overwrite the trinket that button set the value for
                else if (prevSelected.Length == 2) 
                {
                    LocalCharacterSelectionSlotButton selectedFirst = prevSelected.First(x => x.selectedPos == 2);
                    manager.setTrinket(TargetPlayerNumber, trinket, selectedFirst.trinket);
                    selectedFirst.loseFocus();

                    prevSelected.First(x => x.selectedPos == 1).selectedPos = 2;
                }

                selectedPos = 1;
            }

            currentlySelected = true;
            text.enabled = true;
        }
    }

    /// <summary>
    /// Instantiates Object which plays the on click animation and destroys it after animation has finished
    /// </summary>
    /// <returns></returns>
    private IEnumerator handleOnclickAnimation()
    {
        GameObject anim = Instantiate(OnClickAnimation, transform.parent, false);
        yield return new WaitForSeconds(.333f);
        Destroy(anim);
    }

}
