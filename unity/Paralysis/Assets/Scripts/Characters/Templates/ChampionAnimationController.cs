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
        if (!statDead || CurrentAnimation != AnimatorStates.Die)
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
                StartAnimation(AnimatorStates.BasicAttack1);
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
            trigKnockedBack = false;
            StartAnimation(AnimatorStates.KnockedBack);
        }
        else if (statStunned && CurrentAnimation != AnimatorStates.KnockedBack)
            StartAnimation(AnimatorStates.Stunned);
        else
        {
            // don't interrupt these animations (equivalent to HasExitTime)
            if (AdditionalNotInterruptCondition(CurrentAnimation)) return;

            switch (CurrentAnimation)
            {
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
                case AnimatorStates.Dash:
                    if (trigDashEnd)
                    {
                        trigDashEnd = false;
                        StartAnimation(AnimatorStates.Dash, TypeOfAnimation.EndAnimation);
                    }
                    return;
                case AnimatorStates.DashFor:
                    if (trigDashForwardEnd)
                    {
                        trigDashForwardEnd = false;
                        StartAnimation(AnimatorStates.DashFor, TypeOfAnimation.EndAnimation);
                    }
                    return;
            }

            // For character specific animations
            if (AdditionalAnimationCondition()) return;

            if (trigDash)
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
            else if (statBlock && m_Speed <= 0)
                StartAnimation(AnimatorStates.Block);
            else if (statBlock && m_Speed > 0.001)
                StartAnimation(AnimatorStates.BlockMove);
            else if (!m_Grounded && m_vSpeed <= 0)
                StartAnimation(AnimatorStates.Fall);
            else if (!m_Grounded && m_vSpeed > 0 && trigJump)
            {
                StartAnimation(AnimatorStates.Jump);
                trigJump = false;
            }
            else if (m_Grounded && m_Speed > 0)
                StartAnimation(AnimatorStates.Run);
            else if (m_Grounded)
            {
                if (RevertIdleActualFrames >= RevertIdleNeededFrames)
                {
                    StartAnimation(AnimatorStates.Idle);
                    RevertIdleActualFrames = 0;
                }
                else
                {
                    RevertIdleActualFrames++;
                }
            }
        }
    }

    protected abstract bool AdditionalNotInterruptCondition(AnimatorStates activeAnimation);
    protected abstract bool AdditionalAnimationCondition();
}