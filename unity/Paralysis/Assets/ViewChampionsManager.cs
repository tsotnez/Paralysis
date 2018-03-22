using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script used in ViewChampions scene fot the buttons setting champions
/// </summary>
public class ViewChampionsManager : ChampionSelectionManager {

    public GameObject popUpLore;
    public GameObject popUpSkills;
    public GameObject popUps;

    public Transform platform;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();
        PhotonNetwork.offlineMode = true;
    }
    
    public void showChampion(ChampionDatabase.Champions champ)
    {
        DestroyExistingPreview(platform);
        ShowPrefab(ChampionPrefabs[champ], platform, false);
    }
}
