using System.Collections.Generic;
using System;

/// <summary>
/// Holds information about each Hero and his skills in dictionaries named after the hero it contains information about.
/// </summary>
public static class ChampionAndTrinketDatabase {

    public enum Champions
    {
        Assassin, Archer, Knight, Infantry, Alchemist
    }

    public enum Keys
    {
        Skill1Name, Skill2Name, Skill3Name, Skill4Name, Skill1Text, Skill2Text, Skill3Text, Skill4Text, Lore, Name
    }

    //Set Values for each Dictionary (hardcoded)

    #region Assassin
    public static Dictionary<Keys, string> Assassin = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, "Paralysing Poison"},
        {Keys.Skill1Text, "Assassins blades are infused with a strong poison, stunning the enemy he hits."},

        {Keys.Skill2Name, "Vanish"},
        {Keys.Skill2Text, "Become invisible and next basic attack deals increased damage."},

        {Keys.Skill3Name, "Shadowstep"},
        {Keys.Skill3Text, "A quick teleport to the nearest target on same height allows Assassin to surprise his victim and stun it."},

        {Keys.Skill4Name, "Longfang"},
        {Keys.Skill4Text, "Assassin shoots his pistol dealing massive damage and knocking back the poor victim."},

        {Keys.Name, "Assassin, the Shadowwalker"},
        {Keys.Lore, "This unknown warrior wandered trough the wastelands for a long time after the world faced its doom. He learned to " +
            "survive on his own and salvaged strong tech on his journeys which allow him to perform a variety of skills."}
    };
    #endregion

    #region Archer
    public static Dictionary<Keys, string> Archer = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, "Eagles Talon"},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, "Sharks Teeth"},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, "Bears Strength"},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, "Falcons Speed"},
        {Keys.Skill4Text, ""},

        {Keys.Name, "Archer, the Huntress"},
        {Keys.Lore, ""}
    };
    #endregion

    #region Knight
    public static Dictionary<Keys, string> Knight = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, "Earthquake"},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, "Engage!"},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, "Shield Bash"},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, "Battering Ram"},
        {Keys.Skill4Text, ""},

        {Keys.Name, "Knight, the Fortress"},
        {Keys.Lore, ""}
    };
    #endregion

    #region Infantry
    public static Dictionary<Keys, string> Infantry = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, "Grappling Hook"},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, "Ground Break"},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, "Storm of Blades"},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, "Tornado"},
        {Keys.Skill4Text, ""},

        {Keys.Name, "Infantry, Killer for Coin"},
        {Keys.Lore, ""}
    };
    #endregion

    #region Alchemist
    public static Dictionary<Keys, string> Alchemist = new Dictionary<Keys, string>()
    {
        {Keys.Skill1Name, "Frostbolt"},
        {Keys.Skill1Text, ""},

        {Keys.Skill2Name, "Teleport"},
        {Keys.Skill2Text, ""},

        {Keys.Skill3Name, "Lightning Strikes"},
        {Keys.Skill3Text, ""},

        {Keys.Skill4Name, "Molecular Reconstruction"},
        {Keys.Skill4Text, ""},

        {Keys.Name, "Alchemist, the Solitair"},
        {Keys.Lore, ""}
    };
    #endregion

    #region database Dictionary
    /// <summary>
    /// Returns the dictionary which holds information about the passed champion
    /// </summary>
    public static Dictionary<Champions, Dictionary<Keys, string>> database = new Dictionary<Champions, Dictionary<Keys, string>>()
    {
        {Champions.Assassin, Assassin},
        {Champions.Archer, Archer},
        {Champions.Knight, Knight},
        {Champions.Infantry, Infantry},
        {Champions.Alchemist, Alchemist},
    };
    #endregion

    #region TrinketDescriptions
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
    #endregion

    #region TrinketNames
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
    #endregion

}
