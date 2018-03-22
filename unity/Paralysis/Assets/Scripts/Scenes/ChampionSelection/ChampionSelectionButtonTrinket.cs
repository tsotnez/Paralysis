using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelectionButtonTrinket : ChampionSelectionButton {

    [HideInInspector]
    public int selectedPos = 0; //Order in which this button was selected (The second selected will be removed when selecting a different trinket)

    public Trinket.Trinkets trinket; //Trinket this button sets

    public override void Selecting()
    {
        base.Selecting();
        showTrinketPopUp();
    }

    public override void Deselecting()
    {
        base.Deselecting();
        transform.parent.parent.parent.Find("PopUpDesc").gameObject.SetActive(false);
    }

    private void showTrinketPopUp()
    {
        Transform popup = transform.parent.parent.parent.Find("PopUpDesc");

        popup.Find("TrinketName").gameObject.GetComponent<Text>().text = TrinketDatabase.TrinketNames[trinket];
        popup.Find("Desc").gameObject.GetComponent<Text>().text = TrinketDatabase.TrinketDescriptions[trinket];

        popup.gameObject.SetActive(true);
    }
}
