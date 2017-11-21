public class InfantryAnimationController : ChampionAnimationController
{
// Trigger for character specific animations
    public bool trigSkill1End = false;

    protected override bool AdditionalAnimationCondition()
    {
        return false;
    }

    protected override bool AdditionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        switch (CurrentAnimation)
        {
            case AnimatorStates.Skill1:
                if (trigSkill1End)
                {
                    trigSkill1End = false;
                    StartAnimation(AnimatorStates.Skill1, TypeOfAnimation.EndAnimation);
                }
                return true;
        }

        return false;
    }
}
