using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkChampionSelectionManager : ChampionSelectionManager {

    public static Player localPlayer = 
        new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.KeyboardMouse, 1); //Object that stores all date about local player, set externally
    private int playerCount; //How many players are in the room? needed to show/hide additional Platforms. Set externally
    private int aiCount;

    public GameObject additionalPlatforms3v3;

    public Transform[] teamOnePlatforms; //Position to instantiate the preview champion object to. Only public for testing
    public Transform[] teamTwoPlatforms;

    bool everythingSelected = false;

    public Button backButton;

    protected override void Start()
    {
        base.Start();
        backButton.onClick.AddListener(OnBackButtonClicked);

        GameNetwork.Instance.OnGameStateUpdate += OnGameStateUpdated;


        playerCount = GameNetwork.Instance.getPlayerList().Length;
        if(playerCount < 6)
        {
            additionalPlatforms3v3.SetActive(false);
        }
        if(playerCount < 4)
        {
            additionalPlatforms2v2.SetActive(false);
        }

        OnGameStateUpdated();
    }

    private void AddPlayerToList(PhotonPlayer player, Transform playerTransform)
    {
        print("adding player to transform list...: " + player.NickName);
        Text playerText = playerTransform.GetComponentInChildren<Text>();
        playerText.text = player.NickName;
    }

    private void SetTransformToEmpty(Transform playerTransform)
    {
        playerTransform.gameObject.SetActive(false);
    }

    private void OnGameStateUpdated()
    {
        List<int> teamOne = GameNetwork.Instance.TeamOneList;
        List<int> teamTwo = GameNetwork.Instance.TeamTwoList;

        PhotonPlayer[] players = GameNetwork.Instance.getPlayerList();
        Dictionary<int, int> playerDict = GameNetwork.Instance.PlayerDict;

        for(int i = 0; i < 3; i++)
        {
            if(i < teamOne.Count)
            {
                bool found = false;
                int playerId = teamOne[i];
                foreach(PhotonPlayer player in players)
                {
                    if(playerId == player.ID)
                    {
                        AddPlayerToList(player, teamOnePlatforms[i]);
                        found = true;
                    }
                }
                if(!found)SetTransformToEmpty(teamOnePlatforms[i]);
            }
            else
            {
                SetTransformToEmpty(teamOnePlatforms[i]);
            }

            if(i < teamTwo.Count)
            {
                bool found = false;
                int playerId = teamTwo[i];
                foreach(PhotonPlayer player in players)
                {
                    if(playerId == player.ID)
                    {
                        AddPlayerToList(player, teamTwoPlatforms[i]);
                        found = true;
                    }
                }
                if(!found)SetTransformToEmpty(teamTwoPlatforms[i]);
            }
            else
            {
                SetTransformToEmpty(teamTwoPlatforms[i]);
            }
        }       
    }

    protected override void Update()
    {
        base.Update();

        //Show ready button when champion and trinkets are selected
        everythingSelected = true;

        if (localPlayer.ChampionPrefab == null || localPlayer.trinket1 == localPlayer.trinket2)
            everythingSelected = false;
    }

    public override void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion)
    {
        localPlayer.ChampionPrefab = Champion;
        //TODO
        //DestroyExistingPreview(player1Platform);
        //ShowPrefab(Champion, player1Platform, false);
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
        if(everythingSelected)
        {
            //Show/Hide ready sign for local player here. Called by ready button.
        }
        else
        {
            StartCoroutine(UIManager.showMessageBox(GameObject.FindObjectOfType<Canvas>(), "Select one champion and two different trinkets."));
        }
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(waitForGameNetworkDestroyed());
    }

    private IEnumerator waitForGameNetworkDestroyed ()
    {
        Destroy(GameNetwork.Instance.gameObject);
        yield return new WaitWhile( ()=> GameNetwork.Instance == null);
        SceneManager.LoadScene(GameConstants.MAIN_MENU_SCENE);
    }

    private void OnDestroy()
    {
        GameNetwork.Instance.OnGameStateUpdate -= OnGameStateUpdated;
    }
}
