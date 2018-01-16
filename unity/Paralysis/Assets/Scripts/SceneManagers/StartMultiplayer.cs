using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMultiplayer : MonoBehaviour {

    public string sceneToLoadMulti = "NetworkAPITestLobby";
    public string sceneToLoadSingle = "test";
    public Button startMultiplayerButton;
    public Button startSinglePlayerButton;
    public Dropdown regionDropDown;

    public void onClickStartMultiplayer()
    {
        string region = regionDropDown.options[regionDropDown.value].text;
        GameNetwork.Instance.Connect(region);
        startMultiplayerButton.interactable = false;
        startSinglePlayerButton.interactable = false;
    }

    public void onClickStartSinglePlayer()
    {
        startMultiplayerButton.interactable = false;
        startSinglePlayerButton.interactable = false;
        StartCoroutine(waitForGameNetworkDestroyed());
    }

    private IEnumerator waitForGameNetworkDestroyed ()
    {
        Destroy(GameNetwork.Instance.gameObject);
        yield return new WaitWhile( ()=> GameNetwork.Instance == null);
        SceneManager.LoadScene(sceneToLoadSingle);
    }

    private void OnConnectedToMaster()
    {
        SceneManager.LoadScene(sceneToLoadMulti);
    }
}
