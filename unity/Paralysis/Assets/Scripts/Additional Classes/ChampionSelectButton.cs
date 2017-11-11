using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelectButton : MonoBehaviour {

    Button btn;
    public GameObject previewPrefab;
    public GameObject championPrefab;
    public players playerNumber;

    public Sprite onHoverSprite;
    private Sprite defaultSprite;

    public enum players
    {
        Player1, Player2, Player3, Player4
    }

	// Use this for initialization
	void Start () {
        defaultSprite = GetComponent<Image>().sprite;
        btn = GetComponent<Button>();
        btn.onClick.AddListener(showChampion);
  	}
	
	void showChampion()
    {
        GameObject.Find("manager").GetComponent<ChampionSelectionManagerbehaviour>().showChamp(previewPrefab, championPrefab, playerNumber);
    }

    public void PointerEnter()
    {
        GetComponent<Image>().sprite = onHoverSprite;
    }

    public void PointerExit()
    {
        GetComponent<Image>().sprite = defaultSprite;
    }

}
