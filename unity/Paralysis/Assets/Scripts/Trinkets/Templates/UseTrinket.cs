using System.Collections;
using UnityEngine;

public abstract class UseTrinket : Trinket
{
    public bool TrinketRunning { get; protected set; }

    protected float GeneralDuration;
    protected float PercentageEffectAdd;
    protected int AmountEffect;

    protected virtual void Start()
    {
        GeneralCooldown = 60;

        TrinketRunning = false;
        TrinketReady = true;
    }

    /// <summary>
    /// Use the Trinket
    /// Starts the effect of the Trinket
    /// </summary>
    /// <returns>Trinket has started or not</returns>
    public bool Use(CharacterStats TrinketOwnerStats)
    {
        if (!TrinketRunning && TrinketReady)
        {
            // Set trinket is active
            TrinketRunning = true;
            // Show the player that the trinket is active
            TrinketOwnerStats.ShowFloatingText_TrinketTriggered(DisplayName);
            // Start trinket action
            StartCoroutine(ManageTrinketDuration(TrinketOwnerStats));
            // Start Cooldown
            StartCoroutine(ManageCooldown());
            return true;
        }
        return false;
    }

    protected abstract IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats);

    private IEnumerator ManageCooldown()
    {
        ChampionClassController controller = GetComponent<ChampionClassController>();
        CharacterStats stats = GetComponent<CharacterStats>();

        stats.StartTrinketEffects();
        controller.hotbar.greyOutTrinket(trinketNumber);
        yield return new WaitUntil(() => !TrinketRunning);
        stats.StopTrinketEffects();
        controller.hotbar.setTrinketOnCooldown(trinketNumber, GeneralCooldown);
        TrinketReady = false;
        yield return new WaitForSeconds(GeneralCooldown);
        TrinketReady = true;
    }
}
