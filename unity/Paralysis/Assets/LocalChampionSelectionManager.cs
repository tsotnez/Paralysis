using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LocalChampionSelectionManager : MonoBehaviour {

    public static Player[] team2;
    public static Player[] team1;

    /// <summary>
    /// All players
    /// </summary>
    private Player[] players;

    //Containing 2 additional platforms for 4v4
    public CanvasGroup additionalPlatforms;

    //Ready Button
    public GameObject readyButton;

    public GameObject[] eventSystems;
    public GameObject[] firstSelecteds;

    public GameObject[] platformsTeam1;
    public GameObject[] platformsTeam2;

    // Use this for initialization
    void Start () {

        //Set default Values for debugging
        if(team1 == null && team2 == null)
        {
            team1 = new Player[1];
            team2 = new Player[1];

            team1[0] = new Player(UserControl.PlayerNumbers.Player2, UserControl.InputDevice.KeyboardMouse, 1);
            team2[0] = new Player(UserControl.PlayerNumbers.Player1, UserControl.InputDevice.XboxController, 2);
        }
        
        //Show additionjal player Platforms if 2v2
        if(team1.Length == 2 && team2.Length == 2)
        {
            additionalPlatforms.interactable = true;
            additionalPlatforms.alpha = 1;
            additionalPlatforms.blocksRaycasts = true;
        }

        //All players are the sum of both teams
        players = team1.Concat(team2).ToArray();
        setUpEventSystems();
	}

    private void Update()
    {
        //Show Ready button only when everything is selected
        bool allSelected = true;
        //Check if every player has selected a champion and 2 different trinkets
        foreach (Player player in players)
            if (player.ChampionPrefab == null || player.trinket1 == player.trinket2)
                allSelected = false;

        readyButton.SetActive(allSelected);
    }

    /// <summary>
    /// Sets the champion value for player object and shows a preview for the champion
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="Champion"></param>
    public void setChampion(UserControl.PlayerNumbers targetPlayer, GameObject Champion)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer); //Get player object out of array
        target.ChampionPrefab = Champion;

        GameObject[] platformGroup;
        bool flip;

        if(target.TeamNumber == 1) 
        {
            platformGroup = platformsTeam1;
            flip = false;
        }
        else //If player is in team 2, use platforms of team 2 and flip the sprite
        {
            platformGroup = platformsTeam2;
            flip = true;
        }

        if (team1.Length == 2 && team2.Length == 2)//if 2v2
        {
            if (team1.First(x => x != target).playerNumber > target.playerNumber) //If target has the highest playerNumber in team, use second platform
            {
                DestroyAllChildren(platformGroup[1].transform);
                ShowPrefab(Champion, platformGroup[1].transform, flip);
            }
            else
            {
                //Use first platform
                DestroyAllChildren(platformGroup[0].transform);
                ShowPrefab(Champion, platformGroup[0].transform, flip);
            }
        }
        else //Just use first platform
        {
            DestroyAllChildren(platformGroup[0].transform);
            ShowPrefab(Champion, platformGroup[0].transform, flip);
        }
    }

    /// <summary>
    ///Remove existent Preview GameObject by destroying all children of a Platform
    /// </summary>
    /// <param name="parent"></param>
    private void DestroyAllChildren(Transform parent)
    {
        var children = new List<GameObject>();
        foreach (Transform child in parent) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }


    /// <summary>
    /// Changes the Champions prefab to fit a preview by removing unnecessary stuff and setting new values
    /// </summary>
    /// <param name="Champion"></param>
    /// <param name="parent"></param>
    private void ShowPrefab(GameObject Champion, Transform parent, bool flip)
    {
        Champion.SetActive(false);
        GameObject newPreview = Instantiate(Champion, parent, false);
        Destroy(newPreview.GetComponent<ChampionClassController>());
        Destroy(newPreview.GetComponent<Rigidbody2D>());
        Destroy(newPreview.GetComponent<CharacterStats>());
        Destroy(newPreview.GetComponent<UserControl>());
        Destroy(newPreview.GetComponent<BoxCollider2D>());
        Destroy(newPreview.GetComponent<CircleCollider2D>());
        Destroy(newPreview.transform.Find("GroundCheck").gameObject);
        Destroy(newPreview.transform.Find("Canvas").gameObject);
        Destroy(newPreview.transform.Find("stunnedSymbol").gameObject);

        //flip sprite if necessary
        int direction = 1;
        if (flip)
            direction = -1;

        newPreview.transform.localScale = new Vector3(200 * direction, 200, 1); //Scale up
        newPreview.transform.position = new Vector3(newPreview.transform.position.x, newPreview.transform.position.y + 1.3f, newPreview.transform.position.z);
        newPreview.transform.Find("graphics").GetComponent<ChampionAnimationController>().m_Grounded = true;
        newPreview.transform.Find("graphics").GetComponent<ChampionAnimationController>().trigBasicAttack1 = true;
        newPreview.SetActive(true);
        Champion.SetActive(true);
    }

    public void setTrinket1(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    {
        players.First(x => x.playerNumber == targetPlayer).trinket1 = trinketName;
    }

    public void setTrinket2(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName)
    {
        players.First(x => x.playerNumber == targetPlayer).trinket2 = trinketName;
    }

    public void setTrinket(UserControl.PlayerNumbers targetPlayer, Trinket.Trinkets trinketName, Trinket.Trinkets toOverwrite)
    {
        Player target = players.First(x => x.playerNumber == targetPlayer);

        if (target.trinket1 == toOverwrite)
            target.trinket1 = trinketName;
        else
            target.trinket2 = trinketName;
    }

    /// <summary>
    /// Generates Eventsystems based on how many Input devices are connected
    /// </summary>
    private void setUpEventSystems()
    {
        foreach (Player player in players)
        {
            switch (player.playerNumber)
            {
                case UserControl.PlayerNumbers.Player1:
                    instantiateEventSystem(player, 1);
                    break;
                case UserControl.PlayerNumbers.Player2:
                    instantiateEventSystem(player, 2);
                    break;
                case UserControl.PlayerNumbers.Player3:
                    instantiateEventSystem(player, 3);
                    break;
                case UserControl.PlayerNumbers.Player4:
                    instantiateEventSystem(player, 4);
                    break;
            }
        }
    }

    /// <summary>
    /// Instantiates EventSystem and sets its variables
    /// </summary>
    /// <param name="player"></param>
    /// <param name="ID"></param>
    private void instantiateEventSystem(Player player, int ID)
    {
        Transform parent = GameObject.Find("EventSystems").transform;

        GameObject newSystem = Instantiate(eventSystems[ID - 1], parent, false);
        StandaloneInputModule inputModule = newSystem.GetComponent<StandaloneInputModule>();

        //Set first selected by getting Array Value
        newSystem.GetComponent<EventSystem>().firstSelectedGameObject = firstSelecteds[ID - 1];

        if (player.inputDevice == UserControl.InputDevice.KeyboardMouse)
        {
            //Set axis for Keyboard
            inputModule.horizontalAxis = "Horizontal";
            inputModule.verticalAxis = "Vertical";
            inputModule.submitButton = "Submit";
        }
        else if (player.inputDevice == UserControl.InputDevice.XboxController)
        {
            //Set axis for xbox
            inputModule.horizontalAxis = "Horizontal_XboxPlayer" + ID.ToString();
            inputModule.verticalAxis = "Vertical_XboxPlayer" + ID.ToString();
            inputModule.submitButton = "Skill4_XboxPlayer" + ID.ToString();
        }
    }

    public void startGame()
    {
        //Check if every player has selected a champion and 2 different trinkets
        foreach (Player player in players)
            if (player.ChampionPrefab == null || player.trinket1 == player.trinket2)
                return;
        
        LocalMultiplayerManager.team1 = team1;
        LocalMultiplayerManager.team2 = team2;

        SceneManager.LoadScene("scenes/test");
    }
}
