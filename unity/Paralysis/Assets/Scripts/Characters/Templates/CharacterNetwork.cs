using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNetwork : Photon.MonoBehaviour {

    ChampionAnimationController animCon;
    SpriteRenderer r;
    Transform graphicsTransform;

    private void Awake()
    {
        animCon = GetComponentInChildren<ChampionAnimationController>();
        r = transform.Find("graphics").GetComponent<SpriteRenderer>();
        graphicsTransform = transform.Find("graphics");
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
        }
    }

}
