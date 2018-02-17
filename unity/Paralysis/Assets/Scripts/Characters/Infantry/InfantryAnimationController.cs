public class InfantryAnimationController : ChampionAnimationController
{
// Trigger for character specific animations
    public bool trigSkill1End = false;

    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }

    protected override bool AdditionalNotInterruptCondition(AnimationTypes activeAnimation)
    {
        switch (CurrentAnimation)
        {
            case AnimationTypes.Skill1:
                if (trigSkill1End)
                {
                    trigSkill1End = false;
                    StartEndAnimation(AnimationTypes.Skill1);
                }
                return true;
        }

        return false;
    }
}
