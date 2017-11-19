public class KnightAnimationController : ChampionAnimationController
{
    // Trigger for character specific animations
    public bool trigSkill2End = false;

    protected override bool additionalAnimationCondition()
    {
        if (statBlock && m_Speed > 0.001)
            StartAnimation(AnimatorStates.BlockMove);
        else
            return false;

        return true;
    }

    protected override bool additionalNotInterruptCondition(AnimatorStates activeAnimation)
    {
        switch (currentAnimation)
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
