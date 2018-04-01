using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIUserControl : MonoBehaviour {

    [HideInInspector]
    public bool inputJump;
    [HideInInspector]
    public bool inputAttack;                        // normal attack
    [HideInInspector]
    public int inputDashDirection = 0;
    [HideInInspector]
    public bool inputDown;                          // block or fall down
    [HideInInspector]
    public float inputMove = 0;                     // Is left and right movement
    [HideInInspector]
    public bool inputSkill1;
    [HideInInspector]
    public bool inputSkill2;
    [HideInInspector]
    public bool inputSkill3;
    [HideInInspector]
    public bool inputSkill4;
    [HideInInspector]
    public bool inputTrinket1;
    [HideInInspector]
    public bool inputTrinket2;

    protected AIGetClosetEnemy aiTargeting;
    protected ChampionAnimationController animCon;
    protected ChampionClassController champClassCon;
    protected CharacterStats charStats;
    protected GameObject targetPlayer;
    protected CharacterStats targetStats;
    protected Vector2 targetPosition;

    protected int currentHealth;
    protected int previousHealth;
    protected int currentStamina;
    protected int previousStamina;



    protected float targetDirectionX = 0;
    protected float targetDistance = 0f;
    protected float facingDirection = 1;
    protected float yDiff = 0;
    protected bool isGrounded;
    protected bool facingTarget = false;

    protected bool isRetreating = false;
    protected float retreatUntilStamina = 50f;
    protected float retreatDuration = 0f;
    protected float initialRetreatTime = 0f;

    protected Section mySection;
    protected Section targetSection;
    protected Section retreatSection;

    protected SectionPathNode[] currentNodes;
    protected SectionPathNode currentNode;
    protected int currentNodeIndex = 0;
    private const float MIN_DISTANCE_TO_NODE = .115f;

    public enum AI_GOALS { MOVE_TO_LOCATION, MOVE_THROUGH_NODES, JUMP1, JUMP2, FALL_THROUGH, STAND_BY, WAIT };
    public float waitTime;

    public enum TRIGGER_GOALS { MOVE_CLOSER, WAIT_FOR_ATTACK, RETREAT, CONTINUE }

    protected AI_GOALS distRetNewGoal;
    protected float triggerWait = 0f;

    protected AI_GOALS currentGoal = AI_GOALS.STAND_BY;
    protected AI_GOALS previousGoal = AI_GOALS.STAND_BY;
    protected AI_GOALS newTriggerGoal = AI_GOALS.STAND_BY;

    private AI_GOALS stuckGoal = AI_GOALS.STAND_BY;

    private float timeToUnstuck;
    private const float MAX_STUCK_TIME = 2f;
    private float timeInCurrentGoal = 0f;

    private float jumpDuration = 0f;
    private float initialJumpTime = 0f;

    protected virtual int getLowStamiaTrigger(){return 10;}
    protected virtual float getLongRangeAttackDistance(){return 1.5f;}
    protected virtual float getMediumDistanceAttackDistance(){return 5f;}
    protected virtual float getCloseRangeAttackDistance(){return 7f;}

    protected void Start()
    {
        if(GetComponent<UserControl>().inputDevice != UserControl.InputDevice.AI)
        {
            enabled = false;
        }
        else
        {
            aiTargeting = GetComponent<AIGetClosetEnemy>();
            animCon = GetComponentInChildren<ChampionAnimationController>();
            charStats = GetComponentInChildren<CharacterStats>();
            champClassCon = GetComponent<ChampionClassController>();
        }
    }

    protected void LateUpdate()
    {
        AI_GOALS goalBefore = currentGoal;
        previousHealth = currentHealth;
        previousStamina = currentStamina;

        //If we get stunned just standby
        //TODO add right after stunned trigger
        if(animCon.statStunned)
        {
            changeCurrentAndPreviousGoal(AI_GOALS.STAND_BY, AI_GOALS.STAND_BY);
        }

        resetInputs();
        setCurrentState();

        switch (currentGoal)
        {
        case AI_GOALS.WAIT:
            if (Time.time > waitTime)setCurrentGoal();
            break;
        case AI_GOALS.MOVE_TO_LOCATION:
            setCurrentGoal();

            if (!isRetreating)moveTowardsTarget(targetPosition);
            else moveTowardsTarget(retreatSection.getRetreatPosition());
            break;
        case AI_GOALS.MOVE_THROUGH_NODES:
            
            if (!isRetreating)moveThroughSectionPath(targetSection, targetPosition);
            else moveThroughSectionPath(retreatSection, targetPosition);

            break;
        case AI_GOALS.JUMP1:
            jump1();
            break;
        case AI_GOALS.JUMP2:
            jump2();
            break;
        case AI_GOALS.FALL_THROUGH:
            fallThrough();
            break;
        case AI_GOALS.STAND_BY:
            if (isRetreating && checkStopRetreating())
            {
                isRetreating = false;
            }
            setCurrentGoal();
            break;
        default:
            break;
        }

        if(goalBefore == currentGoal)timeInCurrentGoal += Time.deltaTime;
        else timeInCurrentGoal = 0;

        //print("current goal: " + currentGoal);
    }

    private bool handleTriggerAndContinue(TRIGGER_GOALS triggerGoal)
    {
        bool continueAfter = true;

        switch (triggerGoal)
        {
        case TRIGGER_GOALS.RETREAT:
            continueAfter = false;
            isRetreating = true;
            break;
        case TRIGGER_GOALS.WAIT_FOR_ATTACK:
            setGoalWait(triggerWait);
            continueAfter = false;
            break;
        case TRIGGER_GOALS.MOVE_CLOSER:
            //Move closert ot he target
            moveTowardsTargetPlayer();
            continueAfter = false;
            break;
        case TRIGGER_GOALS.CONTINUE:
        default:
            continueAfter = true;
            break;
        }

        return continueAfter;
    }

    #region CurrentState

    protected virtual void setCurrentState()
    {
        if (champClassCon.FacingRight) facingDirection = 1;
        else facingDirection = -1;

        if (aiTargeting.TargetPlayer != null)
        {
            if (targetPlayer == null)
            {
                targetPlayer = aiTargeting.TargetPlayer;
                targetStats = targetPlayer.GetComponent<CharacterStats>();
            }

            mySection = AISectionManager.Instance.getSectionForPosition(transform.position);
            targetPosition = targetPlayer.transform.position;
            targetDirectionX = Mathf.Sign(targetPosition.x - transform.position.x); 
            targetDistance = distanceToTargetPlayer();

            yDiff = targetPosition.y - transform.position.y;

            if (!isRetreating)
            {
                targetSection = AISectionManager.Instance.getSectionForPosition(targetPosition);
            }
            else
            {
                retreatSection = AISectionManager.Instance.getRetreatSection(transform.position, targetPosition);
            }

            facingTarget = false;
            if ((facingDirection == 1 && targetPosition.x > transform.position.x) || (facingDirection == -1 && targetPosition.x < transform.position.x))
            {
                facingTarget = true;
            }
        }
        else
        {
            facingTarget = false;
            targetPlayer = null;
            targetDistance = 9999f;
        }

        isGrounded = animCon.propGrounded;
        currentHealth = charStats.CurrentHealth;
        currentStamina = charStats.CurrentStamina;
    }

    protected virtual void setCurrentGoal()
    {
        if (targetPlayer != null)
        {
            // Retreat
            if (isRetreating)
            {
                if (mySection != retreatSection)
                {
                    changeGoal(AI_GOALS.MOVE_THROUGH_NODES);
                }
                else if (mySection == retreatSection && !mySection.nonTargetable)
                {
                    changeGoal(AI_GOALS.MOVE_TO_LOCATION);
                }
                else
                {
                    changeGoal(AI_GOALS.STAND_BY);
                }
            }
            // if we aren't retreating move to and attack player
            else
            {
                setAttackingGoals();
            }
        }
        else
        {
            changeGoal(AI_GOALS.STAND_BY);
        }
    }

    protected virtual void setAttackingGoals()
    {
        //Look at health on damage
        if (currentHealth < previousHealth)
        {
            int maxHealth = charStats.maxHealth;
            int healthPercent = (int)(((float)currentHealth / (float)maxHealth) * 100);
            int prevHealthPercent = (int)(((float)previousHealth / (float)maxHealth) * 100);

            //Triggered after losing 10% health
            for (int i = 90; i > 0; i = i-10)
            {
                if (healthPercent <= i && prevHealthPercent > i)
                {
                    RaycastHit2D rightWallRay = Physics2D.Raycast(transform.position, transform.right, 999f, GameConstants.WALL_LAYER);
                    RaycastHit2D leftWallRay = Physics2D.Raycast(transform.position, transform.right * -1, 999f, GameConstants.WALL_LAYER);
                    TRIGGER_GOALS triggerGoal = healthDecreasedTenPercent(currentHealth, previousHealth, targetStats.CurrentHealth, rightWallRay, leftWallRay);

                    if(!handleTriggerAndContinue(triggerGoal))
                    {
                        return;
                    }
                }
            }
        }

        //Look to see if we have low stamina
        if (/*currentStamina < previousStamina &&*/ currentStamina < getLowStamiaTrigger())
        {
            TRIGGER_GOALS triggerGoal = lowStamina(currentStamina, targetDistance, yDiff, charStats, targetStats);
            if(!handleTriggerAndContinue(triggerGoal))
            {
                return;
            }
        }

        //Set attacking goals here
        if(mySection == targetSection && !mySection.nonTargetable)
        {
            changeGoal(AI_GOALS.MOVE_TO_LOCATION);
        }
        else if(!mySection.nonTargetable && !targetSection.nonTargetable)
        {
            changeGoal(AI_GOALS.MOVE_THROUGH_NODES);
        }
        else
        {
            changeGoal(AI_GOALS.STAND_BY);
        }
    }

    #endregion

    #region Movement

    protected virtual void moveThroughSectionPath(Section moveToSection, Vector2 moveToPosition)
    {
        if(inSameSectionAsTargetSection())
        {
            changeGoal(AI_GOALS.MOVE_TO_LOCATION);
            return;
        }

        //If we are retreating and we should stop
        if (isRetreating && checkStopRetreating())
        {
            changeGoal(AI_GOALS.STAND_BY);
            return;
        }

        if(targetPlayer != null && previousGoal != AI_GOALS.MOVE_THROUGH_NODES)
        {
            currentNodes = mySection.getOptimalPathForSection(moveToSection, transform.position, moveToPosition).Nodes;
            currentNodeIndex = 0;
            currentNode = currentNodes[currentNodeIndex];

            //since we care about previous goal make sure to set it here
            previousGoal = AI_GOALS.MOVE_THROUGH_NODES;

            if (!isRetreating)
            {
                TRIGGER_GOALS triggerGoal = lockedOnToTarget(charStats, targetStats);
                if (!handleTriggerAndContinue(triggerGoal))
                {
                    return;
                }
            }
            else
            {
                initialRetreatTime = Time.time;
            }
        }

        //Get distance to our current node
        float distFromCurrentNode = distanceFromNode(currentNode);
        if(distFromCurrentNode <= MIN_DISTANCE_TO_NODE)
        {
            //if we are close to our current node and its a jump node, JUMP
            if(currentNode.isJumpNode1)
            {
                changeGoal(AI_GOALS.JUMP1);
                return;
            }
            else if(currentNode.isFallThroughNode)
            {
                changeGoal(AI_GOALS.FALL_THROUGH);
            }
            else if(currentNodeIndex + 1 < currentNodes.Length)
            {
                incrementNodeIndex();
            }
            else
            {
                //Hit the end of our nodes
                changeGoal(AI_GOALS.STAND_BY);
                return;
            }
        }

        inputMove = Mathf.Sign(currentNode.transform.position.x - transform.position.x);
        if(!isGrounded) inputMove = inputMove/2;
    }

    protected virtual void moveTowardsTarget(Vector2 position)
    {
        //If we are retreating and we should stop
        if (isRetreating)
        {
            if (checkStopRetreating())
            {
                changeGoal(AI_GOALS.STAND_BY);
            } 
            else
            {
                moveTowardsPosition(position);
            }
        }
        else if (targetPlayer != null)
        {
            TRIGGER_GOALS distRetValue = TRIGGER_GOALS.MOVE_CLOSER;

            if (isGrounded)
            {
                if (targetDistance <= getCloseRangeAttackDistance())
                {
                    distRetValue = closeRangeAttack(facingTarget, targetDistance, yDiff, charStats, targetStats);
                } else if (targetDistance <= getMediumDistanceAttackDistance())
                {
                    distRetValue = mediumRangeAttack(facingTarget, targetDistance, yDiff, charStats, targetStats);
                } else if (targetDistance <= getLongRangeAttackDistance())
                {
                    distRetValue = longRangeAttack(facingTarget, targetDistance, yDiff, charStats, targetStats);
                }
            }

            handleTriggerAndContinue(distRetValue);
        }
    }

    protected virtual void moveTowardsPosition(Vector2 position)
    {
        float xDiff = position.x - transform.position.x;
        if (xDiff > .5f)
        {
            inputMove = Mathf.Sign(position.x - transform.position.x);
        }
    }

    protected virtual void moveTowardsTargetPlayer()
    {
        //Move closert to the target, or if we are close enough but not facing the target...
        if (targetDistance >= getCloseRangeAttackDistance() || !facingTarget )
        {
            inputMove = Mathf.Sign(targetDirectionX);
        }
    }

    #endregion

    #region Attack
    protected virtual TRIGGER_GOALS lockedOnToTarget(CharacterStats myStats, CharacterStats targetStats)
    {
        return TRIGGER_GOALS.CONTINUE;
    }

    protected virtual TRIGGER_GOALS closeRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected virtual TRIGGER_GOALS mediumRangeAttack(bool facingTarget, float distance,float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected virtual TRIGGER_GOALS longRangeAttack(bool facingTarget, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats)
    {
        return TRIGGER_GOALS.MOVE_CLOSER;
    }
    #endregion

    #region StatsChanged
    public virtual TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth, RaycastHit2D rightWallRay, RaycastHit2D leftWallRay){
        return TRIGGER_GOALS.CONTINUE;
    }

    public virtual TRIGGER_GOALS lowStamina(int currentStamina, float distance, float yDiff, CharacterStats myStats, CharacterStats targetStats){
        return TRIGGER_GOALS.CONTINUE;
    }

    //TODO right after stunned, get number of players around me and distances

    #endregion

    #region FallThrough

    protected virtual void fallThrough()
    {
        if(inSameSectionAsTargetSection())
        {
            changeGoal(AI_GOALS.MOVE_TO_LOCATION);
            return;
        }

        //wait to be grounded to start this goal
        if(previousGoal != AI_GOALS.FALL_THROUGH && !isGrounded)
        {
            changeCurrentAndPreviousGoal(AI_GOALS.FALL_THROUGH, AI_GOALS.STAND_BY);
            return;
        }

        if(isGrounded && previousGoal != AI_GOALS.FALL_THROUGH)
        {
            StartCoroutine(inputDashForFall());
        }

        inputDown = true;
    }

    protected IEnumerator inputDashForFall()
    {
        inputDashDirection = 1;
        yield return new WaitForSeconds(.2f);
        inputDashDirection = 0;
        StartCoroutine(fallThroughRoutine());
    }

    protected IEnumerator fallThroughRoutine()
    {
        yield return new WaitUntil(() => ( isGrounded ));
        if(currentGoal == AI_GOALS.FALL_THROUGH)
        {
            incrementNodeIndex();
            changeCurrentAndPreviousGoal(AI_GOALS.MOVE_THROUGH_NODES, AI_GOALS.MOVE_THROUGH_NODES);
        }
    }

    #endregion

    #region Jump

    protected virtual void jump1()
    {
        if(inSameSectionAsTargetSection() && isGrounded)
        {
            changeGoal(AI_GOALS.MOVE_TO_LOCATION);
            return;
        }

        //Were grounded but dont have enough stamina
        if(isGrounded && currentStamina < champClassCon.stamina_Jump + 5 && Time.time < initialJumpTime)
        {
            changeGoal(AI_GOALS.STAND_BY);
            return;
        }

        if(isGrounded)
        {
            if(previousGoal != AI_GOALS.JUMP1)
            {
                //if we are double jumping... start coroutine
                if(currentNode.isJumpNode2)
                {
                    jumpDuration = Time.time + currentNode.doubleJumpWait - .1f;
                    StartCoroutine(doubleJump(currentNode.doubleJumpWait));
                }
                else
                {
                    jumpDuration = Time.time + currentNode.jumpDuration1;
                }

                previousGoal = AI_GOALS.JUMP1;

                initialJumpTime = Time.time;
                inputJump = true;
                inputMove = currentNode.jumpForce1X;
            }
            else if(!currentNode.isJumpNode2 && previousGoal == AI_GOALS.JUMP1)
            {
                //No double jump we are grounded checkout our distance to next node
                float distFromNextNode = distanceFromNode(currentNodes[currentNodeIndex + 1]);
                if(distFromNextNode <= MIN_DISTANCE_TO_NODE)
                {
                    changeCurrentAndPreviousGoal(AI_GOALS.MOVE_THROUGH_NODES, AI_GOALS.MOVE_THROUGH_NODES);
                    incrementNodeIndex();
                    return;
                }

                if(timeInCurrentGoal > 2)
                {
                    //We tried to jump 2 seconds ago and we are still grounded
                    changeGoal(AI_GOALS.STAND_BY);
                }
            }
        }
        else
        {
            if(Time.time > jumpDuration)
            {
                inputJump = false;
            }
            //We are in the air, just apply direction
            inputMove = currentNode.jumpForce1X;
        }
    }

    protected IEnumerator doubleJump(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        yield return new WaitUntil(()=> !inputJump);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if(currentGoal != AI_GOALS.JUMP2)
        {
            changeCurrentAndPreviousGoal(AI_GOALS.JUMP2, AI_GOALS.JUMP1);
        }
    }

    protected virtual void jump2()
    {
        if(inSameSectionAsTargetSection() && isGrounded)
        {
            changeGoal(AI_GOALS.MOVE_TO_LOCATION);
            return;
        }

        if(isGrounded)
        {
            float distFromNextNode = distanceFromNode(currentNodes[currentNodeIndex + 1]);
            if(distFromNextNode <= MIN_DISTANCE_TO_NODE)
            {
                changeCurrentAndPreviousGoal(AI_GOALS.MOVE_THROUGH_NODES, AI_GOALS.MOVE_THROUGH_NODES);
                incrementNodeIndex();
            }
            //Second jump failed
            else
            {
                changeGoal(AI_GOALS.STAND_BY);
            }
        }
        else if(previousGoal == AI_GOALS.JUMP1)
        {
            previousGoal = AI_GOALS.JUMP2;
            jumpDuration = Time.time + currentNode.jumpDuration2;
        }

        if(Time.time < jumpDuration)
        {
            inputJump = true;
        }

        inputMove = currentNode.jumpForce2X;
    }

    #endregion

    #region Helper Methods
    /// <summary>
    /// Call this method in certain goals if you feel like they might not succeed etc.
    /// </summary>
    private void stuckTimer()
    {
        if(currentGoal == AI_GOALS.STAND_BY) return;

        if(stuckGoal != currentGoal)
        {
            stuckGoal = currentGoal;
            timeToUnstuck = Time.time + MAX_STUCK_TIME;
        }

        if(Time.time > timeToUnstuck)
        {
            stuckGoal = AI_GOALS.STAND_BY;
            currentGoal = AI_GOALS.STAND_BY;
        }
    }

    private bool checkStopRetreating()
    {
        return currentStamina >= retreatUntilStamina || Time.time > initialRetreatTime + retreatDuration;
    }

    private void resetStuck()
    {
        timeToUnstuck = Time.time + MAX_STUCK_TIME;
    }

    private bool inSameSectionAsTargetSection()
    {
        if (!isRetreating)
        {
            return targetPlayer != null && mySection == targetSection && !mySection.nonTargetable;
        } 
        else
        {
            return targetPlayer != null && mySection == retreatSection && !mySection.nonTargetable;
        }
    }

    private void changeGoal(AI_GOALS newGoal)
    {
        this.previousGoal = currentGoal;
        this.currentGoal = newGoal;
    }

    private void changeCurrentAndPreviousGoal(AI_GOALS currentGoal, AI_GOALS previousGoal)
    {
        this.currentGoal = currentGoal;
        this.previousGoal = previousGoal;
    }

    private void incrementNodeIndex()
    {
        currentNodeIndex++;
        if(currentNodes == null || currentNodes.Length == currentNodeIndex)
        {
            Debug.LogError("Cannot increment nodes here...");
        }
        else
        {
            currentNode = currentNodes[currentNodeIndex];
        }
    }

    private float distanceFromNode(SectionPathNode node)
    {
        return Mathf.Abs(Vector2.Distance(node.transform.position, transform.position));
    }

    private float distanceXFromNode(SectionPathNode node)
    {
        return Mathf.Abs(node.transform.position.x - transform.position.x);
    }

    private float distanceToTargetPlayer()
    {
        return Mathf.Abs(Vector2.Distance(targetPosition, transform.position));
    }

    private void setGoalWait(float timeToWait)
    {
        waitTime = Time.time + timeToWait;
        changeGoal(AI_GOALS.WAIT);
    }

    private void resetInputs()
    {
        if(currentGoal != AI_GOALS.FALL_THROUGH)
        {
            inputDashDirection = 0;
            inputDown = false;
        }

        if( currentGoal != AI_GOALS.JUMP1)
        {
            inputJump = false;
        }

        inputAttack = false;
        inputMove = 0;
        inputSkill1 = false;
        inputSkill2 = false;
        inputSkill3 = false;
        inputSkill4 = false;
        inputTrinket1 = false;
        inputTrinket2 = false;
    }

    #endregion
}
