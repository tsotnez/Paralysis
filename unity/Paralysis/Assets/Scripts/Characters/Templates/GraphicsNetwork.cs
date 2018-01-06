using UnityEngine;

/// <summary>
/// Snchronizes animation variables
/// </summary>
public class GraphicsNetwork : MonoBehaviour {

    ChampionAnimationController animCon;
    SpriteRenderer ren;

    // Use this for initialization
    void Awake () {
        animCon = GetComponent<ChampionAnimationController>();
        ren = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ren.color = GameObject.Find("manager").GetComponent<GameplayManager>().championSpriteOverlayColor;
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

    #region RPC
    //Jan --- RPC Methods for networking. These are called from the client instance to syncronize animation
    [PunRPC]
    void setBasicAttack1()
    {
        animCon.trigBasicAttack1 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setBasicAttack2()
    {
        animCon.trigBasicAttack2 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setBasicAttack3()
    {
        animCon.trigBasicAttack3 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setSkill1()
    {
        animCon.trigSkill1 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setSkill2()
    {
        animCon.trigSkill2 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setSkill3()
    {
        animCon.trigSkill3 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setSkill4()
    {
        animCon.trigSkill4 = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setKnockedBack()
    {
        animCon.trigKnockedBack = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setKnockedBackEnd()
    {
        animCon.trigKnockedBackEnd = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setJump()
    {
        animCon.trigJump = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setJumpAttack()
    {
        animCon.trigJumpAttack = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setJumpAttackEnd()
    {
        animCon.trigJumpAttackEnd = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setDash()
    {
        animCon.trigDash = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setDashEnd()
    {
        animCon.trigDashEnd = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setDashForward()
    {
        animCon.trigDashForward = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setDashForwardEnd()
    {
        animCon.trigDashForwardEnd = true;
        animCon.setByRPC = true;
    }
    [PunRPC]
    void setHit()
    {
        animCon.trigHit = true;
        animCon.setByRPC = true;
    }
    #endregion 
}
