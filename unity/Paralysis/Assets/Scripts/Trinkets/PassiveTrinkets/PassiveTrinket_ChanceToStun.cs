using System;
using System.Collections;

/// <summary>
/// Position: 10
/// Effect: Stun the enemy while hitting with a low chance
/// </summary>
class PassiveTrinket_ChanceToStun : PassiveTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Chance to deal extra damage";
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        throw new NotImplementedException();
    }
}