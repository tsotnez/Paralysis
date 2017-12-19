using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkVersusManager : Photon.MonoBehaviour
{

    public Player player1;
    public string pathToChampionPrefabs;
    public Text connectionStatusText;
    public GameObject hotbarPrefab;

    public Transform spawnPlayer1;

    private GameObject myPlayer;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("Paralysis alpha");
    }

    private void InstantiatePlayers()
    {
        //if (player1 == null)
        //    player1 = rPlayer1;

        //Player1
        GameObject instPlayer1 = PhotonNetwork.Instantiate(player1.ChampionPrefab.name, spawnPlayer1.position, Quaternion.identity, 0);
        instPlayer1.GetComponent<UserControl>().inputDevice = player1.inputDevice;
        instPlayer1.layer = 11;

        LayerMask whatToHitP1 = new LayerMask();
        whatToHitP1 |= (1 << 12); //Add Team2 as target layer

        instPlayer1.GetComponent<ChampionClassController>().m_whatToHit = whatToHitP1;
        instPlayer1.GetComponent<UserControl>().playerNumber = player1.playerNumber;

        //Trinkets P1
        instPlayer1.AddComponent(Trinket.trinketsForNames[player1.trinket1]);
        instPlayer1.AddComponent(Trinket.trinketsForNames[player1.trinket2]);
        instPlayer1.GetComponent<ChampionClassController>().Trinket1 = instPlayer1.GetComponents<Trinket>()[0];
        instPlayer1.GetComponent<ChampionClassController>().Trinket2 = instPlayer1.GetComponents<Trinket>()[1];
        instPlayer1.GetComponent<ChampionClassController>().Trinket1.trinketNumber = 1;
        instPlayer1.GetComponent<ChampionClassController>().Trinket2.trinketNumber = 2;

        Camera.main.GetComponent<CameraBehaviour>().changeTarget(instPlayer1.transform);
        myPlayer = instPlayer1;
    }

    private void BuildUi()
    {
        Transform parent = GameObject.Find("Hotbars").transform;

        GameObject hotbar = Instantiate(hotbarPrefab, parent, false);
        hotbar.GetComponent<HotbarController>().setChampionName(player1.ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initAbilityImages(player1.ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initTrinketImages(myPlayer.GetComponents<Trinket>()[0].DisplayName, myPlayer.GetComponents<Trinket>()[1].DisplayName);

        //Assign hotbar to player
        myPlayer.GetComponent<CharacterStats>().hotbar = hotbar.GetComponent<HotbarController>();
        myPlayer.GetComponent<ChampionClassController>().hotbar = hotbar.GetComponent<HotbarController>();
    }

    private void Update()
    {
        connectionStatusText.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinOrCreateRoom("Room", null, null);
    }

    public virtual void OnConnectedToMaster()
    {
        Debug.Log("connected");
        PhotonNetwork.JoinLobby();
    }

    public virtual void OnJoinedRoom()
    {
        InstantiatePlayers();
        BuildUi();
    }

}
