public class KnightAnimationController : ChampionAnimationController
{
    // Trigger for character specific animations
    public bool trigDashForward = false;
    public bool trigSkill2End = false;

    protected override bool additionalAnimationCondition()
    {
        if (trigSkill2End)
        {
            trigSkill2End = false;
            StartEndAnimation();
        }
        if (statBlock && m_Speed > 0.001)
            StartAnimation(AnimatorStates.BlockMove);
        else if (trigDashForward)
            StartAnimation(AnimatorStates.DashFor);
        else
            return false;

        return true;
    }

    protected override bool additionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        if (activeAnimation == AnimatorStates.DashFor)
            return true;

        return false;
    }
}
