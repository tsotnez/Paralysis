using UnityEngine;

public abstract class ChampionAnimationController : AnimationController
{
    /// <summary>
    /// Indicates wheter a trigger was set by an RPC
    /// </summary>
    public bool setByRPC = false;

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
    public bool trigDoubleJump = false;
    public bool trigJumpAttack = false;
    public bool trigJumpAttackEnd = false;
    public bool trigDash = false;
    public bool trigDashEnd = false;
    public bool trigDashForward = false;
    public bool trigDashForwardEnd = false;
    public bool trigHit = false;

    // Revert to Idle Frames
    private int RevertIdleNeededFrames = 3;
    private int RevertIdleActualFrames = 0;

    protected virtual void Update()
    {
        if (!statDead || CurrentAnimation != AnimationTypes.Die)
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
                trigBasicAttack1 = false;
                StartAnimation(AnimationTypes.BasicAttack1);
                return;
            }
            StartAnimation(AnimationTypes.Idle);
        }
        else if (statDead)
        {
            StartAnimation(AnimationTypes.Die);
        }
        else if (trigKnockedBack)
        {
            trigKnockedBack = false;
            StartAnimation(AnimationTypes.KnockedBack);
        }
        else if (statStunned && CurrentAnimation != AnimationTypes.KnockedBack)
            StartAnimation(AnimationTypes.Stunned);
        else
        {
            // don't interrupt these animations (equivalent to HasExitTime)
            if (AdditionalNotInterruptCondition(CurrentAnimation)) return;

            switch (CurrentAnimation)
            {
                case AnimationTypes.BasicAttack1:
                case AnimationTypes.BasicAttack2:
                case AnimationTypes.BasicAttack3:
                case AnimationTypes.Skill1:
                case AnimationTypes.Skill2:
                case AnimationTypes.Skill3:
                case AnimationTypes.Skill4:
                    return;
                case AnimationTypes.KnockedBack:
                    if (trigKnockedBackEnd)
                    {
                        trigKnockedBackEnd = false;
                        StartEndAnimation(AnimationTypes.KnockedBack);
                    }
                    return;
                case AnimationTypes.JumpAttack:
                    if (trigJumpAttackEnd)
                    {
                        trigJumpAttackEnd = false;
                        StartEndAnimation(AnimationTypes.JumpAttack);
                    }
                    return;
                case AnimationTypes.Dash:
                    if (trigDashEnd)
                    {
                        trigDashEnd = false;
                        StartEndAnimation(AnimationTypes.Dash);
                    }
                    return;
                case AnimationTypes.DashFor:
                    if (trigDashForwardEnd)
                    {
                        trigDashForwardEnd = false;
                        StartEndAnimation(AnimationTypes.DashFor);
                    }
                    return;
            }

            // For character specific animations
            if (AdditionalAnimationCondition()) return;

            if (trigDash)
            {
                trigDash = false;
                StartAnimation(AnimationTypes.Dash);
            }
            else if (trigDashForward)
            {
                trigDashForward = false;
                StartAnimation(AnimationTypes.DashFor);
            }
            else if (trigHit)
            {
                trigHit = false;
                StartAnimation(AnimationTypes.Hit);
            }
            else if (trigJumpAttack)
            {
                trigJumpAttack = false;
                StartAnimation(AnimationTypes.JumpAttack);
            }
            else if (trigBasicAttack1)
            {
                trigBasicAttack1 = false;
                StartAnimation(AnimationTypes.BasicAttack1);
            }
            else if (trigBasicAttack2)
            {
                trigBasicAttack2 = false;
                StartAnimation(AnimationTypes.BasicAttack2);
            }
            else if (trigBasicAttack3)
            {
                trigBasicAttack3 = false;
                StartAnimation(AnimationTypes.BasicAttack3);
            }
            else if (trigSkill1)
            {
                trigSkill1 = false;
                StartAnimation(AnimationTypes.Skill1);
            }
            else if (trigSkill2)
            {
                trigSkill2 = false;
                StartAnimation(AnimationTypes.Skill2);
            }
            else if (trigSkill3)
            {
                trigSkill3 = false;
                StartAnimation(AnimationTypes.Skill3);
            }
            else if (trigSkill4)
            {
                trigSkill4 = false;
                StartAnimation(AnimationTypes.Skill4);
            }
            else if (statBlock && m_Speed <= 0)
                StartAnimation(AnimationTypes.Block);
            else if (statBlock && m_Speed > 0.001)
                StartAnimation(AnimationTypes.BlockMove);
            else if (!m_Grounded && m_vSpeed <= 0)
                StartAnimation(AnimationTypes.Fall);
            else if (!m_Grounded && m_vSpeed >= 0 && trigJump)
            {
                StartAnimation(AnimationTypes.Jump);
                trigJump = false;
            }
            else if (trigDoubleJump)
            {
                StartAnimation(AnimationTypes.Jump, true);
                trigDoubleJump = false;
            }
            else if (m_Grounded && m_Speed > 0)
                StartAnimation(AnimationTypes.Run);
            else if (m_Grounded)
            {
                if (RevertIdleActualFrames >= RevertIdleNeededFrames)
                {
                    StartAnimation(AnimationTypes.Idle);
                    RevertIdleActualFrames = 0;
                }
                else
                {
                    RevertIdleActualFrames++;
                }
            }
        }
    }

    protected abstract bool AdditionalNotInterruptCondition(AnimationTypes activeAnimation);
    protected abstract bool AdditionalAnimationCondition();
}