using UnityEngine;

/// <summary>
/// Snchronizes animation variables
/// </summary>
public class GraphicsNetwork : MonoBehaviour {

    ChampionAnimationController animCon;
    SpriteRenderer ren;

    // Use this for initialization
    void Start () {

        if(!PhotonNetwork.offlineMode) 
        {
            animCon = GetComponent<ChampionAnimationController>();
            ren = GetComponent<SpriteRenderer>();
            ren.color = GameObject.Find("manager").GetComponent<GameplayManager>().championSpriteOverlayColor;
        }
        else 
        {
            enabled = false;
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
        }
    }
}
