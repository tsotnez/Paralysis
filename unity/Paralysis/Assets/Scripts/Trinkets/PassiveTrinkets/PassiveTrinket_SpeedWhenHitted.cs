using System;
using System.Collections;

/// <summary>
/// Position: 5
/// Effect: Gain movementspeed when hitted with a low chance
/// </summary>
class PassiveTrinket_SpeedWhenHitted : PassiveTrinket
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