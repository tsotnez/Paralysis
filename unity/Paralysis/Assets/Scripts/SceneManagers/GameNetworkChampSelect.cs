using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkChampSelect : MonoBehaviour {

    public static GameNetworkChampSelect Instance;
    private PhotonView photonV;

    public delegate void otherPlayerSelectedChamp(int photonID, int chamNump);
    public event otherPlayerSelectedChamp OnPlayerSelectedChamp;

    private void Awake()
    {
        Instance = this;
        photonV = GetComponent<PhotonView>();
    }

    public void selectedChamipon(int champion)
    {
        photonV.RPC("RPC_SelectedChampion", PhotonTargets.Others, (short)PhotonNetwork.player.ID, (short)champion);
    }

    [PunRPC]
    private void RPC_SelectedChampion(short photonId, short sChamp)
    {
        if(OnPlayerSelectedChamp != null)
            OnPlayerSelectedChamp((int)photonId, (int)sChamp);
    }
}
