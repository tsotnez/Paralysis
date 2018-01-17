public class AlchemistAnimationController : ChampionAnimationController
{
    // Trigger for character specific animations
    public bool trigSkill2End = false;

    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }

    protected override bool AdditionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        switch (CurrentAnimation)
        {
            case AnimatorStates.Skill2:
                if (trigSkill2End)
                {
                    trigSkill2End = false;
                    StartAnimation(AnimatorStates.Skill2, TypeOfAnimation.EndAnimation);
                }
                return true;
        }
        return false;
    }
}