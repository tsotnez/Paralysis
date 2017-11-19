using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 4
/// Effect: Deal percentual extra damage with a low chance
/// </summary>
class PassiveTrinket_ChanceDealingMoreDamage : PassiveTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Extra Damage";
        GeneralDuration = 3;
        PercentageEffectAdd = 0.2f;
        TrinketTriggerType = TriggerType.DealDamage;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Set Effects of Use-Trinket 
        TrinketOwnerStats.PercentageDamage += PercentageEffectAdd;

        // Wait till duration ends
        yield return new WaitForSeconds(GeneralDuration);

        // Disable Effects of Use-Trinket
        TrinketOwnerStats.PercentageDamage -= PercentageEffectAdd;

        // Stop Trinket
        TrinketRunning = false;
    }
}