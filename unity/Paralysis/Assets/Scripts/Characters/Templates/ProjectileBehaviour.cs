using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {
    bool stuck = false;
    bool falling = false; //True if projectile has reched maxRange already
    Coroutine fallingRoutine = null;

    public bool explodeOnHit = false; //If true, the projectile will play an explosion effect on hit. It well stuck into the Object it hit otherwise
    public GameObject creator;

    public Vector2 speed = new Vector2(7, 0); //Max Speed of the bullet
    public int direction = 0; //Direction to travel in, -1 if left, 1 if right
    public GameObject explosionPrefab;
    public float range = 5; //Distance to travel before dying
    public Skill.skillEffect effect = Skill.skillEffect.nothing;
    public int effectDuration = 1;
    public int damage = 0;
    public LayerMask whatToHit;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position; //Save starting position
    }

    // Use this for initialization
    void Update()
    {
        if (!stuck && !falling)
        {
            if (direction == -1) GetComponent<SpriteRenderer>().flipX = true; //Flip sprite if necessary
            if (Vector2.Distance(startPos, transform.position) >= range)
            {
                if (explodeOnHit)
                    explode(); //Explode if max range is reached
                else
                    fallingRoutine = StartCoroutine(fallToGround());
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!stuck && !falling)
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed.x * direction, speed.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject != creator)
        {
            //On collision, check if collider is in whatToHit layermask
            if (whatToHit == (whatToHit | (1 << collision.gameObject.layer)))
            {
                CharacterStats targetStats = collision.gameObject.GetComponent<CharacterStats>();

                switch (effect)
                {
                    case Skill.skillEffect.nothing:
                        break;
                    case Skill.skillEffect.stun:
                        targetStats.startStunned(effectDuration);
                        break;
                    case Skill.skillEffect.knockback:
                        targetStats.startKnockBack(transform.position);
                        break;
                    case Skill.skillEffect.bleed:
                        targetStats.startBleeding(effectDuration);
                        break;
                }
                targetStats.takeDamage(damage, false);
                if (!explodeOnHit)
                {
                    //Destroy on hit if no explosion effect is supposed to be played
                    Destroy(gameObject);
                    return;
                }
            }
            if (explodeOnHit)
                explode();
            else
                StartCoroutine(getStuck());
        }
    }

    void explode()
    {
        //Plays explosion effect and destroys the bullet
        if(explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator getStuck()
    {
        //Stop moving and destroy after 5 seconds 
        stuck = true;
        if (fallingRoutine != null)
            StopCoroutine(fallingRoutine);
        GetComponent<Collider2D>().enabled = false; //Disable collider
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //Stop moving
        GetComponent<Rigidbody2D>().freezeRotation = true;
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    IEnumerator fallToGround()
    {
        //Move towards the ground while rotating 
        falling = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed.x * direction, -5);

        //Turn to face the ground
        if (direction > 0)
        {
            while (transform.eulerAngles.z > -83 && !stuck)
            {
                transform.Rotate(new Vector3(0, 0, -4));
                yield return new WaitForSeconds(0.02f);

                float vertical = GetComponent<Rigidbody2D>().velocity.y;
                float horizontal = GetComponent<Rigidbody2D>().velocity.x;
                GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Clamp(horizontal - 0.3f, 0, speed.x), vertical - 0.3f);
            }
        }
        else
        {
            while (transform.eulerAngles.z < 83 && !stuck)
            {
                transform.Rotate(new Vector3(0, 0, 4));
                yield return new WaitForSeconds(0.02f);

                float vertical = GetComponent<Rigidbody2D>().velocity.y;
                float horizontal = GetComponent<Rigidbody2D>().velocity.x;
                GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Clamp(horizontal + 0.3f, -speed.x, 0), vertical - 0.3f);
            }
        }
    }
}
