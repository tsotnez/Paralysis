using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class for all gameplay scene managers
/// </summary>
public abstract class GameplayManager : Photon.MonoBehaviour
{
    public static GameplayManager Instance;

    /// <summary>
    /// Current GameMode, default is TeamDeathmatch
    /// </summary>
    public GameModes gameMode = GameModes.TeamDeathmatch;

    // GameOver overlay
    public Transform gameOverOverlay;

    // Team Arrays containing Players
    public static List<Team> Teams;

    // SpawnPoints
    public SpawnPoint[] Spawns;

    // Redundant variables so they can be assigned in the inspector (static ones cant)
    public Player defaultPlayer1 = null;
    public Player defaultPlayer2 = null;

    // Color to apply to player images to fit the environemts lighting
    public Color championSpriteOverlayColor;

    protected GameObject simpleGlobalMessage;
    protected GameObject playerInteractionGlobalMessage;
    protected Transform mainCanvas;
    protected int controllersConnected;

    public enum GameModes
    {
        TeamDeathmatch
    }

    protected virtual void Awake()
    {
        Instance = this;
        Cursor.visible = false;

        PhotonNetwork.offlineMode = true;
        InstantiatePlayers();
        BuildUI();

        simpleGlobalMessage = Resources.Load<GameObject>("Prefabs/UI/GlobalMessages/SimpleGlobalMessage");
        playerInteractionGlobalMessage = Resources.Load<GameObject>("Prefabs/UI/GlobalMessages/PlayerInteractionGlobalMessage");
        mainCanvas = GameObject.Find("MainCanvas").transform;

        controllersConnected = Input.GetJoystickNames().Count(x => x == GameConstants.NAME_OF_XBOX360CONTROLLER_IN_ARRAY);
    }

    protected virtual void Update()
    {
        ManageControllers();
    }
    protected void ManageControllers()
    {
        // Checking for disconnecting/connecting controllers
        int newCount = Input.GetJoystickNames().Count(x => x == GameConstants.NAME_OF_XBOX360CONTROLLER_IN_ARRAY);
        if (newCount < controllersConnected)
            StartCoroutine(UIManager.showMessageBox(GameObject.Find("MainCanvas").GetComponent<Canvas>(), "Lost connection to controller"));
        else if (newCount > controllersConnected)
            StartCoroutine(UIManager.showMessageBox(GameObject.Find("MainCanvas").GetComponent<Canvas>(), "Controller connected"));

        controllersConnected = newCount;
    }

    /// <summary>
    /// Called when a player dies to check whether the game is over
    /// </summary>
    /// <param name="deadPlayer"></param>
    public virtual void PlayerDied(GameObject deadPlayer)
    {
        ShowGlobalMessage(deadPlayer.GetComponent<CharacterNetwork>().PlayerName + " has died!");

        switch (gameMode)
        {
            case GameModes.TeamDeathmatch:
                TeamDeatchMatch();
                break;
        }
    }

    protected abstract void InstantiatePlayers();
    protected abstract void BuildUI();

    /// <summary>
    /// Shows the game over overlay and stops camera movement
    /// </summary>
    /// <param name="winner"></param>
    protected virtual void GameOver(string GameEndText)
    {
        // Loop through every Team
        foreach (Team actTeam in Teams)
        {
            // Foreach Player in Team
            foreach (Player actPlayer in actTeam.TeamPlayers)
            {
                // Disable user input so players cant control their chamnpions when game is over
                actPlayer.InstantiatedPlayer.GetComponent<UserControl>().enabled = false;
                actPlayer.InstantiatedPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }

        gameOverOverlay.Find("Title").GetComponent<Text>().text = GameEndText;
        gameOverOverlay.gameObject.SetActive(true);
        Camera.main.GetComponent<CameraBehaviour>().gameRunning = false;
    }

    /// <summary>
    /// Called, when the restart button in the game over menu is clicked
    /// </summary>
    public virtual void Restart()
    {

    }

    /// <summary>
    /// Displays a message in huge letters for a moment
    /// </summary>
    /// <param name="message"></param>
    protected virtual void ShowGlobalMessage(string message)
    {
        GameObject messageObject = Instantiate(simpleGlobalMessage, mainCanvas, false);
        messageObject.GetComponent<Text>().text = message;
    }

    #region Win Conditions

    /// <summary>
    /// ends the game if all players of one team are dead
    /// </summary>
    protected virtual void TeamDeatchMatch()
    {
        List<int> TeamsAlive = new List<int>();

        // Loop through every Team
        foreach (Team actTeam in Teams)
        {
            //Check if all player on team 1 are dead
            if (actTeam.TeamPlayers.All(x => x.InstantiatedPlayer.GetComponent<CharacterStats>().CharacterDied == false))
            {
                TeamsAlive.Add(actTeam.TeamNumber);
            }
        }

        switch (TeamsAlive.Count)
        {
            case 0:
                GameOver("Draw");
                break;
            case 1:
                GameOver("Team " + TeamsAlive[0] + " won the game");
                break;
        }
    }

    #endregion
}
