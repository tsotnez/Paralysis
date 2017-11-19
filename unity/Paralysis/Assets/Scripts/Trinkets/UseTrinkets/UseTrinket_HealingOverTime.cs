using System.Collections;
using UnityEngine;

/// <summary>
/// Position: 6
/// Effect: Heal hitpoints over time
/// </summary>
public class UseTrinket_HealingOverTime : UseTrinket
{
    // Use this for initialization
    void Start()
    {
        DisplayName += "Healing Over Time";

        GeneralDuration = 10;
        AmountEffect = 50;
    }

    protected override IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats)
    {
        for (int i = 0; i < GeneralDuration; i++)
        {
            // Heal
            TrinketOwnerStats.GetHealth(AmountEffect / (int)GeneralDuration);
            // Wait till next push
            yield return new WaitForSeconds(GeneralDuration / (float)AmountEffect);
        }

        // Stop Trinket
        TrinketRunning = false;
    }
}