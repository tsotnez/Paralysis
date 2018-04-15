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
    protected Transform myTransform;
    protected Transform targetTransform;
    protected Vector2 targetPosition;

    protected int currentHealth;
    protected int previousHealth;
    protected int currentStamina;
    protected int previousStamina;

    /// <summary>
    /// The targets direction relative to you, -1 left 1 right
    /// </summary>
    protected float targetDirectionX = 0;
    /// <summary>
    /// Never negative
    /// </summary>
    protected float targetDistance = 0f;
    protected float facingDirection = 1;
    /// <summary>
    /// The difference in y between the target and you
    /// </summary>
    protected float yDiff = 0;
    protected bool isGrounded;
    protected bool facingTarget = false;
    protected bool targetStunned = false;

    protected bool isRetreating = false;

    private float retreatWaitStamina = 50f;
    private float retreatDuration = 0f;

    protected Section mySection;
    protected Section targetSection;

    protected SectionPathNode[] currentNodes;
    protected SectionPathNode currentNode;
    protected int currentNodeIndex = 0;

    public enum AI_GOALS { MOVE_TO_LOCATION, MOVE_THROUGH_NODES, JUMP1, JUMP2, FALL_THROUGH, STAND_BY, WAIT };
    protected float waitTime;

    public enum TRIGGER_GOALS { MOVE_CLOSER, WAIT_FOR_INPUT, RETREAT, CONTINUE }

    protected AI_GOALS distRetNewGoal;

    protected float triggerWait { get; set; }

    protected AI_GOALS currentGoal = AI_GOALS.STAND_BY;
    protected AI_GOALS previousGoal = AI_GOALS.STAND_BY;
    protected AI_GOALS newTriggerGoal = AI_GOALS.STAND_BY;

    private AI_GOALS stuckGoal = AI_GOALS.STAND_BY;

    private float timeToUnstuck;
    private const float MAX_STUCK_TIME = 2f;
    private float timeInCurrentGoal = 0f;

    private float jumpDuration = 0f;
    private float initialJumpTime = 0f;

    //Overrides
    protected virtual float enemyTooCloseDistance(){return float.MinValue;}
    protected virtual float getMinDistanceToNode() {return .115f;}
    protected virtual int getLowStamiaTrigger(){return champClassCon.stamina_Jump + 5;}
    protected virtual int getMediumStamiaTrigger(){return 50;}
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

            myTransform = transform.Find(GameConstants.EFFECT_OVERLAY).transform;
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
            if (!isRetreating)
            {
                moveTowardsTarget(targetPosition);
            }
            else
            {
                moveTowardsTarget(targetSection.getBestRetreatPoint(myTransform.position, targetPosition));
            }
            break;
        case AI_GOALS.MOVE_THROUGH_NODES:
            moveThroughSectionPath(targetSection, targetPosition);
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
            setCurrentGoal();
            break;
        default:
            break;
        }

        if(goalBefore == currentGoal)timeInCurrentGoal += Time.deltaTime;
        else timeInCurrentGoal = 0;

        //TODO remove, this is for testing...
        if (Input.GetKey(KeyCode.L))
        {
            retreatUntilStamina(90);
        }

        //print("current goal: " + currentGoal + " retreating:" + isRetreating);
    }

    private bool handleTriggerAndContinue(TRIGGER_GOALS triggerGoal)
    {
        bool continueAfter = true;

        switch (triggerGoal)
        {
        case TRIGGER_GOALS.RETREAT:
            continueAfter = false;
            break;
        case TRIGGER_GOALS.WAIT_FOR_INPUT:
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
                targetTransform = targetPlayer.transform.Find(GameConstants.EFFECT_OVERLAY).transform;
            }

            mySection = AISectionManager.Instance.getSectionForPosition(myTransform.position);
            targetPosition = targetTransform.position;

            targetDirectionX = Mathf.Sign(targetPosition.x - myTransform.position.x); 
            targetDistance = distanceToTargetPlayer();
            targetStunned = targetStats.stunned;

            yDiff = targetPosition.y - myTransform.position.y;

            if (!isRetreating)
            {
                targetSection = AISectionManager.Instance.getSectionForPosition(targetPosition);
            }
            else
            {
                targetSection = AISectionManager.Instance.getRetreatSection(myTransform.position, targetPosition);
            }

            facingTarget = false;
            if ((facingDirection == 1 && targetPosition.x > myTransform.position.x) ||
                (facingDirection == -1 && targetPosition.x < myTransform.position.x))
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

    #endregion

    #region SetGoals
    protected virtual void setCurrentGoal()
    {
        checkStopRetreating();
   
        if (targetPlayer != null)
        {
            if (!checkHealthTriggerContinue())
            {
                return;
            }

            // Retreat
            if (isRetreating)
            {
                setRetreatingGoals();
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

    protected virtual void setRetreatingGoals()
    {
        if (mySection == null || targetSection == null)
        {
            changeGoal(AI_GOALS.STAND_BY);
            return;
        }


        if (!mySection.nonTargetable && !targetSection.nonTargetable)
        {
            if (mySection != targetSection)
            {
                changeGoal(AI_GOALS.MOVE_THROUGH_NODES);
            }
            else if (mySection == targetSection)
            {
                changeGoal(AI_GOALS.MOVE_TO_LOCATION);
            }
        }
        else
        {
            changeGoal(AI_GOALS.STAND_BY);
        }
    }

    protected virtual void setAttackingGoals()
    {
        if (handleAttackingTriggers())
        {
            return;
        }

        if (mySection == null || targetSection == null)
        {
            changeGoal(AI_GOALS.STAND_BY);
            return;
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

    public bool handleAttackingTriggers()
    {
        //Look to see if we have low stamina
        if (currentStamina < getLowStamiaTrigger())
        {
            TRIGGER_GOALS triggerGoal = lowStaminaAttacking();
            if (!handleTriggerAndContinue(triggerGoal))
            {
                return true;
            }
        }
        else if (currentStamina < getMediumStamiaTrigger())
        {
            TRIGGER_GOALS triggerGoal = mediumStaminaAttacking();
            if (!handleTriggerAndContinue(triggerGoal))
            {
                return true;
            }
        }

        if (targetDistance <= enemyTooCloseDistance())
        {
            TRIGGER_GOALS triggerGoal = enemyTooCloseAttacking();
            if (!handleTriggerAndContinue(triggerGoal))
            {
                return true;
            }
        }

        return false;
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
        if (checkStopRetreating())
        {
            changeGoal(AI_GOALS.STAND_BY);
            return;
        }

        //This is only called the first timet his goal happens
        if(targetPlayer != null && previousGoal != AI_GOALS.MOVE_THROUGH_NODES)
        {
            SectionPath path = mySection.getOptimalPathForSection(moveToSection, myTransform.position, moveToPosition);
            if (path == null || path.Nodes == null)
            {
                Debug.LogError("path was null no nodes...");
                changeGoal(AI_GOALS.STAND_BY);
                return;
            }
            currentNodes = path.Nodes;

            currentNodeIndex = 0;
            currentNode = currentNodes[currentNodeIndex];

            //since we care about previous goal make sure to set it here
            previousGoal = AI_GOALS.MOVE_THROUGH_NODES;

            if (!isRetreating)
            {
                TRIGGER_GOALS triggerGoal = lockedOnToTarget();
                if (!handleTriggerAndContinue(triggerGoal))
                {
                    return;
                }
            }
        }

        //Get distance to our current node
        float distFromCurrentNode = distanceFromNode(currentNode);
        if(distFromCurrentNode <= getMinDistanceToNode())
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

        inputMove = Mathf.Sign(currentNode.transform.position.x - myTransform.position.x);
        if(!isGrounded) inputMove = inputMove/2;
    }

    protected virtual void moveTowardsTarget(Vector2 position)
    {
        //If we are retreating
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
        //We have a target, should we attack or move to target?
        else if (targetPlayer != null)
        {
            TRIGGER_GOALS distRetValue = TRIGGER_GOALS.MOVE_CLOSER;

            if (isGrounded)
            {
                if (targetDistance <= getCloseRangeAttackDistance())
                {
                    distRetValue = closeRangeAttack();
                } else if (targetDistance <= getMediumDistanceAttackDistance())
                {
                    distRetValue = mediumRangeAttack();
                } else if (targetDistance <= getLongRangeAttackDistance())
                {
                    distRetValue = longRangeAttack();
                }
            }

            handleTriggerAndContinue(distRetValue);
        }
    }

    protected virtual void moveTowardsPosition(Vector2 position)
    {
        if (distanceToPosition(position) > getMinDistanceToNode())
        {
            inputMove = Mathf.Sign(position.x - myTransform.position.x);
        }
    }

    protected virtual void moveTowardsTargetPlayer()
    {
        //Move closert to the target, or if we are close enough but not facing the target...
        if (targetDistance >= getCloseRangeAttackDistance() || ( !facingTarget && Mathf.Abs(yDiff) < 1f ) )
        {
            inputMove = Mathf.Sign(targetDirectionX);
        }
    }

    #endregion

    #region Attack
    protected virtual TRIGGER_GOALS lockedOnToTarget()
    {
        return TRIGGER_GOALS.CONTINUE;
    }

    protected virtual TRIGGER_GOALS closeRangeAttack()
    {
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected virtual TRIGGER_GOALS mediumRangeAttack()
    {
        return TRIGGER_GOALS.MOVE_CLOSER;
    }

    protected virtual TRIGGER_GOALS longRangeAttack()
    {
        return TRIGGER_GOALS.MOVE_CLOSER;
    }
    #endregion

    #region StatsChanged

    private bool checkHealthTriggerContinue()
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
                    TRIGGER_GOALS triggerGoal = healthDecreasedTenPercent(currentHealth, previousHealth, targetStats.CurrentHealth, isRetreating);
                    if(!handleTriggerAndContinue(triggerGoal))
                    {
                        return false;
                    }
                }
            }
        }

        return true;

    }

    public virtual TRIGGER_GOALS healthDecreasedTenPercent(int oldHealth, int newHealth, int targetHealth, bool retreating){
        return TRIGGER_GOALS.CONTINUE;
    }

    public virtual TRIGGER_GOALS enemyTooCloseAttacking(){
        return TRIGGER_GOALS.CONTINUE;
    }

    public virtual TRIGGER_GOALS lowStaminaAttacking(){
        return TRIGGER_GOALS.CONTINUE;
    }

    public virtual TRIGGER_GOALS mediumStaminaAttacking(){
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

        inputDown = true;
        if(isGrounded && previousGoal != AI_GOALS.FALL_THROUGH)
        {
            StartCoroutine(inputDashForFall());
        }
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
                if(distFromNextNode <= getMinDistanceToNode())
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
            if(distFromNextNode <= getMinDistanceToNode())
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
        if (isRetreating)
        {
            if (currentStamina >= retreatWaitStamina || Time.time > retreatDuration)
            {
                isRetreating = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected bool canPerformAttack(bool grounded)
    {
        return champClassCon.CanPerformAction(grounded) && champClassCon.CanPerformAttack();
    }

    private void resetStuck()
    {
        timeToUnstuck = Time.time + MAX_STUCK_TIME;
    }

    private bool inSameSectionAsTargetSection()
    {
        return targetPlayer != null && mySection == targetSection && !mySection.nonTargetable;
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
        return Mathf.Abs(Vector2.Distance(node.transform.position, myTransform.position));
    }

    private float distanceXFromNode(SectionPathNode node)
    {
        return Mathf.Abs(node.transform.position.x - myTransform.position.x);
    }

    private float distanceToPosition(Vector2 position)
    {
        return Mathf.Abs(Vector2.Distance(position, myTransform.position));
    }

    private float distanceToTargetPlayer()
    {
        return Mathf.Abs(Vector2.Distance(targetPosition, myTransform.position));
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

    public TRIGGER_GOALS retreatForDuration(float duration)
    {
        if (isRetreating) return TRIGGER_GOALS.CONTINUE;
        isRetreating = true;
        retreatWaitStamina = 999999;
        retreatDuration = Time.time + duration;
        return TRIGGER_GOALS.RETREAT;
    }

    public TRIGGER_GOALS retreatUntilStamina(int stamina)
    {
        if (isRetreating) return TRIGGER_GOALS.CONTINUE;
        isRetreating = true;
        retreatWaitStamina = stamina;
        retreatDuration = 999999;
        return TRIGGER_GOALS.RETREAT;
    }

    public RaycastHit2D[] getRightLeftWallRays()
    {
        RaycastHit2D[] rayCasts = new RaycastHit2D[2];
        rayCasts[0] = Physics2D.Raycast(myTransform.position, transform.right, 999f, GameConstants.WALL_LAYER);
        rayCasts[1] = Physics2D.Raycast(myTransform.position, transform.right * -1, 999f, GameConstants.WALL_LAYER);
        return rayCasts;
    }

    public RaycastHit2D[] getTopBottomWallRays()
    {
        RaycastHit2D[] rayCasts = new RaycastHit2D[2];
        rayCasts[0] = Physics2D.Raycast(myTransform.position, transform.up, 999f, GameConstants.WALL_LAYER);
        rayCasts[1] = Physics2D.Raycast(myTransform.position, transform.up * -1, 999f, GameConstants.WALL_LAYER);
        return rayCasts;
    }

    public DODGE_DIR getDodgeDirection(float maxWallDistance = 15)
    {
        RaycastHit2D[] rayCasts = getRightLeftWallRays();

        float rightWall = rayCasts [0].distance;
        float leftWall = rayCasts[1].distance;
        float absWallDiff = Mathf.Abs(rightWall - leftWall);

        if (rightWall > maxWallDistance) rightWall = maxWallDistance;
        if (leftWall > maxWallDistance) leftWall = maxWallDistance;


        bool targetOnRight = false;
        bool goLeft = false;
        bool goRight = false;

        //target on right, not left
        if ((facingTarget && facingDirection == 1) || (!facingTarget && facingDirection == -1))
        {
            targetOnRight = true;
        }

        //go left since we have more space, targets too close
        if (absWallDiff > 3 && leftWall > rightWall && targetDistance <= 2)
            goLeft = true;
        //go right since we have more space there, targets too close
        else if (absWallDiff > 3 && rightWall > leftWall && targetDistance <= 2)
            goRight = true;
        //go left we have more space and target is to our right
        else if (leftWall > rightWall && targetOnRight)
            goLeft = true;
        //go right we have more space and target is to our left
        else if (rightWall > leftWall && !targetOnRight)
            goRight = true;
        //If we have equal amount of space go whichever way the target isnt
        else if (rightWall == leftWall)
        {
            if (targetOnRight)
            {
                goLeft = true;
            } 
            else
            {
                goRight = true;
            }
        }
        else
        {
            //otherwise just go the opposite dir of target
            goRight = !targetOnRight;
            goLeft = targetOnRight;
        }

        DODGE_DIR dodgeDir;
        dodgeDir.goRight = goRight;
        dodgeDir.goLeft = goLeft;

        return dodgeDir;
    }

    protected void inputDash(bool goRight, bool goLeft)
    {
        if (goRight && goLeft)
        {
            if (Random.Range(1, 100) >= 50)
            {
                inputDashDirection = 1;
            } 
            else
            {
                inputDashDirection = -1;
            }
        }
        else if (goRight)
        {
            inputDashDirection = 1;
        }
        else
        {
            inputDashDirection = -1;
        }
    }


    public struct DODGE_DIR {
        public bool goRight;
        public bool goLeft;

        //TODO, alchemist?
        //public bool goUp;
        //public bool goDown;
    }

    #endregion

}
