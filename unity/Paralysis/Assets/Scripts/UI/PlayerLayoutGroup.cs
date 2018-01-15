using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour
{
    private const float TIME_TO_WAIT = 1f;

    [SerializeField]
    private GameObject playerListingPrefab;
    private GameObject PlayerListingPrefab
    {
        get { return playerListingPrefab; }
    }

    private List<PlayerListing> playerListings;
    private List<PlayerListing> PlayerListings
    {
        get { return playerListings; }
    }

    private void Start()
    {
        StartCoroutine(waitToPopulate());
    }

    private IEnumerator waitToPopulate()
    {
        yield return new WaitForSecondsRealtime(TIME_TO_WAIT);
        playerListings = new List<PlayerListing>();
        PhotonPlayer[] photonPlayers = GameNetwork.Instance.getPlayerList();
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    private void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null)
            return;

        PlayerLeftRoom(photonPlayer);

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);
        PlayerListings.Add(playerListing);
    }

    private void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        int index = PlayerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);

        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    //TODO - KW these photon callbacks should be going through game network...
    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        GameNetwork.Instance.leaveCurrentRoom();
    }

    //Called by photon when a player joins the room.
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }

    //Called by photon when a player leaves the room.
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }
}
