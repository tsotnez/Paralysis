using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkChampSelect : MonoBehaviour {

    public static GameNetworkChampSelect Instance;
    private PhotonView photonV;

    public delegate void otherPlayerSelectedChamp(int networkNum, int chamNump);
    public event otherPlayerSelectedChamp OnPlayerSelectedChamp;

    private void Awake()
    {
        Instance = this;
        photonV = GetComponent<PhotonView>();
    }

    public void selectedChamipon(int champion)
    {
        photonV.RPC("RPC_SelectedChampion", PhotonTargets.Others, (short)GameNetwork.Instance.PlayerNetworkNumber, (short)champion);
    }

    [PunRPC]
    private void RPC_SelectedChampion(short playerNetworkNum, short sChamp)
    {
        if(OnPlayerSelectedChamp != null)
            OnPlayerSelectedChamp((int)playerNetworkNum, (int)sChamp);
    }
}
