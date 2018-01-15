using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMultiplayer : MonoBehaviour {

    public string sceneToLoad = "NetworkAPITestLobby";
    public Button startMultiplayerButton;

    public void onClickStartMultiplayer()
    {
        GameNetwork.Instance.Connect();
        startMultiplayerButton.interactable = false;
    }

    private void OnConnectedToMaster()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
