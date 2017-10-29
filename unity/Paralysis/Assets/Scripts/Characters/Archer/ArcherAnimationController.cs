public class ArcherAnimationController : ChampionAnimationController
{
    protected override bool additionalAnimationCondition()
    {
        return false;
    }

    protected override bool additionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        return false;
    }
}