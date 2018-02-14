public class ArcherAnimationController : ChampionAnimationController
{
    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }

    protected override bool AdditionalNotInterruptCondition(AnimationTypes activeAnimation)
    {
        return false;
    }
}