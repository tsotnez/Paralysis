public abstract class ChampionAnimationController : AnimationController
{
    PhotonView view;

    /// <summary>
    /// Indicates wheter a trigger was set by a RPC
    /// </summary>
    protected bool setByRPC = false;

    // Vars
    public bool m_Grounded;                                              // Whether or not the player is grounded.
    public float m_vSpeed;                                               // Vertical speed
    public float m_Speed;                                                // Horizontal speed

    // Stats
    public bool statDead = false;
    public bool statPreview = false;
    public bool statStunned = false;
    public bool statBlock = false;

    // Trigger
    public bool trigBasicAttack1 = false;
    public bool trigBasicAttack2 = false;
    public bool trigBasicAttack3 = false;
    public bool trigSkill1 = false;
    public bool trigSkill2 = false;
    public bool trigSkill3 = false;
    public bool trigSkill4 = false;
    public bool trigKnockedBack = false;
    public bool trigKnockedBackEnd = false;
    public bool trigJump = false;
    public bool trigJumpAttack = false;
    public bool trigJumpAttackEnd = false;
    public bool trigDash = false;
    public bool trigDashForward = false;
    public bool trigHit = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    protected virtual void Update()
    {
        if (CurrentAnimation != AnimatorStates.Die)
        {
            AnimationManager();
        }
    }

    protected virtual void FixedUpdate() { }

    private void AnimationManager()
    {
        // Animations that work in any State
        if (statPreview)
        {
            if (trigBasicAttack1)
            {
                //Jan --- Dont issue RPC when already called by RPC 
                //if(!setByRPC)
                //    view.RPC("setBasicAttack1", PhotonTargets.Others);
                trigBasicAttack1 = false;
                StartAnimation(AnimatorStates.BasicAttack1);

                //Reset indicator of RPC
                setByRPC = false;
                return;
            }
            StartAnimation(AnimatorStates.Idle);
        }
        else if (statDead)
        {
            StartAnimation(AnimatorStates.Die, TypeOfAnimation.Animation, AnimationPlayTypes.HoldOnEnd);
        }
        else if (trigKnockedBack)
        {
            if (!setByRPC && !PhotonNetwork.offlineMode)
                view.RPC("setKnockedBack", PhotonTargets.Others);
            trigKnockedBack = false;
            StartAnimation(AnimatorStates.KnockedBack);
            setByRPC = false;
        }
        else if (statStunned && CurrentAnimation != AnimatorStates.KnockedBack)
            StartAnimation(AnimatorStates.Stunned);
        else
        {
            // don't interrupt these animations (equivalent to HasExitTime)
            if (AdditionalNotInterruptCondition(CurrentAnimation)) return;

            switch (CurrentAnimation)
            {
                case AnimatorStates.Dash:
                case AnimatorStates.DashFor:
                case AnimatorStates.BasicAttack1:
                case AnimatorStates.BasicAttack2:
                case AnimatorStates.BasicAttack3:
                case AnimatorStates.Skill1:
                case AnimatorStates.Skill2:
                case AnimatorStates.Skill3:
                case AnimatorStates.Skill4:
                    return;
                case AnimatorStates.KnockedBack:
                    if (trigKnockedBackEnd)
                    {
                        if (!setByRPC && !PhotonNetwork.offlineMode)
                            view.RPC("setKnockedBackEnd", PhotonTargets.Others);
                        trigKnockedBackEnd = false;
                        StartAnimation(AnimatorStates.KnockedBack, TypeOfAnimation.EndAnimation);
                        setByRPC = false;
                    }
                    return;
                case AnimatorStates.JumpAttack:
                    if (trigJumpAttackEnd)
                    {
                        if (!setByRPC && !PhotonNetwork.offlineMode)
                            view.RPC("setJumpAttackEnd", PhotonTargets.Others);
                        trigJumpAttackEnd = false;
                        StartAnimation(AnimatorStates.JumpAttack, TypeOfAnimation.EndAnimation);
                        setByRPC = false;
                    }
                    return;
            }

            // For character specific animations
            if (AdditionalAnimationCondition()) return;

            if (statBlock && m_Speed <= 0)
                StartAnimation(AnimatorStates.Block);
            else if(statBlock && m_Speed > 0.001)
                StartAnimation(AnimatorStates.BlockMove);
            else if (trigDash)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setDash", PhotonTargets.Others);
                trigDash = false;
                StartAnimation(AnimatorStates.Dash);
                setByRPC = false;
            }
            else if (trigDashForward)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setDashForward", PhotonTargets.Others);
                trigDashForward = false;
                StartAnimation(AnimatorStates.DashFor);
                setByRPC = false;
            }
            else if (trigHit)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setHit", PhotonTargets.Others);
                trigHit = false;
                StartAnimation(AnimatorStates.Hit);
                setByRPC = false;
            }
            else if (trigJumpAttack)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setJumpAttack", PhotonTargets.Others);
                trigJumpAttack = false;
                StartAnimation(AnimatorStates.JumpAttack);
                setByRPC = false;
            }
            else if (trigBasicAttack1)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setBasicAttack1", PhotonTargets.Others);
                trigBasicAttack1 = false;
                StartAnimation(AnimatorStates.BasicAttack1);
                setByRPC = false;
            }
            else if (trigBasicAttack2)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setBasicAttack2", PhotonTargets.Others);
                trigBasicAttack2 = false;
                StartAnimation(AnimatorStates.BasicAttack2);
                setByRPC = false;
            }
            else if (trigBasicAttack3)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setBasicAttack3", PhotonTargets.Others);
                trigBasicAttack3 = false;
                StartAnimation(AnimatorStates.BasicAttack3);
                setByRPC = false;
            }
            else if (trigSkill1)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setSkill1", PhotonTargets.Others);
                trigSkill1 = false;
                StartAnimation(AnimatorStates.Skill1);
                setByRPC = false;
            }
            else if (trigSkill2)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setSkill2", PhotonTargets.Others);
                trigSkill2 = false;
                StartAnimation(AnimatorStates.Skill2);
                setByRPC = false;
            }
            else if (trigSkill3)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setSkill3", PhotonTargets.Others);
                trigSkill3 = false;
                StartAnimation(AnimatorStates.Skill3);
                setByRPC = false;
            }
            else if (trigSkill4)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setSkill4", PhotonTargets.Others);
                trigSkill4 = false;
                StartAnimation(AnimatorStates.Skill4);
                setByRPC = false;
            }
            else if (!m_Grounded && m_vSpeed <= 0)
                StartAnimation(AnimatorStates.Fall);
            else if (!m_Grounded && m_vSpeed > 0 && trigJump)
            {
                if (!setByRPC && !PhotonNetwork.offlineMode)
                    view.RPC("setJump", PhotonTargets.Others);
                StartAnimation(AnimatorStates.Jump);
                trigJump = false;
                setByRPC = false;
            }
            else if (m_Grounded && m_Speed > 0)
                StartAnimation(AnimatorStates.Run);
            else if (m_Grounded)
                StartAnimation(AnimatorStates.Idle);
        }
    }

    protected abstract bool AdditionalNotInterruptCondition(AnimatorStates activeAnimation);
    protected abstract bool AdditionalAnimationCondition();
}