public abstract class ChampionAnimationController : AnimationController
{
    // Vars
    public bool m_Grounded;                                              // Whether or not the player is grounded.
    public float m_vSpeed;                                               // Vertical speed
    public float m_Speed;                                                // Horizontal speed

    // Stats
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

    protected virtual void Update()
    {
        AnimationManager();
    }

    protected virtual void FixedUpdate() { }

    private void AnimationManager()
    {
        // Animations that work in any State
        if (statStunned)
            StartAnimation(AnimatorStates.Stunned);
        else if (trigKnockedBack)
        {
            trigKnockedBack = false;
            StartAnimation(AnimatorStates.KnockedBack);
        }
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
                        trigKnockedBackEnd = false;
                        StartAnimation(AnimatorStates.KnockedBack, TypeOfAnimation.EndAnimation);
                    }
                    return;
                case AnimatorStates.JumpAttack:
                    if (trigJumpAttackEnd)
                    {
                        trigJumpAttackEnd = false;
                        StartAnimation(AnimatorStates.JumpAttack, TypeOfAnimation.EndAnimation);
                    }
                    return;
            }

            // For character specific animations
            if (AdditionalAnimationCondition()) return;

            if (statBlock)
                StartAnimation(AnimatorStates.Block);
            else if(statBlock && m_Speed > 0.001)
                StartAnimation(AnimatorStates.BlockMove);
            else if (trigDash)
            {
                trigDash = false;
                StartAnimation(AnimatorStates.Dash);
            }
            else if (trigDashForward)
            {
                trigDashForward = false;
                StartAnimation(AnimatorStates.DashFor);
            }
            else if (trigHit)
            {
                trigHit = false;
                StartAnimation(AnimatorStates.Hit);
            }
            else if (trigJumpAttack)
            {
                trigJumpAttack = false;
                StartAnimation(AnimatorStates.JumpAttack);
            }
            else if (trigBasicAttack1)
            {
                trigBasicAttack1 = false;
                StartAnimation(AnimatorStates.BasicAttack1);
            }
            else if (trigBasicAttack2)
            {
                trigBasicAttack2 = false;
                StartAnimation(AnimatorStates.BasicAttack2);
            }
            else if (trigBasicAttack3)
            {
                trigBasicAttack3 = false;
                StartAnimation(AnimatorStates.BasicAttack3);
            }
            else if (trigSkill1)
            {
                trigSkill1 = false;
                StartAnimation(AnimatorStates.Skill1);
            }
            else if (trigSkill2)
            {
                trigSkill2 = false;
                StartAnimation(AnimatorStates.Skill2);
            }
            else if (trigSkill3)
            {
                trigSkill3 = false;
                StartAnimation(AnimatorStates.Skill3);
            }
            else if (trigSkill4)
            {
                trigSkill4 = false;
                StartAnimation(AnimatorStates.Skill4);
            }
            else if (!m_Grounded && m_vSpeed < 0)
                StartAnimation(AnimatorStates.Fall);
            else if (!m_Grounded && m_vSpeed > 0 && trigJump)
            {
                StartAnimation(AnimatorStates.Jump);
                trigJump = false;
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