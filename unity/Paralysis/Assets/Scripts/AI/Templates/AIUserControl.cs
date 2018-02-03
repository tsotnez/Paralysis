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
    protected CharacterStats charStats;

    protected GameObject targetPlayer; 

    protected float targetDirectionX = 0;
    protected float targetDirectionY = 0;           //can be 1 (above), 0 (same), or -1(below)
    protected float yDiff = 0;
    protected float jumpDifference;

    protected Section mySection;
    protected Section targetSection;

    protected SectionPathNode[] currentNodes;
    protected SectionPathNode currentNode;
    protected int currentNodeIndex = 0;
    private const float MIN_DISTANCE_TO_NODE = .1f;


    public enum AI_GOALS { MOVE_TO_PLAYER, MOVE_THROUGH_NODES, JUMP1, JUMP2, FALL_THROUGH, STAND_BY };

    protected AI_GOALS currentGoal = AI_GOALS.STAND_BY;
    protected AI_GOALS previousGoal = AI_GOALS.STAND_BY;

    private float timeToUnstuck;
    private const float MAX_STUCK_TIME = .5f;
    private AI_GOALS stuckGoal = AI_GOALS.STAND_BY;

        
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

            //set jump difference to the size of our box collider
            jumpDifference = GetComponent<BoxCollider2D>().size.y;
        }

    }

    protected void LateUpdate()
    {
        //If we get stunned just standby
        if(animCon.statStunned)
        {
            changeCurrentAndPreviousGoal(AI_GOALS.STAND_BY, AI_GOALS.STAND_BY);
        }

        resetInputs();
        setCurrentState();

        switch(currentGoal)
        {
        case AI_GOALS.MOVE_TO_PLAYER:
            setCurrentGoal();
            moveTowardsTargetPlayer();
            break;
        case AI_GOALS.MOVE_THROUGH_NODES:
            moveThroughSectionPath();
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

        //print("Current goal: " + currentGoal);
    }

    protected virtual void setCurrentState()
    {
        if(aiTargeting.TargetPlayer != null)
        {
            targetPlayer = aiTargeting.TargetPlayer;
            mySection = AISectionManager.Instance.getSectionForPosition(transform.position);
            targetSection = AISectionManager.Instance.getSectionForPosition(targetPlayer.transform.position);
            targetDirectionX = Mathf.Sign(targetPlayer.transform.position.x - transform.position.x); 

            yDiff = targetPlayer.transform.position.y - transform.position.y;
            if(Mathf.Abs(yDiff) < 0.1)
            {
                targetDirectionY = 0;
            }
            else
            {
                targetDirectionY = Mathf.Sign(targetPlayer.transform.position.y - transform.position.y);
            }
        }
    }

    protected virtual void setCurrentGoal()
    {
        if(targetPlayer != null)
        {
            //Set goals here
            if(mySection == targetSection && !mySection.nonTargetable)
            {
                changeGoal(AI_GOALS.MOVE_TO_PLAYER);
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
        else
        {
            changeGoal(AI_GOALS.STAND_BY);
        }
    }

    protected virtual void moveTowardsTargetPlayer()
    {
        if(targetPlayer != null  && distanceToTargetPlayer() > 2f)
        {
            inputMove = Mathf.Sign(targetDirectionX);
        }
    }

    protected virtual void moveThroughSectionPath()
    {
        if(inSameSectionAsTarget())
        {
            changeGoal(AI_GOALS.MOVE_TO_PLAYER);
            return;
        }

        if(targetPlayer != null && previousGoal != AI_GOALS.MOVE_THROUGH_NODES)
        {
            currentNodes = mySection.getOptimalPathForSection(targetSection, transform.position, 
                targetPlayer.transform.position).Nodes;
            currentNodeIndex = 0;
            currentNode = currentNodes[currentNodeIndex];

            //since we care about previous goal make sure to set it here
            previousGoal = AI_GOALS.MOVE_THROUGH_NODES;
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
        if(!animCon.m_Grounded) inputMove = inputMove/2;
    }

    #region FallThrough

    protected virtual void fallThrough()
    {
        if(inSameSectionAsTarget())
        {
            changeGoal(AI_GOALS.MOVE_TO_PLAYER);
            return;
        }

        //wait to be grounded to start this goal
        if(previousGoal != AI_GOALS.FALL_THROUGH && !animCon.m_Grounded)
        {
            changeCurrentAndPreviousGoal(AI_GOALS.FALL_THROUGH, AI_GOALS.STAND_BY);
            return;
        }

        if(animCon.m_Grounded && previousGoal != AI_GOALS.FALL_THROUGH)
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
        yield return new WaitUntil(() => ( animCon.m_Grounded));
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
        if(inSameSectionAsTarget() && animCon.m_Grounded)
        {
            changeGoal(AI_GOALS.MOVE_TO_PLAYER);
            return;
        }

        if(charStats.CurrentStamina < ChampionClassController.JUMP_STAMINA_REQ)
        {
            changeGoal(AI_GOALS.STAND_BY);
            return;
        }

        if(animCon.m_Grounded)
        {
            if(previousGoal != AI_GOALS.JUMP1)
            {
                //if we are double jumping... start coroutine
                if(currentNode.isJumpNode2)
                {
                    StartCoroutine(doubleJump(currentNode.doubleJumpWait));
                }
                previousGoal = AI_GOALS.JUMP1;

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
                else
                {
                    //Start stuck timer here, our jump can fail
                    stuckTimer();
                }
            }
        }
        else
        {
            //We are in the air, just apply direction
            inputMove = currentNode.jumpForce1X;
            resetStuck();
        }
    }

    protected IEnumerator doubleJump(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(currentGoal != AI_GOALS.JUMP2)
        {
            changeCurrentAndPreviousGoal(AI_GOALS.JUMP2, AI_GOALS.JUMP1);
        }
    }

    protected virtual void jump2()
    {
        if(inSameSectionAsTarget() && animCon.m_Grounded)
        {
            changeGoal(AI_GOALS.MOVE_TO_PLAYER);
            return;
        }

        if(animCon.m_Grounded)
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

    private void resetStuck()
    {
        timeToUnstuck = Time.time + MAX_STUCK_TIME;
    }

    private bool inSameSectionAsTarget()
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
        return Mathf.Abs(Vector2.Distance(node.transform.position, transform.position));
    }

    private float distanceXFromNode(SectionPathNode node)
    {
        return Mathf.Abs(node.transform.position.x - transform.position.x);
    }

    private float distanceToTargetPlayer()
    {
        return Mathf.Abs(Vector2.Distance(targetPlayer.transform.position, transform.position));
    }

    private void resetInputs()
    {
        if(currentGoal != AI_GOALS.FALL_THROUGH)
        {
            inputDashDirection = 0;
            inputDown = false;
        }

        inputJump = false;
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
