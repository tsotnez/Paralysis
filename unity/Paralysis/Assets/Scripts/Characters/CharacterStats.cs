using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    private Animator anim;
    private Rigidbody2D rigid;
    private ChampionClassController controller;

    public int maxHealth = 100;
    public int currentHealth;
    public int maxStamina = 100;
    public int currentStamina;

    public int staminaRegRate = 10;

    public bool stunned = false;
    public bool bleeding = false;
    public bool defensive = false;
    public bool knockedBack = false;
    public float slowFactor = 1; //Setting this to a value below 1 will slow down the character

    [SerializeField]
    private float knockBackDuration = 1; //How long the knockedBack status is set (character cant move)
    [SerializeField]
    private float knockBackForceX = 400; //Force used when knockbacking X
    [SerializeField]
    private float knockBackForceY = 600; //Force used when knockbacking Y

    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        controller = GetComponent<ChampionClassController>();
    }
    void Start()
    {
        InvokeRepeating("regenerateStamina", 0f, 1f);
    }

    private void Update()
    {
        regenerateStamina();

        // Lock character in defensive animation, if necessary
        anim.SetBool("defensive", defensive);
        if (defensive) rigid.velocity = new Vector2(0, rigid.velocity.y);
    }

    private void regenerateStamina()
    {
        if(currentStamina > maxStamina)
        {
            if (currentStamina + staminaRegRate > maxStamina) currentStamina = maxStamina;
            else currentStamina += staminaRegRate;
        }
    }

    private void bleedDamage()
    {
        takeDamage(1);
        if (bleeding) Invoke("bleedDamage", 1);
    }

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) die();
    }

    public IEnumerator stun(float time)
    {
        rigid.velocity = new Vector2(0, 0);
        stunned = true;
        anim.SetBool("stunned", true);
        yield return new WaitForSeconds(time);
        anim.SetBool("stunned", false);
        stunned = false;
    }

    //Knocks back the character from the passed origin 
    public IEnumerator knockBack(float force, Vector2 origin)
    {
        knockedBack = true;

        //Calculating the knockback's direction
        int direction = 1;
        if (transform.position.x < origin.x) direction = -1;

        //flipping the character if facing the wrong direction
        if (controller.m_FacingRight && direction > 0) controller.Flip();
        else if (!controller.m_FacingRight && direction < 0) controller.Flip();

        //Zeroing out velocity
        rigid.velocity = Vector2.zero;

        //Adding force and setting status variable
        rigid.AddForce(new Vector2(knockBackForceX * direction, knockBackForceY));
        anim.SetBool("knockedBack", true);
        yield return new WaitForSeconds(knockBackDuration);
        anim.SetBool("knockedBack", false);
        knockedBack = false;
    }

    // Set bleeding for time
    public IEnumerator bleed(float time)
    {
        bleeding = true;
        bleedDamage();
        yield return new WaitForSeconds(time);
        bleeding = false;
    }

    public IEnumerator slow(float time, float factor)
    {
        slowFactor = factor;
        yield return new WaitForSeconds(time);
        slowFactor = 1;
    }

    private void die()
    {
        Destroy(gameObject);
    }

    ////////////////////////////////////////// D   E    B   U  G ////////////////////////////////////////////////////////////

    public void deSlow(float factor)
    {
        StartCoroutine(slow(3, factor));
    }
    public void deStun(float time)
    {
        StartCoroutine(stun(time));
    }
    public void deKnock(float time)
    {
        StartCoroutine(knockBack(time, Vector2.zero));
    }
    public void deBleed(float time)
    {
        StartCoroutine(bleed(time));
    }
}
