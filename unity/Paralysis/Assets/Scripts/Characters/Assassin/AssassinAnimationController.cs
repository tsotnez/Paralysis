class AssassinAnimationController : ChampionAnimationController
{ 
    // trigger for animation
    public bool trigDoubleJump = false;


    #region RPC
    //Jan --- RPC Methods for networking. These are called from the client instance to syncronize animation
    [PunRPC]
    void setBasicAttack1()
    {
        trigBasicAttack1 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setBasicAttack2()
    {
        trigBasicAttack2 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setBasicAttack3()
    {
        trigBasicAttack3 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setSkill1()
    {
        trigSkill1 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setSkill2()
    {
        trigSkill2 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setSkill3()
    {
        trigSkill3 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setSkill4()
    {
        trigSkill4 = true;
        setByRPC = true;
    }
    [PunRPC]
    void setKnockedBack()
    {
        trigKnockedBack = true;
        setByRPC = true;
    }
    [PunRPC]
    void setKnockedBackEnd()
    {
        trigKnockedBackEnd = true;
        setByRPC = true;
    }
    [PunRPC]
    void setJump()
    {
        trigJump = true;
        setByRPC = true;
    }
    [PunRPC]
    void setJumpAttack()
    {
        trigJumpAttack = true;
        setByRPC = true;
    }
    [PunRPC]
    void setJumpAttackEnd()
    {
        trigJumpAttackEnd = true;
        setByRPC = true;
    }
    [PunRPC]
    void setDash()
    {
        trigDash = true;
        setByRPC = true;
    }
    [PunRPC]
    void setDashForward()
    {
        trigDashForward = true;
        setByRPC = true;
    }
    [PunRPC]
    void setHit()
    {
        trigHit = true;
        setByRPC = true;
    }
    #endregion 

    protected override bool AdditionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        return false;
    }

    protected override bool AdditionalAnimationCondition()
    {
        if (!m_Grounded && m_vSpeed > 0.001 && trigDoubleJump)
        {
            StartAnimation(AnimationController.AnimatorStates.DoubleJump);
            trigDoubleJump = false;
            return true;
        }

        return false;
    }
}