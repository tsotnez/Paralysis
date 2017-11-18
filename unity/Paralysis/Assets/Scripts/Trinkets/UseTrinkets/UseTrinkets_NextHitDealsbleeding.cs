using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 9
/// Effect: Next hit in 10 seconds deals bleeding effect
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
        throw new NotImplementedException();

        // Set Effects of Use-Trinket 

        // Wait till duration ends
        yield return new WaitForSeconds(GeneralDuration);

        // Disable Effects of Use-Trinket

        // Stop Trinket
        TrinketRunning = false;
    }
}
