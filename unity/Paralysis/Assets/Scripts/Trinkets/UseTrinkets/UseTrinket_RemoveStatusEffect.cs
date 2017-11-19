using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 1
/// Effect: Remove negative status effects
/// </summary>
public class UseTrinket_RemoveStatusEffect : UseTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Free off Effects";
        GeneralDuration = 0.25f;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        // Set Effects of Use-Trinket 
        TrinketOwnerStats.stunned = false;
        TrinketOwnerStats.bleeding = false;
        TrinketOwnerStats.invincible = true;

        // Wait till duration ends
        yield return new WaitForSeconds(GeneralDuration);

        // Disable Effects of Use-Trinket
        TrinketOwnerStats.invincible = false;

        // Stop Trinket
        TrinketRunning = false;
    }
}