using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 2
/// Effect: Increaded movementspeed for a time
/// </summary>
public class UseTrinket_IncreaseMovementSpeed : UseTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Movementspeed Increased";

        GeneralDuration = 5;
        PercentageEffectAdd = 0.2f;
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