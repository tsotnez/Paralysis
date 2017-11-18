using System.Collections;
using UnityEngine;

public abstract class PassiveTrinket : Trinket
{
    public bool TrinketReady { get; private set; }

    protected int GeneralCooldown; 
    protected float PercentageEffectAdd;

    private int TriggerChance;
    private System.Random random;
    

    protected virtual void Awake()
    {
        DisplayName = "(PASSIVE) ";
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
    public void Use(CharacterStats TrinketOwnerStats)
    {
        if (TrinketReady)
        {
            int rand = random.Next(1, 100);
            if (rand-TriggerChance >= 95)
            {
                // Start trinket action
                StartCoroutine(ManageTrinketDuration(TrinketOwnerStats));
                // Start Cooldown
                StartCoroutine(manageCooldown());
            }
        }
    }

    protected abstract IEnumerator ManageTrinketDuration(CharacterStats TrinketOwnerStats);

    private IEnumerator manageCooldown()
    {
        TrinketReady = false;
        yield return new WaitForSeconds(GeneralCooldown);
        TrinketReady = true;
    }
}