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
    
    public GameObject additionalPlatforms3v3;

    public Transform[] teamOnePlatforms; //Position to instantiate the preview champion object to. Only public for testing
    public Transform[] teamTwoPlatforms;
    public Transform myPlatform;

    bool everythingSelected = false;

    public string nextScene = "Network test";
    public Button backButton;
    public Button[] championButtons;

    protected override void Start()
    {
        base.Start();
        backButton.onClick.AddListener(OnBackButtonClicked);

        //ARCHER = 0, KNIGHT = 1, INFANTRY = 2, ALCHEMIST = 3, ASSASSIN = 4
        foreach(Button button in championButtons)
        {
            button.onClick.AddListener(delegate{
                champButtonPressed(button);
            });
        }

        GameNetwork.Instance.OnGameStateUpdate += OnGameStateUpdated;
        GameNetworkChampSelect.Instance.OnPlayerSelectedChamp += OnOtherPlayerSelectedChamp;

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
        if(player == PhotonNetwork.player)
        {
            myPlatform = playerTransform;
        }
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

    private void OnOtherPlayerSelectedChamp(int playerNetNum, int champ)
    {
        print("other player" + playerNetNum + " cham:" + champ);
    }

    public override void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion)
    {
        localPlayer.ChampionPrefab = Champion;
        DestroyExistingPreview(myPlatform);
        ShowPrefab(Champion, myPlatform, false);
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
        if(GameNetwork.Instance.IsMasterClient)
        {
            GameNetwork.Instance.StartGame();
        }
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

    public void champButtonPressed(Button button)
    {
        //ARCHER = 0, KNIGHT = 1, INFANTRY = 2, ALCHEMIST = 3, ASSASSIN = 4
        for(int i = 0; i < championButtons.Length; i++)
        {
            Button champButton = championButtons[i];
            if(champButton == button)
            {
                GameNetworkChampSelect.Instance.selectedChamipon(i);
            }
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
        GameNetworkChampSelect.Instance.OnPlayerSelectedChamp -= OnOtherPlayerSelectedChamp;
    }
}
