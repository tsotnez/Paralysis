public class InfantryAnimationController : ChampionAnimationController
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
