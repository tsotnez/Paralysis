using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class StartMultiplayer : MonoBehaviour {

    public string sceneToLoadMulti = "NetworkAPITestLobby";
    public string sceneToLoadBack = "MainMenu";
    public Button createOnlineRoomButton;
    public Button joinOnlineRoomButton;
    public Button backButton;
    public Text roomNameText;
    public Dropdown regionDropDown;
    public GameObject ErrorDialog;
    public Text ErrorText;

    private bool creatingRoom = false;
    private bool joiningRoom = false;

    private void Start()
    {
        Button errorOKButton = ErrorDialog.GetComponentInChildren<Button>();

        createOnlineRoomButton.onClick.AddListener(onClickCreateRoom);
        joinOnlineRoomButton.onClick.AddListener(onClickJoinRoom);
        errorOKButton.onClick.AddListener(onClickCloseErrorDialog);
        backButton.onClick.AddListener(OnBackButtonClicked);

        regionDropDown.onValueChanged.AddListener(delegate {onRegionChanged(regionDropDown); });

        GameNetwork.Instance.OnGameStateUpdate += OnGameStateUpdated;

    }

    public void onClickCreateRoom()
    {
        string region = regionDropDown.options[regionDropDown.value].text;
        GameNetwork.Instance.Connect(region);
        setButtonsToActive(false);
        creatingRoom = true;

    }

    public void onClickJoinRoom()
    {
        if(roomNameText.text != "" || roomNameText.text == null)
        {
            string region = regionDropDown.options[regionDropDown.value].text;
            GameNetwork.Instance.Connect(region);
            joiningRoom = true;
        }
        else
        {
            showError("Room name field empty.");
        }
    }

    public void onClickStartSinglePlayer()
    {        
        setButtonsToActive(false);
        StartCoroutine(waitForGameNetworkDestroyed());
    }

    public void onClickCloseErrorDialog()
    {
        ErrorDialog.SetActive(false);
    }

    public void onRegionChanged(Dropdown change)
    {
        if(GameNetwork.Instance.IsConnected())
        {
            GameNetwork.Instance.Disconnect();
        }
    }

    private void setButtonsToActive(bool active)
    {
        createOnlineRoomButton.interactable = active;
        joinOnlineRoomButton.interactable = active;
        backButton.interactable = active;
        regionDropDown.interactable = active;
    }

    private void showError(string errorText)
    {
        ErrorDialog.SetActive(true);
        ErrorText.text = errorText;
        creatingRoom = false;
        joiningRoom = false;
        setButtonsToActive(true);
    }

    private void OnFailedToconnectToPhoton(DisconnectCause cause)
    {
        showError(cause.ToString());
    }

    private void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        showError((string)codeAndMsg[1]);
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        showError((string)codeAndMsg[1]);
    }

    private void OnConnectedToMaster()
    {
        if(creatingRoom)
        {
            if(!GameNetwork.Instance.createRandomPrivateRoom())
            {
                showError("Failed to create prive room");
            }
        }
        else if(joiningRoom)
        {
            if(!GameNetwork.Instance.joinRoom(roomNameText.text))
            {
                showError("Failed to join room");
            }
        }

        creatingRoom = false;
        joiningRoom = false;
    }

    //Photon callback
    private void OnJoinedRoom()
    {
        if(GameNetwork.Instance.IsMasterClient){
            SceneManager.LoadScene(sceneToLoadMulti);
        }
    }

    private void OnGameStateUpdated()
    {
        if(!GameNetwork.Instance.IsMasterClient){
            SceneManager.LoadScene(sceneToLoadMulti);
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
        SceneManager.LoadScene(sceneToLoadBack);
    }

    private void OnDestroy()
    {
        GameNetwork.Instance.OnGameStateUpdate -= OnGameStateUpdated;
    }
}
