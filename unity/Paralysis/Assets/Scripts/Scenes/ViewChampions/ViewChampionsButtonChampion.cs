using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script used in the ViewChampions scene for the champion buttons
/// </summary>
public class ViewChampionsButtonChampion : ChampionSelectionButtonChampion {

    private Transform lore;
    private ViewChampionsManager manager;

    protected override void Start()
    {
        base.Start();

        manager = GameObject.FindObjectOfType<ViewChampionsManager>();

        popup = manager.popUps.transform;
        lore = manager.popUpLore.transform;
        skills = manager.popUpSkills.transform;
        ChampionPrefab = manager.ChampionPrefabs[Champion];
    }

    protected override void showSkillPopUps()
    {
        base.showSkillPopUps();

        //Set Values for lore.
        lore.Find("ChampionName").gameObject.GetComponent<Text>().text = ChampionPrefab.GetComponent<ChampionClassController>().characterFullName.ToUpper();
        lore.Find("Lore").gameObject.GetComponent<Text>().text = ChampionPrefab.GetComponent<ChampionClassController>().characterLore.ToUpper();
    }

    public override void Selecting()
    {
        base.Selecting();
    }

    public override void Deselecting()
    {
        base.Deselecting();
    }

    public override void onClick()
    {
        base.onClick();
        showSkillPopUps();
        currentlySelected = true;

        ViewChampionsButtonChampion prevSelected = GameObject.FindObjectsOfType<ViewChampionsButtonChampion>().FirstOrDefault(x =>
        x.currentlySelected && x != this);

        if (prevSelected != null)
            prevSelected.loseFocus();

        manager.showChampion(Champion);
    }
}
