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

    protected GameObject targetPlayer; 

    protected float targetDirectionX = 0;
    protected float targetDirectionY = 0;           //can be 1 (above), 0 (same), or -1(below)
    protected float yDiff = 0;
    protected float jumpDifference;

    protected Section mySection;
    protected Section targetSection;

    protected SectionPath currentSectionPath;
    protected SectionPathNode currentNode;
    protected int currentNodeIndex = 0;

    public enum AI_GOALS { MOVE_TO_PLAYER, MOVE_THROUGH_NODES, STAND_BY };
    public AI_GOALS currentGoal = AI_GOALS.MOVE_TO_PLAYER;
    public AI_GOALS previousGoal = AI_GOALS.STAND_BY;
        
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

            //set jump difference to the size of our box collider
            jumpDifference = GetComponent<BoxCollider2D>().size.y;
        }

    }

    protected void LateUpdate()
    {
        resetInputs();
        setCurrentGoal();

        switch(currentGoal)
        {
        case AI_GOALS.MOVE_TO_PLAYER:
            moveTowardsTargetPlayer();
            break;
        case AI_GOALS.MOVE_THROUGH_NODES:
            moveThroughSectionPath();
            break;
        case AI_GOALS.STAND_BY:
            break;
        default:
            break;
        }

        previousGoal = currentGoal;
    }

    protected virtual void setCurrentGoal()
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

            //Set goals here
            if(mySection == targetSection && !mySection.nonTargetable && !targetSection.nonTargetable)
            {
                currentGoal = AI_GOALS.MOVE_TO_PLAYER;
            }
            else if(!mySection.nonTargetable && !targetSection.nonTargetable)
            {
                currentGoal = AI_GOALS.MOVE_THROUGH_NODES;
            }
            else
            {
                currentGoal = AI_GOALS.STAND_BY;
            }
        }
        else
        {
            currentGoal = AI_GOALS.STAND_BY;
        }
    }

    protected virtual void moveTowardsTargetPlayer()
    {
        if(targetPlayer != null)
        {
            if(animCon.m_Grounded)
            {
                inputMove = Mathf.Sign(targetDirectionX);
            }
        }
    }

    protected virtual void moveThroughSectionPath()
    {
        if(targetPlayer != null && previousGoal != AI_GOALS.MOVE_THROUGH_NODES)
        {
            currentSectionPath = mySection.getPathForTargetSection(targetSection);
            currentNodeIndex = 0;
            currentNode = currentSectionPath.Nodes[currentNodeIndex];
        }

        float distFromCurrentNode = Mathf.Abs(Vector2.Distance(currentNode.transform.position, transform.position));
        if(distFromCurrentNode <= .1f)
        {
            if(currentNodeIndex + 1 < currentSectionPath.Nodes.Length)
            {
                currentNodeIndex++;
                currentNode = currentSectionPath.Nodes[currentNodeIndex];
            }
            else
            {
                //Hit the end of our nodes
                currentGoal = AI_GOALS.STAND_BY;
                return;
            }
        }

        inputMove = Mathf.Sign(currentNode.transform.position.x - transform.position.x);
    }
        

    private void resetInputs()
    {
        inputJump = false;
        inputAttack = false;
        inputDashDirection = 0;
        inputDown = false;
        inputMove = 0;
        inputSkill1 = false;
        inputSkill2 = false;
        inputSkill3 = false;
        inputSkill4 = false;
        inputTrinket1 = false;
        inputTrinket2 = false;
    }
}
