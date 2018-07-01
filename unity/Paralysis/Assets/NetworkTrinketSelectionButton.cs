using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NetworkTrinketSelectionButton : MonoBehaviour {

    public Trinket.Trinkets trinket;
    public int trinketId = 1; // Which trinket to set for
    private bool currentlySelected = false;

    ShowFrameOnHighlight sfoh;
	// Use this for initialization
	void Start () {
        GetComponentInChildren<Button>().onClick.AddListener(onButtonClick);
        sfoh = GetComponentInChildren<ShowFrameOnHighlight>();

    }
	
    void onButtonClick()
    {
        currentlySelected = true;
        sfoh.LockFrameVisible = true;
        TrinketsParentButton activatedButton = FindObjectsOfType<TrinketsParentButton>().First(x => x.CurrentlySelected);

        activatedButton.gameObject.transform.Find("portrait").GetComponent<Image>().sprite 
            = transform.Find("portrait").GetComponent<Image>().sprite;

        int trinketId = activatedButton.trinketId;
        NetworkChampionSelectionManager.Instance.setTrinket(trinket, trinketId);

        activatedButton.CurrentlySelected = false;
        transform.parent.parent.gameObject.SetActive(false);
    }

    protected void OnDisable()
    {
        if (!currentlySelected)
        {
            sfoh.LockFrameVisible = false;
            sfoh.onExit();
        }
    }
}
