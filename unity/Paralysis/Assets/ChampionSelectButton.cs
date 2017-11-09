using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelectButton : MonoBehaviour {

    Button btn;
    public GameObject previewPrefab;
    public GameObject championPrefab;
    public players playerNumber;

    public enum players
    {
        Player1, Player2, Player3, Player4
    }

	// Use this for initialization
	void Start () {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(showChampion);
  	}
	
	void showChampion()
    {
        GameObject.Find("manager").GetComponent<ChampionSelectionManagerbehaviour>().showChamp(previewPrefab, championPrefab, playerNumber);
    }
}
