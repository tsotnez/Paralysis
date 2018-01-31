using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkChampionSelectionManager : ChampionSelectionManager {

    public static Player localPlayer = 
        new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.KeyboardMouse, 1); //Object that stores all date about local player, set externally
    public static int playerCount = 6; //How many players are in the room? needed to show/hide additional Platforms. Set externally

    public GameObject additionalPlatforms3v3;

    public Transform playerPlatform; //Position to instantiate the preview champion object to. Only public for testing

    protected override void Start()
    {
        base.Start();

        //Show additional platforms if needed
        if (playerCount > 4)
        {
            additionalPlatforms3v3.SetActive(true);
            additionalPlatforms2v2.SetActive(true);
        }
        else if (playerCount > 2)
        {
            additionalPlatforms2v2.SetActive(true);
        }
    }

    protected override void Update()
    {
        base.Update();

        //Shoe ready button when champion and trinkets are selected
        bool everythingSelected = true;

        if (localPlayer.ChampionPrefab == null || localPlayer.trinket1 == localPlayer.trinket2)
            everythingSelected = false;

        readyButton.SetActive(everythingSelected);
    }

    public override void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion)
    {
        localPlayer.ChampionPrefab = Champion;
        DestroyExistingPreview(playerPlatform);
        ShowPrefab(Champion, playerPlatform, false);
    }

    public override void setTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName, Trinket.Trinkets toOverwrite)
    {
        if (localPlayer.trinket1 == toOverwrite)
            localPlayer.trinket1 = trinketName;
        else
            localPlayer.trinket2 = trinketName;
    }

    public override void setTrinket1(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    {
        localPlayer.trinket1 = trinketName;
    }

    public override void setTrinket2(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    {
        localPlayer.trinket2 = trinketName;
    }


    /// <summary>
    /// Called when all players are ready
    /// </summary>
    public override void startGame()
    {
        //Pass Player info to next scenes here
    }

    public void toggleReady()
    {
        //Show/Hide ready sign for local player here. Called by ready button.
    }
}
