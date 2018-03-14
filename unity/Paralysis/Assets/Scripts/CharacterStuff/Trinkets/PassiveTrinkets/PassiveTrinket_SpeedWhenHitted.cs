using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 5
/// Effect: Gain movementspeed when hitted with a low chance
/// </summary>
class PassiveTrinket_SpeedWhenHitted : PassiveTrinket
{
    // Use this for initialization
    void Awake()
    {
        DisplayName += "Worn Boots";
        PercentageEffectAdd = 0.2f;
        TrinketTriggerType = TriggerType.TakeDamage;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Set Effects of Use-Trinket 
        TrinketOwnerStats.PercentageMovement += PercentageEffectAdd;

        // Wait till duration ends
        yield return new WaitForSeconds(GeneralDuration);

        // Disable Effects of Use-Trinket
        TrinketOwnerStats.PercentageMovement -= PercentageEffectAdd;

        // Stop Trinket
        TrinketRunning = false;
    }
}