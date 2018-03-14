using System.Collections.Generic;
using System;

/// <summary>
/// Holds information about each Hero and his skills in dictionaries named after the hero it contains information about.
/// </summary>
public static class ChampionAndTrinketDatabase
{
    public enum Champions
    {
        Assassin, Archer, Knight, Infantry, Alchemist
    }

    public enum Keys
    {
        Skill1, Skill2, Skill3, Skill4, Skill1Text, Skill2Text, Skill3Text, Skill4Text, Lore, Name, JumpAttack, BasicAttack1, BasicAttack2, BasicAttack3
    }

    //Set Values for each Dictionary (hardcoded)

    #region Assassin

    public static Dictionary<Keys, string> Assassin = new Dictionary<Keys, string>()
    {
        {Keys.Skill1, "Paralysing Poison"},
        {Keys.Skill1Text, "Assassins blades are infused with a strong poison, stunning the enemy he hits."},

        {Keys.Skill2, "Vanish"},
        {Keys.Skill2Text, "Become invisible and next basic attack deals increased damage."},

        {Keys.Skill3, "Shadowstep"},
        {Keys.Skill3Text, "A quick teleport to the nearest target on same height allows Assassin to surprise his victim and stun it."},

        {Keys.Skill4, "Longfang"},
        {Keys.Skill4Text, "Assassin shoots his pistol dealing massive damage and knocking back the poor victim."},

        {Keys.Name, "Assassin, the Shadowwalker"},
        {Keys.Lore, "This unknown warrior wandered through the wastelands for a long time after the world faced its doom. He learned to " +
            "survive on his own and salvaged strong tech on his journeys which allow him to perform a variety of skills."},

        {Keys.BasicAttack1, "Basic Attack 1"},
        {Keys.BasicAttack2, "Basic Attack 2"},

        {Keys.BasicAttack3, "Basic Attack 3"},
        {Keys.JumpAttack, "Jump Attack"}
    };

    #endregion

    #region Archer

    public static Dictionary<Keys, string> Archer = new Dictionary<Keys, string>()
    {
        {Keys.Skill1, "Eagles Talon"},
        {Keys.Skill1Text, "Archer charges her bow further, creating a strong force that will cause knockback to any first enemy it comes in contact with."},

        {Keys.Skill2, "Sharks Teeth"},
        {Keys.Skill2Text, "Archer puts down a trap. Anyone that walks on it will receive explosive damage and stuns the enemy."},

        {Keys.Skill3, "Bears Strength"},
        {Keys.Skill3Text, "Archer hits the ground with her feet, stunning any enemy in a short radius."},

        {Keys.Skill4, "Falcons Speed"},
        {Keys.Skill4Text, "Archer jumps back to gain distance. Archer gains invisibility in that period."},

        {Keys.Name, "Archer, the Huntress"},
        {Keys.Lore, ""},

        {Keys.BasicAttack1, "Basic Attack 1"},
        {Keys.BasicAttack2, "Basic Attack 2"},

        {Keys.BasicAttack3, "Basic Attack 3"},
        {Keys.JumpAttack, "Jump Attack"}

    };

    #endregion

    #region Knight

    public static Dictionary<Keys, string> Knight = new Dictionary<Keys, string>()
    {
        {Keys.Skill1, "Earthquake"},
        {Keys.Skill1Text, "The knight smashes his shield into the ground, stunning any enemy in a short radius."},

        {Keys.Skill2, "Engage!"},
        {Keys.Skill2Text, "The knight leaps at a fixed altitude of the map, once he lands he stuns any enemies in a short radius."},

        {Keys.Skill3, "Shield Bash"},
        {Keys.Skill3Text, "The knight takes a few swift steps to deliver a strike with his shield, any enemy struck by it will be put in a knockback animation."},

        {Keys.Skill4, "Battering Ram"},
        {Keys.Skill4Text, "The knight throws a spear at a fixed distance and a fixed altitude, any enemy struck by it will receive damage."},

        {Keys.Name, "Knight, the Fortress"},
        {Keys.Lore, ""},

        {Keys.BasicAttack1, "Basic Attack 1"},
        {Keys.BasicAttack2, "Basic Attack 2"},

        {Keys.BasicAttack3, "Basic Attack 3"},
        {Keys.JumpAttack, "Jump Attack"}
    };

    #endregion

    #region Infantry

    public static Dictionary<Keys, string> Infantry = new Dictionary<Keys, string>()
    {
        {Keys.Skill1, "Grappling Hook"},
        {Keys.Skill1Text, "Infantry launches a hook to a flexibly fixed distance and altitude the hook latches on to the first enemy it comes in contact with. The Infantry then repels to that target and kicks, causing a knockback effect to the enemy"},

        {Keys.Skill2, "Ground Break"},
        {Keys.Skill2Text, "The Infantry strikes the ground with his sword producing a force that can stun any enemys directly in front of him."},

        {Keys.Skill3, "Storm of Blades"},
        {Keys.Skill3Text, "Infantry swings his sword in all directions. Any enemy directly in front or behind the Infantry will take extensive damage."},

        {Keys.Skill4, "Tornado"},
        {Keys.Skill4Text, "Infantry swings his sword upwards, along with any enemy directly in front of him. This causes a knockback to the enemy."},

        {Keys.Name, "Infantry, Killer for Coin"},
        {Keys.Lore, ""},

        {Keys.BasicAttack1, "Basic Attack 1"},
        {Keys.BasicAttack2, "Basic Attack 2"},

        {Keys.BasicAttack3, "Basic Attack 3"},
        {Keys.JumpAttack, "Jump Attack"},
    };

    #endregion

    #region Alchemist

    public static Dictionary<Keys, string> Alchemist = new Dictionary<Keys, string>()
    {
        {Keys.Skill1, "Frostbolt"},
        {Keys.Skill1Text, "Alchemist casts a frost bolt at a fixed distance and altitute. The first enemy hit by this effect will have a 50% decreased movement speed."},

        {Keys.Skill2, "Teleport"},
        {Keys.Skill2Text, "Alchemist teleports in the direction that he is moving or dashing. "+
                            "If standing still, teleports in the direction he is facing. "+
                            "If jumping, teleports on top platform. "+
                            "If ducking, teleports on lower platform."},

        {Keys.Skill3, "Melted Stone"},
        {Keys.Skill3Text, "Alchemist casts a spell at a fixed distance and altitude. The first enemy stuck by this will be stunned."},

        {Keys.Skill4, "Molecular Reconstruction"},
        {Keys.Skill4Text, "Alchemist uses his alchemy to explode and reconstruct himnself. As he explodes, any enemy within the radius will receive a knockback."},

        {Keys.Name, "Alchemist, the Solitair"},
        {Keys.Lore, ""},

        {Keys.BasicAttack1, "Basic Attack 1"},
        {Keys.BasicAttack2, "Basic Attack 2"},

        {Keys.BasicAttack3, "Basic Attack 3"},
        {Keys.JumpAttack, "Jump Attack"},
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
