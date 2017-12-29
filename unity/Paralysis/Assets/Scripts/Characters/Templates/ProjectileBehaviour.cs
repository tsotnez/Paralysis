using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Public properties
    public GameObject creator;
    public LayerMask whatToHit;
    public bool explodeOnHit = false;                                   //If true, the projectile will play an explosion effect on hit. It well stuck into the Object it hit otherwise
    public GameObject explosionPrefab;
    public Vector2 speed = new Vector2(7, 0);                           //Max Speed of the bullet
    public float range = 5;                                             //Distance to travel before dying
    public int direction = 0;                                           //Direction to travel in, -1 if left, 1 if right
    public Skill.SkillEffect effect = Skill.SkillEffect.nothing;
    public int effectDuration = 1;
    public int damage = 0;

    // Protected properties
    protected Rigidbody2D ProjectileRigid;
    protected Vector3 startPos;
    protected bool stuck = false;
    protected bool falling = false;                                     //True if projectile has reched maxRange already
    Coroutine fallingRoutine = null;

    protected void Start()
    {
        startPos = transform.position; //Save starting position
        ProjectileRigid = this.GetComponent<Rigidbody2D>();

        ////Flip sprite if necessary
        //if (direction == -1)
        //{
        //    Flip(this);
        //}
    }

    // Use this for initialization
    protected void Update()
    {
        if (!stuck && !falling)
        {
            if (Vector2.Distance(startPos, transform.position) >= range)
            {
                if (explodeOnHit)
                    Explode(); //Explode if max range is reached
                else
                    fallingRoutine = StartCoroutine(FallToGround());
            }
        }
    }

    protected void FixedUpdate()
    {
        if (!stuck && !falling)
            ProjectileRigid.velocity = new Vector2(speed.x * direction, speed.y);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject != creator)
        {
            //On collision, check if collider is in whatToHit layermask
            if (whatToHit == (whatToHit | (1 << collision.gameObject.layer)))
            {
                CharacterStats targetStats = collision.gameObject.GetComponent<CharacterStats>();

                switch (effect)
                {
                    case Skill.SkillEffect.nothing:
                        break;
                    case Skill.SkillEffect.stun:
                        targetStats.StartStunned(effectDuration);
                        break;
                    case Skill.SkillEffect.knockback:
                        targetStats.StartKnockBack(transform.position);
                        break;
                    case Skill.SkillEffect.bleed:
                        targetStats.StartBleeding(effectDuration);
                        break;
                }
                creator.GetComponent<CharacterStats>().DealDamage(targetStats, damage, false);
                if (!explodeOnHit)
                {
                    //Destroy on hit if no explosion effect is supposed to be played
                    Die();
                    return;
                }
            }
            if (explodeOnHit)
                Explode();
            else
                StartCoroutine(GetStuck());
        }
    }

    protected void Explode()
    {
        //Plays explosion effect and destroys the bullet
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Die();
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
        ProjectileRigid.velocity = new Vector2(speed.x * direction, -5);

        //Turn to face the ground
        if (direction > 0)
        {
            while (transform.eulerAngles.z > -83 && !stuck)
            {
                transform.Rotate(new Vector3(0, 0, -4));
                yield return new WaitForSeconds(0.02f);

                float vertical = ProjectileRigid.velocity.y;
                float horizontal = ProjectileRigid.velocity.x;
                ProjectileRigid.velocity = new Vector2(Mathf.Clamp(horizontal - 0.3f, 0, speed.x), vertical - 0.3f);
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
                ProjectileRigid.velocity = new Vector2(Mathf.Clamp(horizontal + 0.3f, -speed.x, 0), vertical - 0.3f);
            }
        }
    }

    protected virtual void Die()
    {
        if (!PhotonNetwork.offlineMode)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject);
    }

    public static void Flip(Component ObjectToFlip)
    {
        // Multiply the sprites x local scale by -1.
        Vector3 theScale = ObjectToFlip.transform.localScale;
        theScale.x *= -1;
        ObjectToFlip.transform.localScale = theScale;
    }

    public static void Flip(GameObject ObjectToFlip)
    {
        // Multiply the sprites x local scale by -1.
        Vector3 theScale = ObjectToFlip.transform.localScale;
        theScale.x *= -1;
        ObjectToFlip.transform.localScale = theScale;
    }
}
