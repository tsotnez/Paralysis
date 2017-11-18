using UnityEngine;

[System.Serializable]
public abstract class Trinket : MonoBehaviour
{
    public string DisplayName;

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