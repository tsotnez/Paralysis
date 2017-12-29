using UnityEngine;
using System.Linq;

public class CharacterNetwork : Photon.MonoBehaviour {

    ChampionAnimationController animCon;
    SpriteRenderer r;
    Transform graphicsTransform;
    CharacterStats stats;

    private void Awake()
    {
        animCon = GetComponentInChildren<ChampionAnimationController>();
        r = transform.Find("graphics").GetComponent<SpriteRenderer>();
        graphicsTransform = transform.Find("graphics");
        stats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        if (photonView.isMine)
        {   
            GameObject[] players = GameObject.FindGameObjectsWithTag("MainPlayer");

            //Join the team with lesser players
            if (players.Where(x => x.layer == 11).Count() - 1 <= players.Where(x => x.layer == 12).Count())
                photonView.RPC("SetTeam", PhotonTargets.All, 11, 12);
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
            stream.SendNext(animCon.m_Grounded);
            stream.SendNext(animCon.m_vSpeed);
            stream.SendNext(animCon.m_Speed);
            stream.SendNext(animCon.statDead);
            stream.SendNext(animCon.statPreview);
            stream.SendNext(animCon.statStunned);
            stream.SendNext(animCon.statBlock);
            stream.SendNext(r.flipX);
            stream.SendNext(graphicsTransform.localScale);
            stream.SendNext(stats.CurrentHealth);
            stream.SendNext(stats.CurrentStamina);
        }
        else
        {
            //Someone elses Player, receive info and set values
            animCon.m_Grounded = (bool)stream.ReceiveNext();
            animCon.m_vSpeed = (float)stream.ReceiveNext();
            animCon.m_Speed = (float)stream.ReceiveNext();
            animCon.statDead = (bool)stream.ReceiveNext();
            animCon.statPreview = (bool)stream.ReceiveNext();
            animCon.statStunned = (bool)stream.ReceiveNext();
            animCon.statBlock = (bool)stream.ReceiveNext();
            r.flipX = (bool)stream.ReceiveNext();
            graphicsTransform.localScale = (Vector3)stream.ReceiveNext();
            stats.CurrentHealth = (int)stream.ReceiveNext();
            stats.CurrentStamina = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void SetTeam(int team, int teamToHit)
    {
        gameObject.layer = team;

        LayerMask whatToHit = new LayerMask();
        whatToHit |= (1 << teamToHit);
        GetComponent<ChampionClassController>().m_whatToHit = whatToHit;

        stats.setTeamColor();
    }
}
