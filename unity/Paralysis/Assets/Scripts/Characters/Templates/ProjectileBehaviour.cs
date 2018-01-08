using System;
using System.Collections;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Public properties
    public GameObject creator;                                          // Creator of the projectile
    public LayerMask whatToHit;                                         // Layer that shall be hitted
    public GameObject explosionPrefab;                                  // Prefab that shall be played on explosion
    public int direction = 0;                                           // Direction to travel in, -1 if left, 1 if right
    public RangedSkill SkillValues;                                     // Stuff that is placed in the skill

    // Protected properties
    protected Rigidbody2D ProjectileRigid;                              // Rigidbody of the projectile
    protected Vector3 startPos;                                         // Position where the projectile has spwaned
    protected bool stuck = false;                                       // True if projectile has successfully collided with an object
    protected bool falling = false;                                     // True if projectile has reched maxRange already
    protected bool castFinished = false;                                // While false, the projectile will not move foreward

    // Private properties
    Coroutine fallingRoutine = null;                                    // Falling Routine 
    Coroutine castRoutine = null;                                       // Falling Routine 

    // Use this for initialization
    protected void Start()
    {
        startPos = transform.position; //Save starting position
        ProjectileRigid = this.GetComponent<Rigidbody2D>();

        if (SkillValues.castTime <= 0)
        {
            castFinished = true;
        }
        else
        {
            // Freeze Rigid that it can not fall down while casting
            ProjectileRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    protected void Update()
    {
        if (!stuck && !falling)
        {
            if (Vector2.Distance(startPos, transform.position) >= SkillValues.range)
            {
                if (SkillValues.onHitEffect)
                    Die(); //Explode if max range is reached
                else
                    fallingRoutine = StartCoroutine(FallToGround());
            }
        }
    }

    protected void FixedUpdate()
    {
        if (castFinished && !stuck && !falling)
        {
            ProjectileRigid.velocity = new Vector2(SkillValues.speed.x * direction, SkillValues.speed.y);
        }
        else if (!castFinished && castRoutine == null)
        {
            castRoutine = StartCoroutine(DoCast());
        }
    }

    private IEnumerator DoCast()
    {
        yield return new WaitForSeconds(SkillValues.castTime);
        ProjectileRigid.constraints = RigidbodyConstraints2D.None;
        castFinished = true;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject == creator)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
        else
        {
            //On collision, check if collider is in whatToHit layermask
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
                creator.GetComponent<CharacterStats>().DealDamage(targetStats, SkillValues.damage, false);
                Die();
                return;
            }
            if (((RangedSkill)SkillValues).onHitEffect)
                Die();
            else
                StartCoroutine(GetStuck());
        }
    }

    protected IEnumerator GetStuck()
    {
        //Stop moving and destroy after 5 seconds 
        stuck = true;
        if (fallingRoutine != null)
            StopCoroutine(fallingRoutine);
        GetComponent<Collider2D>().enabled = false; //Disable collider
        ProjectileRigid.velocity = new Vector2(0, 0); //Stop moving
        ProjectileRigid.freezeRotation = true;
        yield return new WaitForSeconds(5);
        Die();
    }

    protected IEnumerator FallToGround()
    {
        //Move towards the ground while rotating 
        falling = true;
        ProjectileRigid.velocity = new Vector2(SkillValues.speed.x * direction, -5);

        //Turn to face the ground
        if (direction > 0)
        {
            while (transform.eulerAngles.z > -83 && !stuck)
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
            while (transform.eulerAngles.z < 83 && !stuck)
            {
                transform.Rotate(new Vector3(0, 0, 4));
                yield return new WaitForSeconds(0.02f);

                float vertical = ProjectileRigid.velocity.y;
                float horizontal = ProjectileRigid.velocity.x;
                ProjectileRigid.velocity = new Vector2(Mathf.Clamp(horizontal + 0.3f, -SkillValues.speed.x, 0), vertical - 0.3f);
            }
        }
    }

    protected virtual void Die()
    {
        //Plays explosion effect and destroys the bullet
        if (SkillValues.onHitEffect && explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (!PhotonNetwork.offlineMode)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject);
    }

    public static void Flip(GameObject ObjectToFlip)
    {
        // Multiply the sprites x local scale by -1.
        Vector3 theScale = ObjectToFlip.transform.localScale;
        theScale.x *= -1;
        ObjectToFlip.transform.localScale = theScale;
    }
}
