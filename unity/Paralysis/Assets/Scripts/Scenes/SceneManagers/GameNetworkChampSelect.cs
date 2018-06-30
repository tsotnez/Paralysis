using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkChampSelect : MonoBehaviour {

    public static GameNetworkChampSelect Instance;
    private PhotonView photonV;

    public delegate void otherPlayerSelectedChamp(int photonID, int chamNump);
    public event otherPlayerSelectedChamp OnPlayerSelectedChamp;

    public delegate void otherPlayerReadyUp(int photonID, int champNum, bool ready);
    public event otherPlayerReadyUp OnPlayerReady;

    public delegate void allReady();
    public event allReady OnAllReady;

    private void Awake()
    {
        Instance = this;
        photonV = GetComponent<PhotonView>();
    }

    public void selectedChamipon(int champion)
    {
        photonV.RPC("RPC_SelectedChampion", PhotonTargets.All, (short)PhotonNetwork.player.ID, (short)champion);
    }

    public void readyUp(int champID, bool ready)
    {
        photonV.RPC("RPC_PlayerReady", PhotonTargets.All, (short)PhotonNetwork.player.ID, (short)champID, ready);
    }

    public void setTrinket(Trinket.Trinkets trinket, byte trinketNum)
    {
        photonV.RPC("RPC_SetTrinketForPlayer", PhotonTargets.All, (short)PhotonNetwork.player.ID, trinketNum, (byte)trinket);
    }        

    public void allReadySignal()
    {
        if(GameNetwork.Instance.IsMasterClient)
        {
            photonV.RPC("RPC_AllReady", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void RPC_SelectedChampion(short photonId, short sChamp)
    {
        GameNetwork.Instance.setPlayerChampForId((int)photonId, (int)sChamp);
        if(OnPlayerSelectedChamp != null)
        {
            OnPlayerSelectedChamp((int)photonId, (int)sChamp);
        }
    }

    [PunRPC]
    private void RPC_PlayerReady(short photonId, short sChamp, bool ready)
    {
        GameNetwork.Instance.setPlayerChampForId((int)photonId, (int)sChamp);
        if(OnPlayerReady != null)
            OnPlayerReady((int)photonId, (int)sChamp, ready);
    }

    [PunRPC]
    private void RPC_AllReady()
    {
        if(OnAllReady != null)
            OnAllReady();
    }

    [PunRPC]
    private void RPC_SetTrinketForPlayer(short photonId, byte trinketNum, byte trinket)
    {
        GameNetwork.Instance.setTrinketForId((int)photonId, (int)trinketNum, trinket);
    }
}
