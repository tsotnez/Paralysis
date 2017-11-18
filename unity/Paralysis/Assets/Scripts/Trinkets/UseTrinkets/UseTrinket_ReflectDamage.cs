using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 8
/// Effect: Reflect damage for a short time
/// </summary>
public class UseTrinket_ReflectDamage : UseTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Reflect damage";

        GeneralDuration = 2;
        PercentageEffectAdd = 0f;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Set Effects of Use-Trinket 
        TrinketOwnerStats.reflect = true;

        // Wait till duration ends
        yield return new WaitForSeconds(GeneralDuration);

        // Disable Effects of Use-Trinket
        TrinketOwnerStats.reflect = false;

        // Stop Trinket
        TrinketRunning = false;
    }
}