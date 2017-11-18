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
    private Image image;

    public bool selectedInGroup = false;
    private bool pointerOverButton = false;


    public enum players
    {
        Player1, Player2, Player3, Player4
    }

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        defaultSprite = image.sprite;
        btn = GetComponent<Button>();
        btn.onClick.AddListener(showChampion);
  	}

    private void Update()
    {
        //Is the button the selected button, keep showing the hover sprite
        if (!pointerOverButton)
        {
            if (selectedInGroup)
            {
                image.sprite = onHoverSprite;
            }
            else
                image.sprite = defaultSprite;
        }
    }

    void showChampion()
    {
        transform.parent.transform.parent.GetComponent<ChampionSelectButtonGroup>().setSelectedButton(this);
        GameObject.Find("manager").GetComponent<ChampionSelectionManagerbehaviour>().showChamp(previewPrefab, championPrefab, playerNumber);
    }

    public void PointerEnter()
    {
        pointerOverButton = true;
        transform.Find("Overlay").gameObject.SetActive(true);

    }

    public void PointerExit()
    {
        pointerOverButton = false;
        transform.Find("Overlay").gameObject.SetActive(false);
        image.sprite = defaultSprite;
    }

}
