using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 7
/// Effect: Get invincible for a short time
/// </summary>
public class UseTrinket_GetImmun : UseTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Invincibility";

        GeneralDuration = 2;
        PercentageEffectAdd = 0f;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Set Effects of Use-Trinket 
        TrinketOwnerStats.invincible = true;

        // Wait till duration ends
        yield return new WaitForSeconds(GeneralDuration);

        // Disable Effects of Use-Trinket
        TrinketOwnerStats.invincible = false;

        // Stop Trinket
        TrinketRunning = false;
    }
}