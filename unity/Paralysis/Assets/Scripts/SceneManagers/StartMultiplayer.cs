using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMultiplayer : MonoBehaviour {

    public string sceneToLoad = "NetworkAPITestLobby";
    public Button startMultiplayerButton;
    public Dropdown regionDropDown;

    public void onClickStartMultiplayer()
    {
        string region = regionDropDown.options[regionDropDown.value].text;
        GameNetwork.Instance.Connect(region);
        startMultiplayerButton.interactable = false;
    }

    private void OnConnectedToMaster()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
