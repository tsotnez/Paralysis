
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Script used to handle buttons used to select champions in the local champion selection scene
/// </summary>
public class LocalChampionSelectionButtonChampion : ChampionSelectionButtonChampion{

    public UserControl.PlayerNumbers TargetPlayerNumber; //For which player
    private LocalChampionSelectionManager manager;

    [HideInInspector]
    public bool highlighted = false; //True while button is being hovered over

    protected override void Start()
    {
        base.Start();

        manager = LocalChampionSelectionManager.instance;
        
        if(skills == null)
            skills = manager.popUps[(int)TargetPlayerNumber - 1].transform;

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
        manager.showChampion(TargetPlayerNumber, ChampionPrefab, false);
        highlighted = true;
        showSkillPopUps();
    }

    public override void onClick()
    {
        base.onClick();
        manager.setChampion(TargetPlayerNumber, ChampionPrefab);
        currentlySelected = true;
        text.enabled = true;
    }

    public override void Deselecting()
    {
        currentlySelected = false;

        if (!isSelectedByOtherPlayer()) //Only transition to shadow if no other player is selecting this button
            base.Deselecting();

        if(!currentlySelected)
            manager.setChampion(TargetPlayerNumber, null); //Resetting selected champion when transferring to different button

        text.enabled = false;
        highlighted = false;
        popup.gameObject.SetActive(false);
    }

    protected override void showSkillPopUps()
    {
        if(manager.showSkillPopUpsForPlayer[TargetPlayerNumber])
            base.showSkillPopUps();
    }

    //toggles skill pop ups status. Called by scene manager
    public void toggleSkillPopUp()
    {
        if (popup.gameObject.activeSelf)
            popup.gameObject.SetActive(false);
        else
        {
            showSkillPopUps();
        }
    }
}
