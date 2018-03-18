using System;
using System.Collections;
using UnityEngine;

public class KnightController : ChampionClassController
{
    [SerializeField]
    private float skill2_leap_speed = 10f;

    private bool skill2_leap = false;

    #region default Methods

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animCon = graphics.GetComponent<KnightAnimationController>();
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
        DoMeleeSkill(ref animCon.trigSkill1, Skill.SkillType.Skill1);
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
        MeleeSkill skill2 = GetMeleeSkillByType(Skill.SkillType.Skill2);
        if (CanPerformAction(true) && CanPerformAttack() && skill2.notOnCooldown && stats.HasSufficientStamina(skill2.staminaCost))
        {
            hotbar.StartCoroutine(hotbar.flashBlack(skill2.type));
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
        yield return new WaitUntil(() => !animCon.propGrounded);
        // Wait while not on ground
        yield return new WaitUntil(() => animCon.propGrounded);
        Camera.main.GetComponent<CameraBehaviour>().startShake(); //Shake the camera
        skill2_leap = false;
        stats.immovable = false;

        DoMeleeSkill(ref ((KnightAnimationController)animCon).trigSkill2End, Skill.SkillType.Skill2, true);
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
        DoMeleeSkill(ref animCon.trigSkill3, Skill.SkillType.Skill3);
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
        DoRangeSkill(ref animCon.trigSkill4, Skill.SkillType.Skill4);
    }

    #endregion

}