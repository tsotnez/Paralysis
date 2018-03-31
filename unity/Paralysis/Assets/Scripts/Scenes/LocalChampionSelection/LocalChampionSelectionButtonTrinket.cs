using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocalChampionSelectionButtonTrinket : ChampionSelectionButtonTrinket {

    protected LocalChampionSelectionManager manager;
    public UserControl.PlayerNumbers TargetPlayerNumber; //For which player
    private Sprite picture;

    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        manager = FindObjectOfType<LocalChampionSelectionManager>();
        picture = transform.parent.Find("portrait").gameObject.GetComponent<Image>().sprite;
    }

    public override void onClick()
    {
        base.onClick();

        if(!currentlySelected && manager.canSetForPlayer(TargetPlayerNumber)) //Trinket is not yet selected by this player -> Select
        {
            manager.setTrinket(TargetPlayerNumber, trinket, picture);
            currentlySelected = true;
            text.enabled = true;
        }
        else if(currentlySelected) //Trinket is selected already -> remove trinket
        {
            manager.removeTrinket(TargetPlayerNumber, trinket);
            currentlySelected = false;
            text.enabled = false;
        }
    }

    public override void Selecting()
    {
        base.Selecting();
        if(manager.canSetForPlayer(TargetPlayerNumber))
            manager.showTrinket(TargetPlayerNumber, trinket, picture);
    }

    public override void loseFocus()
    {
        base.loseFocus();
    }
}
