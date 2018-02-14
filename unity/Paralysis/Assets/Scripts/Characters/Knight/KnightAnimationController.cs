public class KnightAnimationController : ChampionAnimationController
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
                    StartAnimation(AnimationTypes.Skill2, AnimationKind.EndAnimation);
                }
                return true;
        }

        return false;
    }
}
