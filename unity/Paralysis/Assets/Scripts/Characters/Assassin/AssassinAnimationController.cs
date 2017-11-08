class AssassinAnimationController : ChampionAnimationController
{ 
    // trigger for animation
    public bool trigDoubleJump = false;

    protected override bool additionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        return false;
    }

    protected override bool additionalAnimationCondition()
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