
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Script used to handle buttons used to select champions in the local champion selection scene
/// </summary>
public class LocalChampionSelectionButtonChampion : ChampionSelectionButtonChampion{

    public UserControl.PlayerNumbers TargetPlayerNumber; //For which player
    private LocalChampionSelectionManager manager;

    protected override void Start()
    {
        base.Start();

        manager = FindObjectOfType<LocalChampionSelectionManager>();
        EventSystemGroup group = GetComponent<EventSystemGroup>();

        if (group != null)
        {
            switch (group.EventSystemID) //Set player number depending on eventsystemID
            {
                case 1:
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player1;
                    skills = manager.popUps[0].transform;
                    break;
                case 2:
                    skills = manager.popUps[1].transform;
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player2;
                    break;
                case 3:
                    skills = manager.popUps[2].transform;
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player3;
                    break;
                case 4:
                    skills = manager.popUps[3].transform;
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player4;
                    break;
            }
        }
        popup = skills;
        ChampionPrefab = manager.ChampionPrefabs[Champion];
    }

    /// <summary>
    /// Checks if the current Champion/Trinket is also hovered over by another player and should therefore not transition to the shadow image.
    /// </summary>
    /// <returns></returns>
    private bool isSelectedByOtherPlayer()
    {
        foreach (LocalChampionSelectionButtonChampion button in transform.parent.GetComponentsInChildren<LocalChampionSelectionButtonChampion>())
        {
            if (button != this && (button.img.enabled || button.currentlySelected))
                return true;
        }
        return false;
    }

    public override void Selecting()
    {
        base.Selecting();
        showSkillPopUps();
    }

    public override void onClick()
    {
        base.onClick();

        manager.setChampion(TargetPlayerNumber, ChampionPrefab);
        currentlySelected = true;
        text.enabled = true;

        LocalChampionSelectionButtonChampion prevSelected = GameObject.FindObjectsOfType<LocalChampionSelectionButtonChampion>().FirstOrDefault(x =>
        x.TargetPlayerNumber == this.TargetPlayerNumber && x.currentlySelected && x != this);

        if (prevSelected != null)
            prevSelected.loseFocus();
    }

    public override void loseFocus()
    {
        currentlySelected = false;
        text.enabled = false;

        if (!isSelectedByOtherPlayer())
            portrait.switchTo(0);
    }

    public override void Deselecting()
    {
        if(!isSelectedByOtherPlayer()) //Only transition to shadow if no toher player is selecting this button
            base.Deselecting();

        popup.gameObject.SetActive(false);
    }
}
