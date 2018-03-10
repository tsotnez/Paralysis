using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Trinket : MonoBehaviour
{
    public string DisplayName;
    public int trinketNumber;

    public bool TrinketReady { get; protected set; }
    protected int GeneralCooldown;

    public static Dictionary<Trinkets, System.Type> trinketsForNames = new Dictionary<Trinkets, System.Type>()
    {
        {Trinkets.UseTrinket_GetImmun,                      typeof(UseTrinket_GetImmun)},
        {Trinkets.PassiveTrinket_ChanceDealingMoreDamage,   typeof(PassiveTrinket_ChanceDealingMoreDamage)},
        {Trinkets.PassiveTrinket_ChanceToStun,              typeof(PassiveTrinket_ChanceToStun)},
        {Trinkets.PassiveTrinket_SpeedWhenHitted,           typeof(PassiveTrinket_SpeedWhenHitted)},
        {Trinkets.UseTrinkets_NextHitDealsbleeding,         typeof(UseTrinkets_NextHitDealsbleeding)},
        {Trinkets.UseTrinket_HealingOverTime,               typeof(UseTrinket_HealingOverTime)},
        {Trinkets.UseTrinket_IncreaseDamage,                typeof(UseTrinket_IncreaseDamage)},
        {Trinkets.UseTrinket_IncreaseMovementSpeed,         typeof(UseTrinket_IncreaseMovementSpeed)},
        {Trinkets.UseTrinket_ReflectDamage,                 typeof(UseTrinket_ReflectDamage)},
        {Trinkets.UseTrinket_RemoveStatusEffect,            typeof(UseTrinket_RemoveStatusEffect)}
    };

    public enum Trinkets : byte
    {
        UseTrinket_GetImmun = 0,
        UseTrinket_HealingOverTime = 1,
        UseTrinket_IncreaseDamage = 2,
        UseTrinket_IncreaseMovementSpeed = 3,
        UseTrinket_ReflectDamage = 4,
        UseTrinket_RemoveStatusEffect = 5,
        UseTrinkets_NextHitDealsbleeding = 6,
        PassiveTrinket_ChanceDealingMoreDamage = 7,
        PassiveTrinket_ChanceToStun = 8,
        PassiveTrinket_SpeedWhenHitted = 9
    }

    public void resetValues()
    {
        TrinketReady = true;
        StopAllCoroutines();
    }
}