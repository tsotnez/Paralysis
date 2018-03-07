class AssassinAnimationController : ChampionAnimationController
{ 
    // trigger for animation
    public bool trigDoubleJump = false;

    protected override bool AdditionalNotInterruptCondition(AnimationTypes activeAnimation)
    {
        return false;
    }

    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }
}