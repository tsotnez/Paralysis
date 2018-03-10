using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : Photon.MonoBehaviour
{
    private ChampionAnimationController animCon;
    private Rigidbody2D rigid;
    private ChampionClassController controller;
    private GameObject stunnedSymbol;
    private GameObject floatingTextPrefab;

    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            if (currentHealth <= 0)
                Die();
        }
    }
    public int maxStamina = 100;
    public int CurrentStamina;

    private int staminaRegRate = 2;
    private int staminaRegRateWhileStanding = 4;
    private int callsSinceLastAction = 10;

    [Header("Statusses")]
    public bool CharacterDied = false;          // Stores if character has died
    public bool stunned = false;                // Stores if character is stunned
    public Coroutine stunnedRoutine = null;     // Stores the stunned Coroutine
    public bool bleeding = false;               // Takes damage while true
    public Coroutine bleedingRoutine = null;    // Stores the bleeding Coroutine
    public bool knockedBack = false;            // Prevents actions while being knocked back
    public Coroutine knockBackRoutine = null;   // Stores the knockBack Coroutine
    public Coroutine slowRoutine = null;        // Stores the slow Coroutine
    public bool immovable = false;              // Stores if character is immoveable
    public bool invincible = false;             // If true, character cant be harmed 
    public bool reflect = false;                // If true, character reflect damage to its dealer
    public bool invisible = false;              // Is the character invisible
    Coroutine invisRoutine = null;              // Stores the invisible coroutine

    [Header("Knock Back")]
    [SerializeField]
    private float knockBackForceX = 6;          // Force used when knockbacking X
    [SerializeField]
    private float knockBackForceY = 12;         // Force used when knockbacking Y

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

    Color col;                                  // Color of the sprite renderer. Used to store default color while invisible

    #region default

    void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;

        animCon = transform.Find("graphics").GetComponent<ChampionAnimationController>();
        rigid = GetComponent<Rigidbody2D>();
        controller = GetComponent<ChampionClassController>();
        floatingTextPrefab = Resources.Load("Prefabs/FloatingText/FloatingText") as GameObject;
        stunnedSymbol = transform.Find("stunnedSymbol").gameObject;
    }
    void Start()
    {
        if(photonView.isMine)
        {
            InvokeRepeating("RegenerateStamina", 0f, 0.1f);
        }

        if (PhotonNetwork.offlineMode)
            SetTeamColor();

        col = transform.Find("graphics").GetComponent<SpriteRenderer>().color;
    }

    public void SetTeamColor()
    {
        //Assign team Color
        if (gameObject.layer == LayerMask.NameToLayer("Team1"))
            TeamColor.sprite = Team1Color;
        else
            TeamColor.sprite = Team2Color;
    }

    private void Update()
    {
        if (Input.GetKeyDown("k"))
            TakeDamage(100, false);

        float h = CurrentHealth;
        float mH = maxHealth;

        float s = CurrentStamina;
        float mS = maxStamina;

        float hpPercent = h / mH;
        float staminaPercent = s / mS;

        if (hotbar != null)
            hotbar.setFillAmounts(hpPercent, staminaPercent);

        floatingHpBar.fillAmount = hpPercent;
        floatingStaminaBar.fillAmount = staminaPercent;

        // tell the animationController when player is stunned (only if local Client --- Jan)
        if (photonView != null && photonView.isMine)
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
        if (CurrentHealth + amount > maxHealth)
        {
            amount = maxHealth - CurrentHealth;
            CurrentHealth = maxHealth;
        }
        else
        {
            CurrentHealth += amount;
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
        CharacterDied = true;
        animCon.statDead = true;
        if (PhotonNetwork.offlineMode || photonView.isMine)
            GameplayManager.Instance.playerDied(gameObject);

        // Prevent body from sliding when killed in air or with knockback
        if (knockedBack || !animCon.m_Grounded)
        {
            StartCoroutine(Die_ManageFallingToGround());
        }
    }

    private IEnumerator Die_ManageFallingToGround()
    {
        if (animCon.m_Grounded)
        {
            yield return new WaitUntil(() => !animCon.m_Grounded);
        }
        yield return new WaitUntil(() => animCon.m_Grounded);
        rigid.velocity = Vector2.zero;
    }

    #endregion

    #region Stamina

    // Is called repeatetly to regenerate stamina value
    private void RegenerateStamina()
    {
        if (CurrentStamina < maxStamina && !controller.blocking && controller.ShouldRegenerateStamina())
        {
            if (callsSinceLastAction == 10) //regenerate only if characters hasnt done anything for a while
            {
                int regRate;
                //Check if character is standing still. If so, increase reg Rate
                if (rigid.velocity == Vector2.zero)
                    regRate = staminaRegRateWhileStanding;
                else
                    regRate = staminaRegRate;


                if (CurrentStamina + regRate > maxStamina) this.CurrentStamina = this.maxStamina;
                else this.CurrentStamina += regRate;
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
            if(PhotonNetwork.offlineMode || photonView.isMine)
            {
                CurrentStamina -= amount;
            }
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
        if (CurrentStamina - amount >= 0) return true;
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
        amount = (int)Math.Round(amount * PercentageDamage);

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
    [PunRPC]
    public void TakeDamage(int amount, bool playAnimation, bool issueRPC = true)
    {
        //If we are online and its not mine tell others to take damage
        if (!PhotonNetwork.offlineMode && !photonView.isMine && issueRPC)
        {
            photonView.RPC("TakeDamage", PhotonTargets.Others, amount, playAnimation, false);
            return;
        }

        if (amount > 0 && !CharacterDied)
        {
            if (!invincible)
            {
                if (controller.blocking) amount /= 2; //Reduce damage by 50% if blocking

                //Show number of damage received
                ShowFloatingText_Damage(amount);

                if (photonView.isMine) //only substract health and check for trinkets if running on originial instance
                {
                    CurrentHealth = Mathf.Clamp(this.CurrentHealth -= amount, 0, maxHealth); //Substract health
                    if (playAnimation) animCon.trigHit = true; //Play hit animation

                    // If trinket 1 or trinket 2 is an passive trinket and has as triggertype TakeDamage, then use it passively
                    TooglePassiveTrinkets(controller.Trinket1, PassiveTrinket.TriggerType.TakeDamage);
                    TooglePassiveTrinkets(controller.Trinket2, PassiveTrinket.TriggerType.TakeDamage);
                }
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
    [PunRPC]
    private void TakeBleedDamage(int amount, bool issueRPC = true)
    {
        if (CharacterDied)
            return;

        //if (!PhotonNetwork.offlineMode && issueRPC)
        //    photonView.RPC("TakeBleedDamage", PhotonTargets.Others, amount, false);

        GameObject text = Instantiate(floatingTextPrefab, transform.Find("Canvas"), false); //Show number of damage received
        text.GetComponentInChildren<Text>().text = amount.ToString();

        if (photonView.isMine)
            CurrentHealth = Mathf.Clamp(this.CurrentHealth -= amount, 0, maxHealth); //Substract health
    }

    #endregion

    #region Skill Special Effects

    #region Stun

    /// <summary>
    /// Sets the stunned value till disable
    /// </summary>
    [PunRPC]
    public void StartStunned(bool issueRPC = true)
    {
        //If called from a copy, issue rpc on original instance
        if (!PhotonNetwork.offlineMode && !photonView.isMine && issueRPC)
        {
            photonView.RPC("StartKnockBack", PhotonTargets.Others, false);
            return;
        }

        if (!invincible)
        {
            // Stop coroutine if running
            if (stunnedRoutine != null) StopCoroutine(stunnedRoutine);

            rigid.velocity = new Vector2(0, rigid.velocity.y);
            stunned = true;
            stunnedSymbol.SetActive(true);
        }
    }

    /// <summary>
    /// Stop the stun immediately
    /// </summary>
    public void StopStunned()
    {
        if (stunned)
        {
            // Stop coroutine if running
            if (stunnedRoutine != null) StopCoroutine(stunnedRoutine);

            //manually disable stunned symbol and remove stunned effect
            stunnedSymbol.SetActive(false);
            stunned = false;
        }
    }

    /// <summary>
    /// Sets the stunned value for given amount of time.
    /// </summary>
    /// <param name="time"></param>
    [PunRPC]
    public void StartStunned(float time, bool issueRPC = true)
    {
        //If called from a copy, issue rpc on original instance
        if (!PhotonNetwork.offlineMode && !photonView.isMine && issueRPC)
        {
            photonView.RPC("StartStunned", PhotonTargets.Others, time, false);
            return;
        }

        if (!invincible)
        {
            if (stunnedRoutine != null) StopCoroutine(stunnedRoutine);
            stunnedRoutine = StartCoroutine(Stun(time));
        }
    }

    private IEnumerator Stun(float time)
    {
        rigid.velocity = Vector2.zero;
        stunned = true;
        stunnedSymbol.SetActive(true);
        yield return new WaitForSeconds(time);
        stunnedSymbol.SetActive(false);
        stunned = false;
    }

    #endregion

    #region KnockBack

    [PunRPC]
    public void StartKnockBack(Vector3 origin, bool issueRPC = true)
    {
        //If called from a copy, issue rpc on original instance
        if (!PhotonNetwork.offlineMode && !photonView.isMine && issueRPC)
        {
            photonView.RPC("StartKnockBack", PhotonTargets.Others, origin, false);
            return;
        }

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
        if (controller.FacingRight && direction > 0) controller.Flip();
        else if (!controller.FacingRight && direction < 0) controller.Flip();

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

    #endregion

    #region Bleed

    [PunRPC]
    public void StartBleeding(int time, bool issueRPC = true)
    {
        //If called from a copy, issue rpc on original instance
        if (!PhotonNetwork.offlineMode && !photonView.isMine && issueRPC)
        {
            photonView.RPC("StartBleeding", PhotonTargets.Others, time, false);
            return;
        }

        if (!invincible)
        {
            if (bleedingRoutine != null) StopCoroutine(bleedingRoutine);
            bleedingRoutine = StartCoroutine(Bleed(time));
        }
    }

    public void StopBleeding()
    {
        if (bleeding)
        {
            if (bleedingRoutine != null) StopCoroutine(bleedingRoutine);
            bleeding = false;
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

    #endregion

    #region Slow

    /// <summary>
    /// Slows down the character by the given factor for the given time
    /// </summary>
    /// <param name="time">duration of slow</param>
    /// <param name="factor">factor of slowing | 0,2 causes a movementspeed of 0,8 (80%)</param>
    [PunRPC]
    public void StartSlow(float time, float factor, bool issueRPC = true)
    {
        //If called from a copy, issue rpc on original instance
        if (!PhotonNetwork.offlineMode && !photonView.isMine && issueRPC)
        {
            photonView.RPC("StartSlow", PhotonTargets.Others, time, factor, false);
            return;
        }

        if (!invincible)
        {
            if (slowRoutine != null) StopCoroutine(slowRoutine);
            slowRoutine = StartCoroutine(Slow(time, factor));
        }
    }

    public void StopSlow()
    {
        if (PercentageMovement < 1)
        {
            if (slowRoutine != null) StopCoroutine(slowRoutine);
            PercentageMovement = 1;
        }
    }

    private IEnumerator Slow(float time, float factor)
    {
        PercentageMovement -= PercentageMovement * factor;
        yield return new WaitForSeconds(time);
        PercentageMovement = 1;
    }

    #endregion

    #endregion

    #region Invisibility

    /// <summary>
    /// Called to start the invis routine from network or local
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="seconds"></param>
    /// <param name="issueRpc"></param>
    [PunRPC]
    public void StartInvisible(int seconds, bool issueRpc = true)
    {
        if (!PhotonNetwork.offlineMode && issueRpc)
            photonView.RPC("StartInvisible", PhotonTargets.Others, seconds, false);

        if (invisRoutine != null) StopCoroutine(invisRoutine);
        invisRoutine = StartCoroutine(ManageInvisibility(seconds));
    }

    /// <summary>
    /// Turns the character invisible, setting the staus variable and changing the sprite renderes color
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator ManageInvisibility(int seconds)
    {
        invisible = true;

        //Set Alpha to 0 if on different client (Character should be in fact invisible) and to 0.5f if on the controlling client for a visual represanttation of invisibility
        //Also disable floating status bar
        float newAlpha;
        if (!PhotonNetwork.offlineMode && photonView.isMine)
            newAlpha = 0.5f;
        else
        {
            newAlpha = 0f;
            floatingHpBar.transform.parent.parent.gameObject.SetActive(false);
        }

        // set transparency
        SpriteRenderer ren = transform.Find("graphics").GetComponent<SpriteRenderer>();
        Color oldCol = ren.color;
        oldCol.a = newAlpha;
        ren.color = oldCol;

        yield return new WaitForSeconds(seconds);
        StopInvisible();
    }

    /// <summary>
    /// Resets invsi values and sprite renderer color
    /// </summary>
    /// <param name="issueRpc"></param>
    [PunRPC]
    public void StopInvisible(bool issueRpc = true)
    {
        if (!PhotonNetwork.offlineMode && issueRpc)
            photonView.RPC("StopInvisible", PhotonTargets.Others, false);

        invisible = false;
        if (invisRoutine != null) StopCoroutine(invisRoutine);
        transform.Find("graphics").GetComponent<SpriteRenderer>().color = col;
        floatingHpBar.transform.parent.parent.gameObject.SetActive(true);
    }

    #endregion

    #region Trinket Stuff

    public void StartTrinketEffects()
    {
        controller.transform.Find("graphics").parent.localScale = new Vector3(1.2f, 1.2f, 1f);
        controller.transform.Find("effectOverlay").gameObject.SetActive(true);
    }

    public void StopTrinketEffects()
    {
        controller.transform.Find("graphics").parent.localScale = Vector3.one;
        controller.transform.Find("effectOverlay").gameObject.SetActive(false);
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
        if (!PhotonNetwork.offlineMode)
            photonView.RPC("ShowFloatingTextSerialized", PhotonTargets.Others, TextValue, ColorValue.a, ColorValue.r, ColorValue.g, ColorValue.b, changeFontSizeBy);

        GameObject text = Instantiate(floatingTextPrefab, transform.Find("Canvas"), false);
        Text component = text.GetComponentInChildren<Text>();

        component.text = TextValue;
        component.color = ColorValue;
        component.fontSize += changeFontSizeBy;
    }

    /// <summary>
    /// Called by RPC cause Color cant be serialized
    /// </summary>
    [PunRPC]
    public void ShowFloatingTextSerialized(string TextValue, float a, float r, float g, float b, int changeFontSizeBy)
    {
        Color ColorValue = new Color
        {
            a = a,
            r = r,
            g = g,
            b = b
        };

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
    public void ShowFloatingText_Heal(int value, bool issueRPC = true)
    {
        ShowFloatingText(value.ToString(), Color.green, 0);
    }

    /// <summary>
    /// Shows damage taken amout in front of the player
    /// </summary>
    /// <param name="value">Damage amount</param>
    public void ShowFloatingText_Damage(int value, bool issueRPC = true)
    {
        ShowFloatingText(value.ToString(), Color.red, 0);
    }

    /// <summary>
    /// Shows which trigger has activated in front of the player
    /// </summary>
    /// <param name="TriggerName">Name of the trigger</param>
    public void ShowFloatingText_TrinketTriggered(string TriggerName, bool issueRPC = true)
    {
        ShowFloatingText(TriggerName.ToString() + Environment.NewLine + "has triggered", Color.yellow, -10);
    }

    #endregion

    #region Reset

    public void ResetValues()
    {
        StopAllCoroutines();
        CurrentHealth = 100;
        CurrentStamina = maxStamina;

        animCon.statDead = false;
        CharacterDied = false;
        stunned = false;
        bleeding = false;
        knockedBack = false;
        immovable = false;
        invincible = false;
        reflect = false;
        invisible = false;
    }

    #endregion
}