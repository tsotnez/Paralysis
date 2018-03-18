using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGetClosetEnemy : MonoBehaviour {

    public GameObject TargetPlayer { get { 
            if(targetPlayer != null)return targetPlayer.gameObject; 
            else return null;
    }}
    
    private CharacterStats targetPlayer;
    private List<CharacterStats> enemyPlayers;

    private void Start()
    {
        if(GetComponent<UserControl>().inputDevice != UserControl.InputDevice.AI)
        {
            enabled = false;
        }
        else 
        {
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
            if(playerObj != gameObject && playerObj.layer != gameObject.layer)
            {
                enemyPlayers.Add(playerObj.GetComponent<CharacterStats>());
            }
        }
    }


    private void FixedUpdate()
    {
        if (enemyPlayers == null || enemyPlayers.Count == 0)
        {
            setPlayerList();
            return;
        }

		if (enemyPlayers.Count == 1) 
		{
			targetPlayer = enemyPlayers [0];
			return; 
		}

        float actDistance = float.MaxValue;
        targetPlayer = null;
        foreach (CharacterStats enemyPlayer in enemyPlayers)
        {
            float distance = Mathf.Abs(Vector2.Distance(enemyPlayer.transform.position, transform.position));
            if (distance < actDistance && !enemyPlayer.CharacterDied)
            {
                targetPlayer = enemyPlayer;
            }
        }

    }

}
