using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NetworkVersusManager : GameplayManager
{
    public Text KillCount; //Text which displays how many kills where scored by the player
    public Text DeathCount; //Text which displays how times the player has died

    public Text connectionStatusText;
    public GameObject hotbarPrefab;
    private GameObject myPlayerInstance;
    private GameObject[] spawnPoints;

    private Player localPlayer;
    private int playerNetworkNum;

    public const string CHAMP_LOCATION = "Prefabs/Champions/";

    #region default
    protected override void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag(GameConstants.SPAWN_POINT_TAG);
        playerNetworkNum = GameNetwork.Instance.PlayerNetworkNumber;
        localPlayer = NetworkChampionSelectionManager.localPlayer;
    }

    protected override void instantiatePlayers()
    {

    }

    void Start()
    {
        if(localPlayer.inputDevice == UserControl.InputDevice.XboxController)
        {
            StandaloneInputModule mod = GameObject.FindObjectOfType<StandaloneInputModule>();
            mod.horizontalAxis = "Horizontal_XboxPlayer1";
            mod.verticalAxis = "Vertical_XboxPlayer1";
            mod.submitButton = "Skill4_XboxPlayer1";
        }
    }

    #endregion

    public void spawnPlayer()
    {
        // Get players corresponding spawn point
        SpawnPoint spawnPoint = null;
        foreach(GameObject spawnObj in spawnPoints)
        {
            spawnPoint = spawnObj.GetComponent<SpawnPoint>();
            if(spawnPoint.playerNumber == playerNetworkNum && spawnPoint.teamNumber == GameNetwork.Instance.TeamNum)
            {
                break;
            }
        }

        //Player
        GameObject instPlayer1 = PhotonNetwork.Instantiate(CHAMP_LOCATION + localPlayer.ChampionPrefab.name, spawnPoint.transform.position, Quaternion.identity, 0);
        instPlayer1.GetComponent<UserControl>().inputDevice = localPlayer.inputDevice;
        LayerMask whatToHitP1 = new LayerMask();

        ChampionClassController con = instPlayer1.GetComponent<ChampionClassController>();
        if(spawnPoint.facingDir == SpawnPoint.SpawnFacing.left)
        {
            //con.FacingRight = false;
            con.Flip();
        }
        con.m_whatToHit = whatToHitP1;
        instPlayer1.GetComponent<UserControl>().playerNumber = localPlayer.playerNumber;

        //Trinkets P1
        instPlayer1.AddComponent(Trinket.trinketsForNames[localPlayer.trinket1]);
        instPlayer1.AddComponent(Trinket.trinketsForNames[localPlayer.trinket2]);
        con.Trinket1 = instPlayer1.GetComponents<Trinket>()[0];
        con.Trinket2 = instPlayer1.GetComponents<Trinket>()[1];
        con.Trinket1.trinketNumber = 1;
        con.Trinket2.trinketNumber = 2;

        //Instaniate camera
        CameraBehaviour cam = Instantiate(Resources.Load<GameObject>("Main Camera Network"), new Vector3(0, 0, -0.5f), Quaternion.identity).GetComponent<CameraBehaviour>();

        cam.changeTarget(instPlayer1.transform);

        //Disable default cam
        GameObject.Find("LobbyCam").SetActive(false);
        GameObject.Find("MainCanvas").GetComponent<Canvas>().worldCamera = Camera.main;

        myPlayerInstance = instPlayer1;
        buildUI();
        //Wait before joining team, because scene needs time to synchronize
        StartCoroutine(test());
    }

    private void Update()
    {
        connectionStatusText.text = "Ping: " + PhotonNetwork.GetPing() + " " + PhotonNetwork.connectionStateDetailed.ToString();
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(.2f);
        myPlayerInstance.GetComponent<CharacterNetwork>().joinTeam();
    }

    protected override void buildUI()
    {
        Transform parent = GameObject.Find("Hotbars").transform;

        GameObject hotbar = Instantiate(hotbarPrefab, parent, false);
        hotbar.GetComponent<HotbarController>().setChampionName(localPlayer.ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initAbilityImages(localPlayer.ChampionPrefab.name);
        hotbar.GetComponent<HotbarController>().initTrinketImages(myPlayerInstance.GetComponents<Trinket>()[0].DisplayName, myPlayerInstance.GetComponents<Trinket>()[1].DisplayName);

        //Assign hotbar to player
        myPlayerInstance.GetComponent<CharacterStats>().hotbar = hotbar.GetComponent<HotbarController>();
        myPlayerInstance.GetComponent<ChampionClassController>().hotbar = hotbar.GetComponent<HotbarController>();
        myPlayerInstance.GetComponent<CharacterStats>().enabled = true;
    }

    protected override void GameOver(string winner)
    {
        base.GameOver(winner);
        myPlayerInstance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        photonView.RPC("setGameOverRemote", PhotonTargets.Others, winner);
    }

    /// <summary>
    /// Called by remote instances when game over
    /// </summary>
    /// <param name="winner"></param>
    [PunRPC]
    void setGameOverRemote(string winner)
    {
        base.GameOver(winner);
    }

    /// <summary>
    /// Resets erverything to restart the game
    /// </summary>
    [PunRPC]
    public override void restart()
    {
        base.restart();

        myPlayerInstance.GetComponent<UserControl>().enabled = true;
        myPlayerInstance.transform.position = Vector3.zero;
        myPlayerInstance.GetComponent<CharacterStats>().ResetValues();
        myPlayerInstance.GetComponent<ChampionClassController>().ResetValues();
        myPlayerInstance.transform.Find("graphics").gameObject.GetComponent<ChampionAnimationController>().StartAnimation(AnimationController.AnimatorStates.Idle);
        Camera.main.GetComponent<CameraBehaviour>().gameRunning = true;

        gameOverOverlay.gameObject.SetActive(false);
    }
}
