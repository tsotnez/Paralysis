using System;
using System.Collections;
using UnityEngine;

public class KnightController : ChampionClassController
{
    [SerializeField]
    private GameObject Skill4_Spear;
    [SerializeField]
    private float skill2_leap_speed = 10f;

    private bool skill2_leap = false;

    #region default Methods

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<KnightAnimationController>();

        //Instantiate skill variables
        basicAttack1_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.BasicAttack1, delay_BasicAttack1, damage_BasicAttack1, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack1, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack1, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        basicAttack2_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.BasicAttack2, delay_BasicAttack2, damage_BasicAttack2, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack2, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack2, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        basicAttack3_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.BasicAttack3, delay_BasicAttack3, damage_BasicAttack3, Skill.SkillEffect.nothing, 0, 0, stamina_BasicAttack3, Skill.SkillTarget.SingleTarget, cooldown_BasicAttack3, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);

        skill1_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.Skill1, delay_Skill1, damage_Skill1, Skill.SkillEffect.stun, 3, 0, stamina_Skill1, Skill.SkillTarget.MultiTarget, cooldown_Skill1, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        skill2_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.Skill2, delay_Skill2, damage_Skill2, Skill.SkillEffect.stun, 3, 0, stamina_Skill2, Skill.SkillTarget.MultiTarget, cooldown_Skill2, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        skill3_var = new MeleeSkill(ChampionAndTrinketDatabase.Keys.Skill3, delay_Skill3, damage_Skill3, Skill.SkillEffect.knockback, 0, 0, stamina_Skill3, Skill.SkillTarget.SingleTarget, cooldown_Skill3, MeeleRange, ChampionAndTrinketDatabase.Champions.Alchemist);
        skill4_var = new RangedSkill(ChampionAndTrinketDatabase.Keys.Skill4, false, new Vector2(7, 0), Skill4_Spear, delay_Skill4, damage_Skill4, Skill.SkillEffect.nothing, 0, 0, stamina_Skill4, Skill.SkillTarget.SingleTarget, cooldown_Skill4, 5f, ChampionAndTrinketDatabase.Champions.Alchemist);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Add force to knight when leaping (Skill 2)
        if (skill2_leap)
        {
            m_Rigidbody2D.velocity = new Vector2(skill2_leap_speed, m_Rigidbody2D.velocity.y);
        }
    }

    #endregion

    #region BasicAttack and Skills

    /// <summary>
    /// Attack combo. 2 normal hits, 1 strong hit
    /// 
    /// normal hit:  5 dmg
    /// strong hit: 10 dmg
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void BasicAttack()
    {
        DoComboBasicAttack(ref animCon.trigBasicAttack1, ref animCon.trigBasicAttack1, ref animCon.trigBasicAttack2);
    }

    /// <summary>
    /// Ground Smash (Stun)
    /// The knight smashes his shield into the ground.
    /// Stunning any enemy in a short radius
    /// 
    /// Damage: 5
    /// Effect: Stun
    /// Cooldown: 15 sec
    /// Stamina: 20
    /// </summary>
    public override void Skill1()
    {
        DoMeleeSkill(ref animCon.trigSkill1, (MeleeSkill)skill1_var);
    }


    /// <summary>
    /// Leap (Stun)
    /// The knight leaps at a fixed altitude of the map.
    /// Once he lands he stuns any enemies in a short radius.
    /// 
    /// Damage: 5
    /// Effect: Stun
    /// Cooldown: 20 sec
    /// Stamina: 15
    /// </summary>
    public override void Skill2()
    {
        // Validate if skill can be performed
        if (CanPerformAction(true) && CanPerformAttack() && skill2_var.notOnCooldown && stats.HasSufficientStamina(stamina_Skill2))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill2_var.type));
            // set animation
            animCon.trigSkill2 = true;

            // start coroutine for hitting when reaching ground
            StartCoroutine(Skill2_Coroutine());
        }
    }

    private IEnumerator Skill2_Coroutine()
    {
        stats.immovable = true;
        skill2_leap = true;

        // Calculate correct direction
        if (!FacingRight)
            skill2_leap_speed = Math.Abs(skill2_leap_speed) * (-1);
        else
            skill2_leap_speed = Math.Abs(skill2_leap_speed);

        // Add a vertical force to the player.
        m_Rigidbody2D.velocity = Vector2.zero;
        m_Rigidbody2D.AddForce(new Vector2(0, 400));

        // Wait until force is applied
        yield return new WaitUntil(() => !animCon.m_Grounded);
        // Wait while not on ground
        yield return new WaitUntil(() => animCon.m_Grounded);
        Camera.main.GetComponent<CameraBehaviour>().startShake(); //Shake the camera
        skill2_leap = false;
        stats.immovable = false;

        DoMeleeSkill(ref ((KnightAnimationController)animCon).trigSkill2End, (MeleeSkill)skill2_var, true);
    }

    /// <summary>
    /// Shield Bash (Knockback)
    /// The knight takes a few swift steps to deliver a strike with his shield
    /// Any enemy struck by it will be put in a knockback animation.
    /// 
    /// Damage: 10
    /// Effect: Knockback
    /// Cooldown: 20 sec
    /// Stamina: 30
    /// </summary>
    public override void Skill3()
    {
        DoMeleeSkill(ref animCon.trigSkill3, (MeleeSkill)skill3_var);
    }

    /// <summary>
    /// Spear (Damage)
    /// The knight throws a spear at a fixed distance and a fixed altitude.
    /// Any enemy struck by it will receive damage.
    /// 
    /// Damage: 15
    /// Cooldown: 25 sec
    /// Stamina: 20
    /// </summary>
    public override void Skill4()
    {
        DoRangeSkill(ref animCon.trigSkill4, (RangedSkill)skill4_var);
    }

    #endregion

}