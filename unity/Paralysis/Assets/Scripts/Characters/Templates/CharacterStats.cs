﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CharacterStats : MonoBehaviour {

    private ChampionAnimationController animCon;
    private Rigidbody2D rigid;
    private ChampionClassController controller;
    private GameObject stunnedSymbol;

    private GameObject floatingTextPrefab;
    private float nextTextPosition = 1; //Will the next damage number be instaitated on the left or on the right 

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int maxStamina = 100;
    public int currentStamina;

	private int staminaRegRate = 4;

    [Header("Statusses")]
    public bool stunned = false;
    public Coroutine stunnedRoutine = null;     // Stores the stunned Coroutine
    public bool bleeding = false;               // Takes damage while true
    public Coroutine bleedingRoutine = null;    // Stores the bleeding Coroutine
    public bool knockedBack = false;            // Prevents actions while being knocked back
    public Coroutine knockBackRoutine = null;   // Stores the knockBack Coroutine
    public bool immovable = false;              // Stores if character is immoveable
    public float slowFactor = 1;                // Setting this to a value below 1 will slow down the character
    public bool invincible = false;             // If true, character cant be harmed 
    public bool trigHit = false;

    [Header("Knock Back")]
    [SerializeField]
    private float knockBackForceX = 400;        // Force used when knockbacking X
    [SerializeField]
    private float knockBackForceY = 600;        // Force used when knockbacking Y

    [HideInInspector]
    public HotbarController hotbar;

    [Header("Status Bars")]
    public Image TeamColor;
    public Sprite Team1Color;
    public Sprite Team2Color;
    public Image floatingHpBar;
    public Image floatingStaminaBar;

    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        animCon = transform.Find("graphics").GetComponent<ChampionAnimationController>();
        rigid = GetComponent<Rigidbody2D>();
        controller = GetComponent<ChampionClassController>();
        floatingTextPrefab = Resources.Load("Prefabs/FloatingText/FloatingText") as GameObject;
        stunnedSymbol = transform.Find("stunnedSymbol").gameObject;
    }
    void Start()
    {
        InvokeRepeating("regenerateStamina", 0f, 1f);
        
        //Assign team Color
        if (gameObject.layer == 11)
            TeamColor.sprite = Team1Color;
        else
            TeamColor.sprite = Team2Color;

    }

    private void Update()
    {
        float h = currentHealth;
        float mH = maxHealth;

        float s = currentStamina;
        float mS = maxStamina;

        float hpPercent = h / mH;
        float staminaPercent = s / mS;

        hotbar.setFillAmounts(hpPercent, staminaPercent);
        floatingHpBar.fillAmount = hpPercent;
        floatingStaminaBar.fillAmount = staminaPercent;
    }

    // Is called repeatetly to regenerate stamina value
    private void regenerateStamina()
    {
        if(currentStamina < maxStamina && !controller.blocking)
        {
            if (currentStamina + staminaRegRate > maxStamina) this.currentStamina = this.maxStamina;
            else this.currentStamina += this.staminaRegRate;
		}

    }

    /// <summary>
    /// Checks if enough/sufficient stamina is left and lose the stamina
    /// </summary>
    /// <param name="amount">amount of stamina</param>
    /// <returns></returns>
    public bool loseStamina(int amount)
    {
        if (hasSufficientStamina(amount))
        {
            currentStamina -= amount;
            return true;
        }
        return false;
	}

    /// <summary>
    /// Checks if enough/sufficient stamina is left
    /// </summary>
    /// <param name="amount">amount of stamina</param>
    /// <returns></returns>
    public bool hasSufficientStamina(int amount)
    {
        if (currentStamina - amount >= 0) return true;
        else return false;
    }

    //Substract damage from current health.
    public void takeDamage(int amount, bool playAnimation)
    {
        if (!invincible && amount > 0)
        {
            if (controller.blocking) amount /= 2; //Reduce damage by 50% if blocking

            GameObject text = Instantiate(floatingTextPrefab, transform.Find("Canvas"), false); //Show number of damage received
            nextTextPosition = -nextTextPosition; //Toggle next text positon
            text.GetComponentInChildren<Text>().text = amount.ToString();

            this.currentHealth -= amount; //Substract health
            if (playAnimation) trigHit = true; //Play hit animation
            if (currentHealth <= 0) die();
        }
    }

    /// <summary>
    /// Same as "takeDamage" but no check if invincible and wont lower damage if blocking
    /// </summary>
    private void takeBleedDamage(int amount)
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.Find("Canvas"), false); //Show number of damage received
        nextTextPosition = -nextTextPosition; //Toggle next text positon
        text.GetComponentInChildren<Text>().text = amount.ToString();

        this.currentHealth -= amount; //Substract health
        if (currentHealth <= 0) die();
    }

    public void startStunned(int time)
    {
        if (!invincible)
        {
            if (stunnedRoutine != null) StopCoroutine(stunnedRoutine);
            stunnedRoutine = StartCoroutine(stun(time));
        }
    }

    //Sets the stunned value for given amount of time.
    private IEnumerator stun(float time)
    {
        rigid.velocity = Vector2.zero;
        stunned = true;
        stunnedSymbol.SetActive(true);
        yield return new WaitForSeconds(time);
        stunnedSymbol.SetActive(false);
        stunned = false;
    }

    public void startKnockBack(Vector3 origin)
    {
        if (!invincible && !controller.blocking) //Only knock back if not blocking and not invincible
        {
            if (knockBackRoutine != null) StopCoroutine(knockBackRoutine);
            knockBackRoutine = StartCoroutine(knockBack(new Vector2(origin.x, origin.y)));
        }
    }

    //Knocks back the character from the passed origin 
    public IEnumerator knockBack(Vector2 origin)
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
        animCon.statKnockedBack = true;
        //Adding force and setting status variable
        rigid.AddForce(new Vector2(knockBackForceX * direction, knockBackForceY), ForceMode2D.Impulse);
        yield return new WaitUntil(() => !animCon.m_Grounded);
        yield return new WaitUntil(() => animCon.m_Grounded);
        animCon.statKnockedBack = false;
        knockedBack = false;
    }

    public void startBleeding(int time)
    {
        if (!invincible)
        {
            if (bleedingRoutine != null) StopCoroutine(bleedingRoutine);
            bleedingRoutine = StartCoroutine(bleed(time));
        }
    }
    
    // Set bleeding for time
    private IEnumerator bleed(float time)
    {
        bleeding = true;
        if(!IsInvoking("bleedDamage")) bleedDamage();
        yield return new WaitForSeconds(time);
        bleeding = false;
    }

    //Repeats itself as long as bleeding is true
    private void bleedDamage()
    {
        takeBleedDamage(1);
        if (bleeding) Invoke("bleedDamage", 1);
    }

    //Slows down the character by the given factor for the given time
    public IEnumerator slow(float time, float factor)
    {
        slowFactor = factor;
        yield return new WaitForSeconds(time);
        slowFactor = 1;
    }

    //Calles when health equals or is less than zero
    private void die()
    {
        GameObject.Find("manager").GetComponent<LocalMultiplayerManager>().gameOver(gameObject);
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
        startKnockBack(Vector3.zero);
    }
    public void deBleed(float time)
    {
        StartCoroutine(bleed(time));
    }
}