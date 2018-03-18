using UnityEngine;
using System.Linq;

/// <summary>
/// Synchronizes game mechanic values (excluding the transform)
/// </summary>
public class CharacterNetwork : Photon.MonoBehaviour
{
    public string PlayerName = "Player";
    SpriteRenderer r;
    Transform graphicsTransform;
    CharacterStats stats;
    SpriteRenderer shadowRenderer;
    GameObject stunnedSymbol;

    private void Awake()
    {
        if(PhotonNetwork.offlineMode)
        {
            enabled = false;
        }
        else
        {
            r = transform.Find("graphics").GetComponent<SpriteRenderer>();
            graphicsTransform = transform.Find("graphics");
            stats = GetComponent<CharacterStats>();
            shadowRenderer = transform.Find("GroundCheck").GetComponent<SpriteRenderer>();
            stunnedSymbol = transform.Find("stunnedSymbol").gameObject;
            PlayerName = GameNetwork.Instance.PlayerName;
        }
    }

    public void joinTeam()
    {
        if (photonView.isMine)
        {
            if(GameNetwork.Instance.TeamNum == 1)
            {
                photonView.RPC("SetTeam", PhotonTargets.All, GameConstants.TEAMLAYERS[0], GameConstants.TEAMLAYERS[1]); //Join Team 1
            }
            else 
            {
                photonView.RPC("SetTeam", PhotonTargets.All, GameConstants.TEAMLAYERS[1], GameConstants.TEAMLAYERS[0]); //Join Team 2
            }                
        }
    }

    /// <summary>
    /// Sending and receiving animation data
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //This is our player, sending info
            stream.SendNext(r.flipX);
            stream.SendNext(graphicsTransform.localScale);
            stream.SendNext(stats.CurrentHealth);
            stream.SendNext(stats.CurrentStamina);
            stream.SendNext(shadowRenderer.enabled);
            stream.SendNext(stunnedSymbol.activeSelf);
            stream.SendNext(gameObject.layer);
        }
        else
        {
            //Someone elses Player, receive info and set values
            r.flipX = (bool)stream.ReceiveNext();
            graphicsTransform.localScale = (Vector3)stream.ReceiveNext();
            stats.CurrentHealth = (int)stream.ReceiveNext();
            stats.CurrentStamina = (int)stream.ReceiveNext();
            shadowRenderer.enabled = (bool)stream.ReceiveNext();
            stunnedSymbol.SetActive(((bool)stream.ReceiveNext()));
            gameObject.layer = (int)stream.ReceiveNext();
            stats.SetTeamColor();
        }
    }

    #region RPC
    //Jan --- RPC Methods for networking. These are called from the client instance
    [PunRPC]
    void SetTeam(int team, int teamToHit)
    {
        if(!photonView.isMine)
        {
            CameraBehaviour cam = Camera.main.GetComponent<CameraBehaviour>();
            cam.AddTargetToCamera(this.transform);
        }

        gameObject.layer = team;

        LayerMask whatToHit = new LayerMask();
        whatToHit |= (1 << teamToHit);
        GetComponent<ChampionClassController>().m_whatToHit = whatToHit;

        if(stats == null)
            stats = GetComponent<CharacterStats>();
        stats.SetTeamColor();
    }
    #endregion 
}
