﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampionSelectionManagerbehaviour : MonoBehaviour {

    public GameObject[] platforms;

    //Holds [Platform] as key and [previewPrefab][champoionPrefab] as value
    private Dictionary<GameObject, GameObject[]> championsOnPlatforms = new Dictionary<GameObject, GameObject[]>();
    private Button btnStart;

    // Use this for initialization
    void Start () {
        btnStart = GameObject.Find("ButtonStart").GetComponent<Button>();
        btnStart.interactable = false;
        btnStart.onClick.AddListener(startGame);

        //Fill dictionary
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null)
                championsOnPlatforms.Add(platforms[i], null);
        }
	}

    private void Update()
    {
        //Show Start button if all champions are selected
        bool ready = true;
        foreach (KeyValuePair<GameObject, GameObject[]> pair in championsOnPlatforms)
        {
            ready = pair.Value != null;
        }
        btnStart.interactable = ready;
    }

    void startGame()
    {
        //Pass players 
        debug.player1 = championsOnPlatforms[platforms[0]][1];
        debug.player2 = championsOnPlatforms[platforms[1]][1];
        debug.startOnMultiplayer = true;
        //Load scene
        SceneManager.LoadScene("scenes/test");
    }

    public void showChamp(GameObject previewPrefab, GameObject championPrefab, ChampionSelectButton.players player)
    {
        switch (player)
        {
            case ChampionSelectButton.players.Player1:
                instantiateChampion(previewPrefab, championPrefab, 0);
                break;
            case ChampionSelectButton.players.Player2:
                instantiateChampion(previewPrefab, championPrefab, 1);
                break;
            case ChampionSelectButton.players.Player3:
                instantiateChampion(previewPrefab, championPrefab, 2);
                break;
            case ChampionSelectButton.players.Player4:
                instantiateChampion(previewPrefab, championPrefab, 3);
                break;
        }
    }

    void instantiateChampion(GameObject previewPrefab, GameObject championPrefab, int index)
    {
        //Delete champion from platform and show new one
        if (championsOnPlatforms[platforms[index]] != null)
        {
            Destroy(championsOnPlatforms[platforms[index]][0]);
            championsOnPlatforms[platforms[index]] = null;
        }
        GameObject champion = Instantiate(previewPrefab, platforms[index].transform.position + new Vector3(0, 0.3f), Quaternion.identity);

        //Flip champion if necessary
        if (index == 0)
        {
            Vector3 theScale = champion.transform.localScale;
            theScale.x *= -1;
            champion.transform.localScale = theScale;
        }
        //Play basic attack animation
        champion.transform.Find("graphics").GetComponent<ChampionAnimationController>().trigBasicAttack1 = true;

        GameObject[] prefabs = {champion, championPrefab };
        championsOnPlatforms[platforms[index]] = prefabs;
    }
}
