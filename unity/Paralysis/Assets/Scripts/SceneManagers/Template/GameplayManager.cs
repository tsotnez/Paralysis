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
    public static Dictionary<int, Player[]> Teams;

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

    public enum GameModes
    {
        TeamDeathmatch
    }

    protected virtual void Awake()
    {
        Instance = this;

        PhotonNetwork.offlineMode = true;
        instantiatePlayers();
        buildUI();

        simpleGlobalMessage = Resources.Load<GameObject>("Prefabs/UI/GlobalMessages/SimpleGlobalMessage");
        playerInteractionGlobalMessage = Resources.Load<GameObject>("Prefabs/UI/GlobalMessages/PlayerInteractionGlobalMessage");
        mainCanvas = GameObject.Find("MainCanvas").transform;
    }

    /// <summary>
    /// Called when a player dies to check whether the game is over
    /// </summary>
    /// <param name="deadPlayer"></param>
    public virtual void playerDied(GameObject deadPlayer)
    {
        ShowGlobalMessage(deadPlayer.GetComponent<CharacterNetwork>().PlayerName + " has died!");

        switch (gameMode)
        {
            case GameModes.TeamDeathmatch:
                teamDeatchMatch();
                break;
        }
    }
    protected abstract void instantiatePlayers();
    protected abstract void buildUI();
    /// <summary>
    /// Shows the game over overlay and stops camera movement
    /// </summary>
    /// <param name="winner"></param>
    protected virtual void GameOver(string winner)
    {
        // Loop through every Team
        for (int i = 0; i < Teams.Count; i++)
        {
            // Disable user input so players cant control their chamnpions when game is over
            Teams[i].Select(x => x.InstantiatedPlayer.GetComponent<UserControl>().enabled = false).ToList();
        }

        gameOverOverlay.Find("Title").GetComponent<Text>().text = winner + " won the game";
        gameOverOverlay.gameObject.SetActive(true);
        Camera.main.GetComponent<CameraBehaviour>().gameRunning = false;
    }

    /// <summary>
    /// Called, when the restart button in the game over menu is clicked
    /// </summary>
    public virtual void restart()
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

    #region WinConditions
    /// <summary>
    /// ends the game if all players of one team are dead
    /// </summary>
    protected virtual void teamDeatchMatch()
    {
        // Loop through every Team
        for (int i = 0; i < Teams.Count; i++)
        {
            //Check if all player on team 1 are dead
            if (Teams[i].All(x => x.InstantiatedPlayer.GetComponent<CharacterStats>().CharacterDied == true))
            {
                GameOver("Team " + (i + 1));
                return;
            }
        }
    }
    #endregion
}
