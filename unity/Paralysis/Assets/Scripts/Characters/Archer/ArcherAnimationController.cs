public class ArcherAnimationController : ChampionAnimationController
{
    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }

    protected override bool AdditionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        return false;
    }
}