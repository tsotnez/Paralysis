using System;
using System.Collections;

/// <summary>
/// Position: 4
/// Effect: Deal percentual extra damage with a low chance
/// </summary>
class PassiveTrinket_ChanceDealingMoreDamage : PassiveTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Chance to deal extra damage";
        PercentageEffectAdd = 0.2f;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        throw new NotImplementedException();
    }
}