using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 3
/// Effect: Deal percentual more damage for a time
/// </summary>
public class UseTrinket_IncreaseDamage : UseTrinket
{
    // Use this for initialization
    void Awake()
    {
        DisplayName += "Iron Dagger";

        GeneralDuration = 5;
        PercentageEffectAdd = 0.2f;
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