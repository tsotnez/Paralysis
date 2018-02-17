public class AlchemistAnimationController : ChampionAnimationController
{
    // Trigger for character specific animations
    public bool trigSkill2End = false;

    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }

    protected override bool AdditionalNotInterruptCondition(AnimationTypes activeAnimation)
    {
        switch (CurrentAnimation)
        {
            case AnimationTypes.Skill2:
                if (trigSkill2End)
                {
                    trigSkill2End = false;
                    StartEndAnimation(AnimationTypes.Skill2);
                }
                return true;
        }
        return false;
    }
}