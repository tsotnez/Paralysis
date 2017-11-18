using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Trinket : MonoBehaviour
{
    public static string DisplayName;
    public static Dictionary<Trinkets, System.Type> trinketsForNames = new Dictionary<Trinkets, System.Type>()
    {
        {Trinkets.UseTrinket_GetImmun, typeof(UseTrinket_GetImmun)},
        {Trinkets.PassiveTrinket_ChanceDealingMoreDamage,typeof(PassiveTrinket_ChanceDealingMoreDamage)},
        {Trinkets.PassiveTrinket_ChanceToStun, typeof(PassiveTrinket_ChanceToStun)},
        {Trinkets.PassiveTrinket_SpeedWhenHitted, typeof(PassiveTrinket_SpeedWhenHitted)},
        {Trinkets.UseTrinkets_NextHitDealsbleeding, typeof(UseTrinkets_NextHitDealsbleeding)},
        {Trinkets.UseTrinket_HealingOverTime, typeof(UseTrinket_HealingOverTime)},
        {Trinkets.UseTrinket_IncreaseDamage, typeof(UseTrinket_IncreaseDamage)},
        {Trinkets.UseTrinket_IncreaseMovementSpeed, typeof(UseTrinket_IncreaseMovementSpeed)},
        {Trinkets.UseTrinket_ReflectDamage, typeof(UseTrinket_ReflectDamage)},
        {Trinkets.UseTrinket_RemoveStatusEffect, typeof(UseTrinket_RemoveStatusEffect)}
    };

    

    public enum Trinkets
    {
        UseTrinket_GetImmun,
        UseTrinket_HealingOverTime,
        UseTrinket_IncreaseDamage,
        UseTrinket_IncreaseMovementSpeed,
        UseTrinket_ReflectDamage,
        UseTrinket_RemoveStatusEffect,
        UseTrinkets_NextHitDealsbleeding,
        PassiveTrinket_ChanceDealingMoreDamage,
        PassiveTrinket_ChanceToStun,
        PassiveTrinket_SpeedWhenHitted
    }
}