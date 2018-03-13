using UnityEngine;

/// <summary>
/// Snchronizes animation variables
/// </summary>
public class GraphicsNetwork : MonoBehaviour {

    ChampionAnimationController animCon;
    SpriteRenderer ren;

    // Use this for initialization
    void Awake () {

        if(!PhotonNetwork.offlineMode && GameNetwork.Instance.InGame) 
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
        if(!GameNetwork.Instance.InGame) return;

        if (stream.isWriting)
        {
            //This is our player, sending info
            stream.SendNext(animCon.propGrounded);
            stream.SendNext(animCon.propVSpeed);
            stream.SendNext(animCon.propSpeed);
            stream.SendNext(animCon.statDead);
            stream.SendNext(animCon.statPreview);
            stream.SendNext(animCon.statStunned);
            stream.SendNext(animCon.statBlock);
        }
        else
        {
            //Someone elses Player, receive info and set values
            animCon.propGrounded = (bool)stream.ReceiveNext();
            animCon.propVSpeed = (float)stream.ReceiveNext();
            animCon.propSpeed = (float)stream.ReceiveNext();
            animCon.statDead = (bool)stream.ReceiveNext();
            animCon.statPreview = (bool)stream.ReceiveNext();
            animCon.statStunned = (bool)stream.ReceiveNext();
            animCon.statBlock = (bool)stream.ReceiveNext();
        }
    }
}
