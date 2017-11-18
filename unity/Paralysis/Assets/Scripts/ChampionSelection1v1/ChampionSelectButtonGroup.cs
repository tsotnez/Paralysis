using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChampionSelectButtonGroup : MonoBehaviour {
    public ChampionSelectButton[] buttonsGroup1; //Buttons for player 2
    public ChampionSelectButton[] buttonsGroup2; // Buttons for player 2

    public ChampionSelectButton selectedButtonGroup1 = null; //The selected button for player 2
    public ChampionSelectButton selectedButtonGroup2 = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setSelectedButton(ChampionSelectButton button)
    {
        if (Array.IndexOf(buttonsGroup1, button) > -1) // check if button is in group 1 or 2
        {
            //Switch selected buttons
            if (selectedButtonGroup1 != null)
                selectedButtonGroup1.selectedInGroup = false;
            button.selectedInGroup = true;
            selectedButtonGroup1 = button;
        }
        else
        {
            //In group 2
            if (selectedButtonGroup2 != null)
                selectedButtonGroup2.selectedInGroup = false;
            button.selectedInGroup = true;
            selectedButtonGroup2 = button;
        }
    }
}
