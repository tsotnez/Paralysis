using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 9
/// Effect: Next hit deals bleeding effect
/// </summary>
public class UseTrinkets_NextHitDealsbleeding : UseTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Next hit deals bleeding effect";

        GeneralDuration = 10;
        PercentageEffectAdd = 0f;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Set Effects of Use-Trinket 
        TrinketOwnerStats.NextHitDealsBleed = true;

        // Wait
        yield return new WaitForSeconds(0.1f);

        // Wait till effect is dealed
        yield return new WaitUntil(() => !TrinketOwnerStats.NextHitDealsBleed);

        // End trinket effect
        TrinketRunning = false;
    }
}
