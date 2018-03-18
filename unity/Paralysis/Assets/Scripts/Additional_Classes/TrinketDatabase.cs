using System.Collections.Generic;
using System;

/// <summary>
/// Holds information about the trinkets their descriptions.
/// </summary>
public static class TrinketDatabase
{
    public static Dictionary<Trinket.Trinkets, string> TrinketDescriptions = new Dictionary<Trinket.Trinkets, string>()
    {
        {Trinket.Trinkets.PassiveTrinket_ChanceDealingMoreDamage, "Upon inflicting damage on a foe, you will have a chance to increase the damage dealt."},
        {Trinket.Trinkets.PassiveTrinket_ChanceToStun, "When receiving damage, there is a chance to stun the enemy which dealt it."},
        {Trinket.Trinkets.PassiveTrinket_SpeedWhenHitted, "When receiving damagge, you'll have a chance to get a short boost in movement speed."},
        {Trinket.Trinkets.UseTrinkets_NextHitDealsbleeding, "You next hit will cause bleeding."},
        {Trinket.Trinkets.UseTrinket_GetImmun, "Makes your champion immune to all damage for a short period of time."},
        {Trinket.Trinkets.UseTrinket_HealingOverTime, "Heals 25% of your health over a short time."},
        {Trinket.Trinkets.UseTrinket_IncreaseDamage, "Increases all damage dealt for a short time."},
        {Trinket.Trinkets.UseTrinket_IncreaseMovementSpeed, "You can run faster for a moment."},
        {Trinket.Trinkets.UseTrinket_ReflectDamage, "Reflects all incoming damage for a preiod of time."},
        {Trinket.Trinkets.UseTrinket_RemoveStatusEffect, "On use, removes all status effects applied to your character."},
    };

    public static Dictionary<Trinket.Trinkets, string> TrinketNames = new Dictionary<Trinket.Trinkets, string>()
    {
        {Trinket.Trinkets.PassiveTrinket_ChanceDealingMoreDamage, "Bronze Dagger"},
        {Trinket.Trinkets.PassiveTrinket_ChanceToStun, "Vengeful Puppet."},
        {Trinket.Trinkets.PassiveTrinket_SpeedWhenHitted, "Worn Boots"},
        {Trinket.Trinkets.UseTrinkets_NextHitDealsbleeding, "Bloodstained Cloth"},
        {Trinket.Trinkets.UseTrinket_GetImmun, "Red Gem Shards"},
        {Trinket.Trinkets.UseTrinket_HealingOverTime, "Green Slime"},
        {Trinket.Trinkets.UseTrinket_IncreaseDamage, "Iron Dagger"},
        {Trinket.Trinkets.UseTrinket_IncreaseMovementSpeed, "Hermes Boots"},
        {Trinket.Trinkets.UseTrinket_ReflectDamage, "Strange Mirror"},
        {Trinket.Trinkets.UseTrinket_RemoveStatusEffect, "Ghost Chain"},
    };
}
