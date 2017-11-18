using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AvailableTrinketsSelection : MonoBehaviour {

    Sprite[] allTrinketImages;

    // Use this for initialization
	void Awake () {
        allTrinketImages = Resources.LoadAll<Sprite>("Sprites/AbilityImages/Trinkets");
	}

    private void OnEnable()
    {
             
    }

}
