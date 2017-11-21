﻿using UnityEngine;
using UnityEngine.UI;

public class trinketSelectionButton : MonoBehaviour {

    public GameObject availableTrinkets;
    public Trinket.Trinkets selectedTrinket;
    public ChampionSelectButton.players targetPlayer;

    public int slotNumber = 1;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(ShowAvailableTrinkets);
        SetSelectedTrinket(selectedTrinket);
	}
	
	void ShowAvailableTrinkets()
    {
        //Show trinket selection and set target slot to this object
        if (availableTrinkets.activeInHierarchy == true)
            availableTrinkets.SetActive(false);

        availableTrinkets.GetComponent<AvailableTrinketsSelection>().targetSlot = this;
        availableTrinkets.SetActive(true);
    }

    public void SetSelectedTrinket(Trinket.Trinkets value)
    {
        selectedTrinket = value;

        switch (targetPlayer)
        {
            case ChampionSelectButton.players.Player1:
                if (slotNumber == 1)
                    FindObjectOfType<ChampionSelectionManagerbehaviour>().trinket1Player1 = value;
                else
                    FindObjectOfType<ChampionSelectionManagerbehaviour>().trinket2Player1 = value;
                break;
            case ChampionSelectButton.players.Player2:
                if (slotNumber == 1)
                    FindObjectOfType<ChampionSelectionManagerbehaviour>().trinket1Player2 = value;
                else
                    FindObjectOfType<ChampionSelectionManagerbehaviour>().trinket2Player2 = value;
                break;
            case ChampionSelectButton.players.Player3:
                break;
            case ChampionSelectButton.players.Player4:
                break;
        }
    }
}
