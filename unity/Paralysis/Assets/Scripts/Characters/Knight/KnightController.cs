using UnityEngine;

public class KnightController : ChampionClassController
{
    Coroutine attackingRoutine;

    [Header("Attack Delays")]
    [SerializeField]
    private float delay_BasicAttack1 = 0;
    [SerializeField]
    private float delay_BasicAttack2 = 0;
    [SerializeField]
    private float delay_BasicAttack3 = 0;
    [SerializeField]
    private float delay_Skill1 = 0;
    [SerializeField]
    private float delay_Skill2 = 0;
    [SerializeField]
    private float delay_Skill3 = 0;
    [SerializeField]
    private float delay_Skill4 = 0;

    [Header("Attack Stamina Costs")]
    [SerializeField]
    private int stamina_BasicAttack = 5;
    [SerializeField]
    private int stamina_BasicAttackCombo = 10;
    [SerializeField]
    private int stamina_JumpAttack = 5;
    [SerializeField]
    private int stamina_Skill1_GroundSmash = 20;
    [SerializeField]
    private int stamina_Skill2_Leap = 15;
    [SerializeField]
    private int stamina_Skill3_ShieldBash = 30;
    [SerializeField]
    private int stamina_Skill4_Spear = 20;

    /// <summary>
    /// Attack combo. 2 normal hits, 1 strong hit
    /// 
    /// normal hit:  5 dmg
    /// strong hit: 10 dmg
    /// </summary>
    /// <param name="shouldAttack"></param>
    public override void basicAttack(bool shouldAttack)
    {
        // When the timer is over the player is not allowed to continue his combo
        if (timer.hasFinished() || attackCount==3)
        {
            //reset combo
            inCombo = false;
            attackCount = 0;
            timer.timerStop();
            timer.reset();
        }

        if (shouldAttack && !attacking)
        {
            if (m_Grounded)
            {
                //Check if enough Stamina for Attack
                if (stats.currentStamina >= stamina_BasicAttack && (attackCount == 0 || attackCount == 1) || //Basic Attack (5 Stamina)
                    stats.currentStamina >= stamina_BasicAttackCombo && (attackCount == 2)) // Strong Attack (10 Stamina)
                {
                    // Determining the attackCount
                    if (attackCount == 0 && !inCombo)
                    {
                        //First attack              
                        timer.timerStart();
                        inCombo = true;
                    }

                    //Attack Count increase per attack
                    attackCount++;

                    //Playing the correct animation depending on the attackCount and setting attacking status
                    switch (attackCount)
                    {
                        case 1:
                        case 2:
                            stats.loseStamina(stamina_BasicAttack);
                            m_Anim.SetTrigger("Attack");
                            if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                            attackingRoutine = StartCoroutine(setAttacking(attackLength[0] - 0.08f));
                            Invoke("basicAttack_hit", delay_BasicAttack1);
                            break;
                        case 3:
                            stats.loseStamina(stamina_BasicAttackCombo);
                            m_Anim.SetTrigger("AttackCombo");
                            if (attackingRoutine != null) StopCoroutine(attackingRoutine);
                            attackingRoutine = StartCoroutine(setAttacking(attackLength[2] - 0.08f));
                            Invoke("strongAttack_hit", delay_BasicAttack2);
                            break;
                    }
                }
            }
            else if (!m_Grounded) //Jump attack only when falling
            {
                // Check if enough stamina is left
                if (stats.currentStamina >= stamina_JumpAttack) // Jump Attack needs 5 Stamina
                {
                    //Lose Stamina
                    stats.loseStamina(stamina_JumpAttack);
                    //Jump Attack
                    StartCoroutine(jumpAttack());
                    attackingRoutine = StartCoroutine(setAttacking(attackLength[3] - 0.08f));
                    //reset combo
                    inCombo = false;
                    attackCount = 0;
                    timer.timerStop();
                    timer.reset();
                }
            }
        }
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
    public override void skill1()
    {
        if (!attacking && m_Grounded)
        {
            attackingRoutine = StartCoroutine(setAttacking(attackLength[4]));
            m_Anim.SetTrigger("stunAttack");
            Invoke("skill1_hit", delay_Skill1); //Invoke damage function
            stats.loseStamina(20);
        }
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
    public override void skill2()
    {
        if (!attacking && m_Grounded)
        {
            attackingRoutine = StartCoroutine(setAttacking(attackLength[4]));
            m_Anim.SetTrigger("stunAttack");
            Invoke("skill2_hit", delay_Skill2); //Invoke damage function
            stats.loseStamina(15);
        }
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
    public override void skill3()
    {
        if (!attacking && m_Grounded)
        {
            attackingRoutine = StartCoroutine(setAttacking(attackLength[7]));
            m_Anim.SetTrigger("knockedBack");
            Invoke("skill3_hit", delay_Skill3);
            stats.loseStamina(30);
        }
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
    public override void skill4()
    {
        if (!attacking && m_Grounded)
        {
            attackingRoutine = StartCoroutine(setAttacking(attackLength[7]));
            Invoke("skill4_hit", delay_Skill4);
            stats.loseStamina(20);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void skill1_hit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        CharacterStats target;
        if (hit == true)
        {
            target = hit.transform.gameObject.GetComponent<CharacterStats>();
            target.startStunned(3);
            target.takeDamage(5, false);
        }
    }

    private void basicAttack_hit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        if (hit == true) hit.transform.gameObject.GetComponent<CharacterStats>().takeDamage(5, true); //Let the hit character take damage
    }

    private void strongAttack_hit()
    {
        RaycastHit2D hit = tryToHit(1.5f);
        if (hit == true) hit.transform.gameObject.GetComponent<CharacterStats>().takeDamage(10, true); //Let the hit character take damage
    }
}