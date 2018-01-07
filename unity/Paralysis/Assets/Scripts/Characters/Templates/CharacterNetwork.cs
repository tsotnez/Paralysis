using UnityEngine;
using System.Linq;

/// <summary>
/// Synchronizes game mechanic values (excluding the transform)
/// </summary>
public class CharacterNetwork : Photon.MonoBehaviour {


    public string PlayerName = "Player";
    SpriteRenderer r;
    Transform graphicsTransform;
    CharacterStats stats;
    SpriteRenderer shadowRenderer;
    GameObject stunnedSymbol;

    private void Awake()
    {
        r = transform.Find("graphics").GetComponent<SpriteRenderer>();
        graphicsTransform = transform.Find("graphics");
        stats = GetComponent<CharacterStats>();
        shadowRenderer = transform.Find("GroundCheck").GetComponent<SpriteRenderer>();
        stunnedSymbol = transform.Find("stunnedSymbol").gameObject;
    }

    public void joinTeam()
    {
        if (photonView.isMine)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("MainPlayer").Where(x => x != gameObject).ToArray();

            //Join the team with lesser players
            if (players.Where(x => x.layer == 11).Count() <= players.Where(x => x.layer == 12).Count())
                photonView.RPC("SetTeam", PhotonTargets.All, 11, 12); //Join Team 1
            else
                photonView.GetComponent<PhotonView>().RPC("SetTeam", PhotonTargets.All, 12, 11);
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
        gameObject.layer = team;

        LayerMask whatToHit = new LayerMask();
        whatToHit |= (1 << teamToHit);
        GetComponent<ChampionClassController>().m_whatToHit = whatToHit;

        stats.SetTeamColor();
    }

    /// <summary>
    /// Refreshes the list of player instances when a new player gets instantiated
    /// </summary>
    [PunRPC]
    private void OnNewPlayerInstantiated()
    {
        GameObject.Find("manager").GetComponent<GameplayManager>().players = GameObject.FindGameObjectsWithTag("MainPlayer").ToList();
    }
    #endregion 
}
