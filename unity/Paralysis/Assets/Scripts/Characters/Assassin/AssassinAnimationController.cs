class AssassinAnimationController : ChampionAnimationController
{ 

    protected override bool AdditionalNotInterruptCondition(AnimationTypes activeAnimation)
    {
        return false;
    }

    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }
}