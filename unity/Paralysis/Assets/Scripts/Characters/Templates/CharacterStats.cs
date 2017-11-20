using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CharacterStats : MonoBehaviour
{
    private ChampionAnimationController animCon;
    private Rigidbody2D rigid;
    private ChampionClassController controller;
    private GameObject stunnedSymbol;
    private GameObject floatingTextPrefab;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int maxStamina = 100;
    public int currentStamina;

    private int staminaRegRate = 2;
    private int staminaRegRateWhileStanding = 4;
    private int callsSinceLastAction = 10;

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
    public bool reflect = false;                // If true, character reflect damage to its dealer

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

    [Header("Effects")]
    public float PercentageMovement = 1f;
    public float PercentageDamage = 1f;
    public bool NextHitDealsStun = false;
    public bool NextHitDealsBleed = false;

    #region default

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
        InvokeRepeating("RegenerateStamina", 0f, 0.1f);

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

        // tell the animationController when player is stunned
        animCon.statStunned = this.stunned;
    }

    #endregion

    #region Health

    /// <summary>
    /// Heal for the given amount
    /// </summary>
    /// <param name="amount">Value of Heal</param>
    public void GetHealth(int amount)
    {
        if (currentHealth + amount > maxHealth)
        {
            amount = maxHealth - currentHealth;
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }

        if (amount > 0)
        {
            // Show that player got healed
            ShowFloatingText_Heal(amount);
        }
    }

    /// <summary>
    /// Calles when health equals or is less than zero
    /// </summary>
    private void Die()
    {
        GameObject.Find("manager").GetComponent<LocalMultiplayerManager>().gameOver(gameObject);
    }

    #endregion

    #region Stamina

    // Is called repeatetly to regenerate stamina value
    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina && !controller.blocking && controller.ShouldRegenerateStamina())
        {
            if (callsSinceLastAction == 10) //regenerate only if characters hasnt done anything for a while
            {
                int regRate;
                //Check if character is standing still. If so, increase reg Rate
                if (rigid.velocity == Vector2.zero)
                    regRate = staminaRegRateWhileStanding;
                else
                    regRate = staminaRegRate;


                if (currentStamina + regRate > maxStamina) this.currentStamina = this.maxStamina;
                else this.currentStamina += regRate;
            }
            else
                callsSinceLastAction++;
        }
        else
        {
            callsSinceLastAction = 0;
        }

    }

    /// <summary>
    /// Checks if enough/sufficient stamina is left and lose the stamina
    /// </summary>
    /// <param name="amount">amount of stamina</param>
    /// <returns></returns>
    public bool LoseStamina(int amount)
    {
        if (HasSufficientStamina(amount))
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
    public bool HasSufficientStamina(int amount)
    {
        if (currentStamina - amount >= 0) return true;
        else return false;
    }

    #endregion

    #region Damage

    /// <summary>
    /// General Method for dealing damage to an other player
    /// </summary>
    /// <param name="StatsOfTheTarget">Enemy Player/Target of the Damage</param>
    /// <param name="amount">Amount of Damage</param>
    /// <param name="playAnimation">shall the HIT-animation be played at the target</param>
    public void DealDamage(CharacterStats StatsOfTheTarget, int amount, bool playAnimation)
    {
        // If trinket 1 or trinket 2 is an passive trinket and has as triggertype DealDamage, then use it passively
        TooglePassiveTrinkets(controller.Trinket1, PassiveTrinket.TriggerType.DealDamage);
        TooglePassiveTrinkets(controller.Trinket2, PassiveTrinket.TriggerType.DealDamage);

        // Add percentage damage to the amount of damage
        amount = (int)System.Math.Round(amount * this.PercentageDamage);

        // If target is reflecting damage, hit the own player
        if (StatsOfTheTarget.reflect)
        {
            this.TakeDamage(amount, playAnimation);
        }
        else
        {
            StatsOfTheTarget.TakeDamage(amount, playAnimation);
        }

        // If Trinket is adding an effect
        if (NextHitDealsStun)
        {
            NextHitDealsStun = false;
            StatsOfTheTarget.StartStunned(3);
        }
        if (NextHitDealsBleed)
        {
            NextHitDealsBleed = false;
            StatsOfTheTarget.StartBleeding(3);
        }
    }

    /// <summary>
    /// Substract damage from current health.
    /// </summary>
    /// <param name="amount">Damage that shall be recieved</param>
    /// <param name="playAnimation">shall the HIT-animation be played</param>
    public void TakeDamage(int amount, bool playAnimation)
    {
        if (amount > 0)
        {
            if (!invincible)
            {
                if (controller.blocking) amount /= 2; //Reduce damage by 50% if blocking

                //Show number of damage received
                ShowFloatingText_Damage(amount);

                this.currentHealth -= amount; //Substract health
                if (playAnimation) animCon.trigHit = true; //Play hit animation
                if (currentHealth <= 0) Die();

                // If trinket 1 or trinket 2 is an passive trinket and has as triggertype TakeDamage, then use it passively
                TooglePassiveTrinkets(controller.Trinket1, PassiveTrinket.TriggerType.TakeDamage);
                TooglePassiveTrinkets(controller.Trinket2, PassiveTrinket.TriggerType.TakeDamage);
            }
            else
            {
                // Show that player is invincible
                ShowFloatingText("Invincible", new Color(0.7f, 0.7f, 0.7f, 1), -5);
            }
        }
    }

    /// <summary>
    /// Checks if a trinket is passive and have to be triggered
    /// </summary>
    /// <param name="TrinketToTrigger">Trinket that may can be triggered</param>
    /// <param name="TypeOfDamage">Type of origin of the damage</param>
    private void TooglePassiveTrinkets(Trinket TrinketToTrigger, PassiveTrinket.TriggerType TypeOfDamage)
    {
        // Only try to trigger if trinket is initialized
        if (TrinketToTrigger != null)
        {
            // Check if trinket is and PassiveTrinket and has the correct type to trigger
            if (typeof(PassiveTrinket) == TrinketToTrigger.GetType().BaseType && ((PassiveTrinket)TrinketToTrigger).TrinketTriggerType == TypeOfDamage)
            {
                // Trigger passive trinket - if it returns true it's activated
                if (((PassiveTrinket)TrinketToTrigger).Use(this))
                {
                    // Show the player that his trinket has triggered
                    ShowFloatingText_TrinketTriggered(TrinketToTrigger.DisplayName);
                }
            }
        }     
    }

    /// <summary>
    /// Same as "takeDamage" but no check if invincible and wont lower damage if blocking
    /// </summary>
    private void TakeBleedDamage(int amount)
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.Find("Canvas"), false); //Show number of damage received
        text.GetComponentInChildren<Text>().text = amount.ToString();

        this.currentHealth -= amount; //Substract health
        if (currentHealth <= 0) Die();
    }

    #endregion

    #region Stun, KnockBack, Bleed, Slow

    public void StartStunned(int time)
    {
        if (!invincible)
        {
            if (stunnedRoutine != null) StopCoroutine(stunnedRoutine);
            stunnedRoutine = StartCoroutine(Stun(time));
        }
    }

    //Sets the stunned value for given amount of time.
    private IEnumerator Stun(float time)
    {
        rigid.velocity = Vector2.zero;
        stunned = true;
        stunnedSymbol.SetActive(true);
        yield return new WaitForSeconds(time);
        stunnedSymbol.SetActive(false);
        stunned = false;
    }

    public void StartKnockBack(Vector3 origin)
    {
        if (!invincible && !controller.blocking) //Only knock back if not blocking and not invincible
        {
            if (knockBackRoutine != null) StopCoroutine(knockBackRoutine);
            knockBackRoutine = StartCoroutine(KnockBack(new Vector2(origin.x, origin.y)));
        }
    }

    //Knocks back the character from the passed origin 
    public IEnumerator KnockBack(Vector2 origin)
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
        animCon.trigKnockedBack = true;
        //Adding force and setting status variable
        rigid.AddForce(new Vector2(knockBackForceX * direction, knockBackForceY), ForceMode2D.Impulse);
        yield return new WaitUntil(() => !animCon.m_Grounded);
        yield return new WaitUntil(() => animCon.m_Grounded);
        animCon.trigKnockedBackEnd = true;
        knockedBack = false;
    }

    public void StartBleeding(int time)
    {
        if (!invincible)
        {
            if (bleedingRoutine != null) StopCoroutine(bleedingRoutine);
            bleedingRoutine = StartCoroutine(Bleed(time));
        }
    }

    // Set bleeding for time
    private IEnumerator Bleed(float time)
    {
        bleeding = true;
        if (!IsInvoking("BleedDamage")) BleedDamage();
        yield return new WaitForSeconds(time);
        bleeding = false;
    }

    //Repeats itself as long as bleeding is true
    private void BleedDamage()
    {
        TakeBleedDamage(1);
        if (bleeding) Invoke("BleedDamage", 1);
    }

    //Slows down the character by the given factor for the given time
    public IEnumerator Slow(float time, float factor)
    {
        slowFactor = factor;
        yield return new WaitForSeconds(time);
        slowFactor = 1;
    }

    #endregion

    #region Floating Text

    /// <summary>
    /// Shows a floating text in front of the player
    /// </summary>
    /// <param name="TextValue">Text that shall be displayed</param>
    /// <param name="ColorValue">Color of the text</param>
    public void ShowFloatingText(string TextValue, Color ColorValue, int changeFontSizeBy)
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.Find("Canvas"), false);
        Text component = text.GetComponentInChildren<Text>();

        component.text = TextValue;
        component.color = ColorValue;
        component.fontSize += changeFontSizeBy;
    }

    /// <summary>
    /// Shows healing amout in front of the player
    /// </summary>
    /// <param name="value">Healing amount</param>
    public void ShowFloatingText_Heal(int value)
    {
        ShowFloatingText(value.ToString(), Color.green, 0);
    }

    /// <summary>
    /// Shows damage taken amout in front of the player
    /// </summary>
    /// <param name="value">Damage amount</param>
    public void ShowFloatingText_Damage(int value)
    {
        ShowFloatingText(value.ToString(), Color.red, 0);
    }

    /// <summary>
    /// Shows which trigger has activated in front of the player
    /// </summary>
    /// <param name="TriggerName">Name of the trigger</param>
    public void ShowFloatingText_TrinketTriggered(string TriggerName)
    {
        ShowFloatingText(TriggerName.ToString() + Environment.NewLine + "has triggered", Color.yellow, -10);
    }

    #endregion

    //////////////////////////////////////////  D   E   B   U  G  //////////////////////////////////////////
    #region Debug

    //public void deSlow(float factor)
    //{
    //    StartCoroutine(slow(3, factor));
    //}
    //public void deStun(float time)
    //{
    //    StartCoroutine(stun(time));
    //}
    //public void deKnock(float time)
    //{
    //    startKnockBack(Vector3.zero);
    //}
    //public void deBleed(float time)
    //{
    //    StartCoroutine(Bleed(time));
    //}

    #endregion

}