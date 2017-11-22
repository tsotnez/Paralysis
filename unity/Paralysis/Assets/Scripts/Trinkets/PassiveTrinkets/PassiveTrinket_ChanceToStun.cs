using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 10
/// Effect: Stun the enemy while hitting with a low chance
/// </summary>
class PassiveTrinket_ChanceToStun : PassiveTrinket
{
    // Use this for initialization
    void Awake()
    {
        DisplayName += "Vengeful Puppet";
        TrinketTriggerType = TriggerType.DealDamage;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Add Effect
        TrinketOwnerStats.NextHitDealsStun = false;

        // Wait till effect is dealed
        yield return new WaitUntil(() => !TrinketOwnerStats.NextHitDealsStun);

        // End Trinket Effect
        TrinketRunning = false;
    }
}