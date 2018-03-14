/// <summary>
/// Describes a skill
/// </summary>
public class Skill
{
    public enum SkillEffect
    {
        nothing,
        stun,
        knockback,
        bleed,
        slow
    }

    public enum SkillTarget
    {
        SingleTarget, MultiTarget, InFront
    }

    public ChampionAndTrinketDatabase.Keys type;
    public ChampionAndTrinketDatabase.Champions champion;
    public string name;
    public SkillEffect effect;
    public int effectDuration;
    public float effectValue;
    public int damage;
    public int staminaCost;
    public float cooldown;
    public SkillTarget targetType;
    public float range;
    public bool needsToBeGrounded;
    public float delay;

    public bool notOnCooldown = true;


    public Skill(ChampionAndTrinketDatabase.Keys skillType, float skillDelay, int skillDamage, Skill.SkillEffect skillSpecialEffect, int skillSpecialEffectTime, float skillSpecialEffectValue, int skillStaminaCost, SkillTarget skillTargetType,
        float skillCooldown, float skillRange, ChampionAndTrinketDatabase.Champions skillChamp, bool skillNeedsToBeGrounded = true)
    {
        champion = skillChamp;
        type = skillType;
        delay = skillDelay;
        damage = skillDamage;
        effect = skillSpecialEffect;
        effectDuration = skillSpecialEffectTime;
        effectValue = skillSpecialEffectValue;
        cooldown = skillCooldown;
        staminaCost = skillStaminaCost;
        needsToBeGrounded = skillNeedsToBeGrounded;
        range = skillRange;
        targetType = skillTargetType;
        name = ChampionAndTrinketDatabase.database[champion][type];
    }

    public Skill(ChampionAndTrinketDatabase.Keys skillType, ChampionAndTrinketDatabase.Champions skillChampion, float skillCooldown)
    {
        champion = skillChampion;
        cooldown = skillCooldown;
        type = skillType;
    }
}