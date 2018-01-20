using System;
using System.Collections;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Getter & Setter
    public bool CastFinished { get; protected set; }                    // While false, the projectile will not move foreward
    public bool Stuck { get; protected set; }                           // True if projectile has successfully collided with an object
    public bool Falling { get; protected set; }                         // True if projectile has reched maxRange already  
    public bool Interrupted { get; protected set; }                     // True if creator got stunned or knockedback while casting

    // Public properties
    [HideInInspector]
    public GameObject creator;                                          // Creator of the projectile
    public LayerMask whatToHit;                                         // Layer that shall be hitted
    public GameObject explosionPrefab;                                  // Prefab that shall be played on explosion
    public int direction = 0;                                           // Direction to travel in, -1 if left, 1 if right
    public RangedSkill SkillValues;                                     // Stuff that is placed in the skill

    // Protected properties
    protected CharacterStats CreatorStats;                              // CharacterStats of the creator
    protected Rigidbody2D ProjectileRigid;                              // Rigidbody of the projectile
    protected Collider2D ProjectileCollider;                            // Collider of the projectile
    protected Vector3 startPos;                                         // Position where the projectile has spawned
    protected short networkId;                                            // Network id of the object, not set unless online

    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    // Private properties
    Coroutine fallingRoutine = null;                                    // Falling Routine  
    Coroutine observeRoutine = null;                                    // Observe Routine

    protected void Awake()
    {   
        // Save starting position
        startPos = transform.position;

        //set a network id
        networkId = (short)(CRCCalculator.CRCFromVector(startPos) ^ creator.gameObject.name.GetHashCode());

        //add behaviour to the projectile manager
        if(!PhotonNetwork.offlineMode)
        {
            NetworkProjectileManager.Instance.addProjectile(networkId, this);
        }
    }

    // Use this for initialization
    protected void Start()
    {
        // Get Objects
        ProjectileRigid = GetComponent<Rigidbody2D>();
        ProjectileCollider = GetComponent<Collider2D>();
        CreatorStats = creator.GetComponent<CharacterStats>();

        if (SkillValues.castTime <= 0)
        {
            CastFinished = true;
            ProjectileCollider.enabled = true;
        }
        else
        {
            // Freeze Rigid that it can not fall down while casting
            ProjectileRigid.constraints = RigidbodyConstraints2D.FreezeAll;
            // Disable collider to prevent collisions while casting
            ProjectileCollider.enabled = false;

            // Start cast and observe stats
            StartCoroutine(DoCast());
            observeRoutine = StartCoroutine(ObserveCreatorStats());
        }

        Stuck = false;
        Falling = false;
    }

    protected void Update()
    {
        if (!Stuck && !Falling)
        {
            if (Vector2.Distance(startPos, transform.position) >= SkillValues.range)
            {
                if (SkillValues.onHitEffect)
                    Die(); // Explode if max range is reached
                else
                    fallingRoutine = StartCoroutine(FallToGround());
            }
        }
    }

    protected void FixedUpdate()
    {
        if (CastFinished && !Interrupted && !Stuck && !Falling)
        {
            ProjectileRigid.velocity = new Vector2(SkillValues.speed.x * direction, SkillValues.speed.y);
        }
    }

    /// <summary>
    /// Manage casting
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator DoCast()
    {
        // Begin cast
        CastFinished = false;
        
        //Show castBAr
        GameObject castBar = GameObject.FindGameObjectWithTag("CastBar");
        castBar.GetComponent<CastBarBehaviour>().startCast(SkillValues.castTime, SkillValues.name);

        // Wait till cast ends
        yield return new WaitForSeconds(SkillValues.castTime);

        // Stop Observing creator
        StopCoroutine(observeRoutine);

        // End freeze and let the projectile fly far far away and crash something to death!
        ProjectileRigid.constraints = RigidbodyConstraints2D.None;
        ProjectileCollider.enabled = true;
        CastFinished = true;
    }

    /// <summary>
    /// Observe the creator while casting if it gets stunned or knockedback
    /// If so let the projectile die
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator ObserveCreatorStats()
    {
        // Wait till Creator got stunned or knockedback
        yield return new WaitUntil(() => CreatorStats.stunned || CreatorStats.knockedBack);
        Interrupted = true;
        ProjectileCollider.enabled = false;

        // Wait before dying that Champion can recognize it
        yield return new WaitForSeconds(0.05f);
        // Let the projectile die
        Die(); // explosion or disapear animation?!?
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == creator)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision);
        }
        else
        {
            // On collision, check if collider is in whatToHit layermask
            if (whatToHit == (whatToHit | (1 << collision.gameObject.layer)))
            {
                CharacterStats targetStats = collision.gameObject.GetComponent<CharacterStats>();

                switch (SkillValues.effect)
                {
                    case Skill.SkillEffect.nothing:
                        break;
                    case Skill.SkillEffect.stun:
                        targetStats.StartStunned(SkillValues.effectDuration);
                        break;
                    case Skill.SkillEffect.knockback:
                        targetStats.StartKnockBack(transform.position);
                        break;
                    case Skill.SkillEffect.bleed:
                        targetStats.StartBleeding(SkillValues.effectDuration);
                        break;
                    case Skill.SkillEffect.slow:
                        targetStats.StartSlow(SkillValues.effectDuration, SkillValues.effectValue);
                        break;
                }

                if(PhotonNetwork.offlineMode)
                {
                    CreatorStats.DealDamage(targetStats, SkillValues.damage, false);
                }
                else 
                {
                    // On the network only deal damage on collision with projectiles 
                    // from the shooters perspective.
                    if(creator.GetComponent<PhotonView>().isMine)
                    {
                        CreatorStats.DealDamage(targetStats, SkillValues.damage, false);                        
                     }
                }
 
                Die();
                return;
            }
            if (SkillValues.onHitEffect)
                Die();
            else
                StartCoroutine(GetStuck());
        }
    }

    protected IEnumerator GetStuck()
    {
        // Stop moving and destroy after 5 seconds 
        Stuck = true;
        if (fallingRoutine != null)
            StopCoroutine(fallingRoutine);
        GetComponent<Collider2D>().enabled = false;     // Disable collider
        ProjectileRigid.velocity = new Vector2(0, 0);   // Stop moving
        ProjectileRigid.freezeRotation = true;
        yield return new WaitForSeconds(5);
        Die();
    }

    protected IEnumerator FallToGround()
    {
        // Move towards the ground while rotating 
        Falling = true;
        ProjectileRigid.velocity = new Vector2(SkillValues.speed.x * direction, -5);

        // Turn to face the ground
        if (direction > 0)
        {
            while (transform.eulerAngles.z > -83 && !Stuck)
            {
                transform.Rotate(new Vector3(0, 0, -4));
                yield return new WaitForSeconds(0.02f);

                float vertical = ProjectileRigid.velocity.y;
                float horizontal = ProjectileRigid.velocity.x;
                ProjectileRigid.velocity = new Vector2(Mathf.Clamp(horizontal - 0.3f, 0, SkillValues.speed.x), vertical - 0.3f);
            }
        }
        else
        {
            while (transform.eulerAngles.z < 83 && !Stuck)
            {
                transform.Rotate(new Vector3(0, 0, 4));
                yield return new WaitForSeconds(0.02f);

                float vertical = ProjectileRigid.velocity.y;
                float horizontal = ProjectileRigid.velocity.x;
                ProjectileRigid.velocity = new Vector2(Mathf.Clamp(horizontal + 0.3f, -SkillValues.speed.x, 0), vertical - 0.3f);
            }
        }
    }

    public virtual void Die()
    {
        //Plays explosion effect and destroys the bullet
        if (SkillValues.onHitEffect && explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        isDead = true;
        if(!PhotonNetwork.offlineMode)
        {
            //Tell the projectile manager this objected died, this collision
            //might not happen everywhere, for instance, the player could be
            //hit by the projectile, on this screen, but not on another.
            NetworkProjectileManager.Instance.killProjectile(networkId);
        }

        Destroy(gameObject);
    }

    public static void Flip(GameObject ObjectToFlip)
    {
        // Multiply the sprites x local scale by -1.
        Vector3 theScale = ObjectToFlip.transform.localScale;
        theScale.x *= -1;
        ObjectToFlip.transform.localScale = theScale;
    }

    protected virtual void OnDestroy()
    {
        if(!PhotonNetwork.offlineMode)
        {
            NetworkProjectileManager.Instance.removeProjectile(networkId, this);
        }
    }
}
