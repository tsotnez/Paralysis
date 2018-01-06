using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class for all gameplay scene managers
/// </summary>
public abstract class GameplayManager : Photon.MonoBehaviour {

    /// <summary>
    /// Current GameMode, default is TeamDeathmatch
    /// </summary>
    public GameModes gameMode = GameModes.TeamDeathmatch;

    //GameOver overlay
    public Transform gameOverOverlay;

    //Team Arrays containing Players
    public static Player[] team1;
    public static Player[] team2;

    //Redundant variables so they can be assigned in the inspector (static ones cant)
    public Player defaultPlayer1 = null;
    public Player defaultPlayer2 = null;

    //Color to apply to player images to fit the environemts lighting
    public Color championSpriteOverlayColor;

    //[HideInInspector]
    //Array containign all player game objects
    public List<GameObject> players = new List<GameObject>();

    public enum GameModes
    {
        TeamDeathmatch
    }

    protected virtual void Awake()
    {
        PhotonNetwork.offlineMode = true;
        instantiatePlayers();
        buildUI();
    }

    /// <summary>
    /// Called when a player dies to check whether the game is over
    /// </summary>
    /// <param name="deadPlayer"></param>
    public virtual void playerDied(GameObject deadPlayer)
    {
        ShowGlobalMessage(deadPlayer.GetComponent<CharacterNetwork>().PlayerName + " has died!" );

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
        //Disable user input so players cant control their chamnpions when game is over
        players.Select(x => x.GetComponent<UserControl>().enabled = false).ToList();

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
        //TODO: Display a message on top of the screen for each player to see
        Debug.Log(message);
    }

    #region WinConditions
    /// <summary>
    /// ends the game if all players of one team are dead
    /// </summary>
    protected virtual void teamDeatchMatch()
    {
        //Check if all player on team 1 are dead
        if(players.Where(x => x.layer == 11).All(x => x.GetComponent<CharacterStats>().CharacterDied == true))
        {
            GameOver("Team 2");
            return;
        }
        //Same for team 2
        if (players.Where(x => x.layer == 12).All(x => x.GetComponent<CharacterStats>().CharacterDied == true))
        {
            GameOver("Team 1");
            return;
        }
    }
    #endregion
}
