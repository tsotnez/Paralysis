using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerTargeting : MonoBehaviour {

    private LayerMask enemyLayerMask;

    private bool hasCoords = false;
    public bool HasCoords { get { return hasCoords; } }

    private Vector3 coords;
    public Vector3 Coords { get { return coords; } }

    private List<CharacterStats> enemyPlayers;
    private CharacterStats closestPlayer;
    private CharacterStats closestPlayerPath;
    private Vector3[] currentNodeList;
    private int currentNodeIndex = 0;
    private List<PlayerWeCantSee> playerInfos;

    private const float RECAL_RATE = 1f;
    private const float RAY_LENGTH = 200;

    private float minDistanceToNode;
    public float MinDistance { get { return minDistanceToNode; } }

    private bool requestingPath = false;
    private float timeForNextRequest = 0f;

    private void Start()
    {
        if(GetComponent<UserControl>().inputDevice != UserControl.InputDevice.AI)
        {
            enabled = false;
        }
        else 
        {
            if(gameObject.layer == GameConstants.TEAM_1_LAYER)
                enemyLayerMask = GameConstants.TEAM_2_LAYER;
            else
                enemyLayerMask = GameConstants.TEAM_1_LAYER;
            
            minDistanceToNode = GridManager.NODE_RADIUS;
            enemyPlayers = new List<CharacterStats>();
        }
    }

    private void setPlayerList()
    {
        if(enemyPlayers != null)
        {
            enemyPlayers.Clear();
        }

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag(GameConstants.MAIN_PLAYER_TAG);
        foreach(GameObject playerObj in playerObjects)
        {            
            if(playerObj != gameObject && playerObj.layer == enemyLayerMask)
            {
                enemyPlayers.Add(playerObj.GetComponent<CharacterStats>());
            }
        }
            
    }

    void LateUpdate ()
    {   
        hasCoords = false;
        if (enemyPlayers == null || enemyPlayers.Count == 0)
        {
            setPlayerList();
            return;
        }

        if (enemyPlayers != null)
        {
            if(findClosestPlayerWeCanSee())
            {
                hasCoords = true;
                coords = closestPlayer.transform.position;
                resetNodePath();
            }
            else
            {
                checkRequestPath();
                handleCurrentNodeList();
            }
        }
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        requestingPath = false;
        if (pathSuccessful)
        {
            currentNodeList = waypoints;
            currentNodeIndex = 0;
        }
    }

    private bool findClosestPlayerWeCanSee()
    {
        float actDistance = float.MaxValue;
        closestPlayer = null;
        playerInfos = new List<PlayerWeCantSee>();
        bool canSeePlayer = false;

        bool retVal = false;
        foreach (CharacterStats enemy in enemyPlayers)
        {
            canSeePlayer = false;
            if (!enemy.CharacterDied)
            {
                //check if we cans see the player
                Vector3 direction = enemy.transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, RAY_LENGTH, enemyLayerMask);
                if (hit && hit.collider.tag == GameConstants.MAIN_PLAYER_TAG)
                {
                    canSeePlayer = true;
                    retVal = true;
                }

                //get the distance
                float distance = Mathf.Abs(Vector3.Distance(enemy.transform.position, transform.position));

                if (canSeePlayer && distance < actDistance)
                {
                    actDistance = distance;
                    closestPlayer = enemy;
                }
                else if (!canSeePlayer)
                {
                    PlayerWeCantSee pInfo = new PlayerWeCantSee();
                    pInfo.player = enemy;
                    pInfo.distance = distance;
                    playerInfos.Add(pInfo);
                }
            }
        }
        return retVal;
    }

    private void checkRequestPath()
    {
        if (!requestingPath && Time.time > timeForNextRequest)
        {
            calculateClosetPlayerPath();
            if (closestPlayerPath != null)
            {
                PathRequestHandler.RequestPath(new PathRequest(transform.position, closestPlayerPath.transform.position, OnPathFound));
                requestingPath = true;
                timeForNextRequest = Time.time + RECAL_RATE;
            }
        }
    }

    private void handleCurrentNodeList()
    {
        if(currentNodeList != null)
        {
            float distanceToCurrentNode = Mathf.Abs(Vector2.Distance(transform.position, currentNodeList[currentNodeIndex]));
            if (distanceToCurrentNode <= minDistanceToNode * 2)
            {
                if (currentNodeIndex + 1 < currentNodeList.Length)
                {
                    currentNodeIndex++;
                }
                else
                {
                    //we hit the end of our path... recalc path..
                    timeForNextRequest = 0;
                }
            }
            hasCoords = true;
            coords = currentNodeList[currentNodeIndex];
        }
    }

    private void calculateClosetPlayerPath()
    {
        //Get the actual closest player ignoring line of sight
        float actDistance = float.MaxValue;
        closestPlayerPath = null;
        foreach (PlayerWeCantSee pInfo in playerInfos)
        {
            if (pInfo.distance < actDistance)
            {
                actDistance = pInfo.distance;
                closestPlayerPath = pInfo.player;
            }
        }
    }

    private void resetNodePath()
    {
        currentNodeList = null;
        currentNodeIndex = 0;
        closestPlayerPath = null;
    }

    private class PlayerWeCantSee
    {
        public CharacterStats player;
        public float distance;
    }

    void OnDrawGizmos()
    {
        if(!enabled)return;

        Gizmos.color = Color.blue;
        if(closestPlayer != null)
        {
            Debug.DrawLine(transform.position, closestPlayer.transform.position);
        }
        else if(currentNodeList != null)
        {
            for(int i = 1; i < currentNodeList.Length; i++)
            {
                Debug.DrawLine(currentNodeList[i - 1], currentNodeList[i]);
            }
        }
    }

}
