using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour {

    public PhotonPlayer PhotonPlayer { get; private set; }

    [SerializeField]
    private Text _playerName;
    private Text PlayerName
    {
        get { return _playerName; }
    }

    private void Start()
    {
        if (!GameNetwork.Instance.IsMasterClient)
        {
            this.GetComponent<Button>().interactable = false;
        }
    }

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        PlayerName.text = photonPlayer.NickName;
    }

    public void OnClick()
    {
        GameNetwork.Instance.KickPlayer(_playerName.text);
    }
}
