using System.Collections;
using UnityEngine;

public abstract class PassiveTrinket : Trinket
{
    public enum TriggerType
    {
        DealDamage, TakeDamage
    }

    public TriggerType TrinketTriggerType { get; protected set; }
    public bool TrinketRunning { get; protected set; }
    public bool TrinketReady { get; private set; }

    protected int GeneralCooldown;
    protected int GeneralDuration;
    protected float PercentageEffectAdd;

    private int TriggerChance;
    private System.Random random;

    protected virtual void Start()
    {
        random = new System.Random();

        GeneralCooldown = 10;
        TriggerChance = 5;
        TrinketReady = true;
    }

    /// <summary>
    /// Use the Trinket
    /// Starts the effect of the Trinket
    /// </summary>
    /// <returns>Trinket has started or not</returns>
    public bool Use(CharacterStats TrinketOwnerStats)
    {
        if (TrinketReady && !TrinketRunning)
        {
            int rand = random.Next(1, 100);
            if (rand-TriggerChance >= 95)
            {
                // Set trinket is active
                TrinketRunning = true;
                // Start trinket action
                StartCoroutine(ManageTrinketDuration(TrinketOwnerStats));
                // Start Cooldown
                StartCoroutine(manageCooldown());
                return true;
            }
        }

        return false;
    }

    protected abstract IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats);

    private IEnumerator manageCooldown()
    {
        GetComponent<ChampionClassController>().hotbar.greyOutTrinket(trinketNumber); // grey out for time its active
        yield return new WaitUntil(() => !TrinketRunning);
        GetComponent<ChampionClassController>().hotbar.setTrinketOnCooldown(trinketNumber, GeneralCooldown); // Set actual visual cooldown
        TrinketReady = false;
        yield return new WaitForSeconds(GeneralCooldown);
        TrinketReady = true;
    }
}