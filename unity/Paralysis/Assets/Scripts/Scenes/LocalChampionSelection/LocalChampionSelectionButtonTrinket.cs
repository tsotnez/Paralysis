using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalChampionSelectionButtonTrinket : ChampionSelectionButtonTrinket {

    protected LocalChampionSelectionManager manager;
    public UserControl.PlayerNumbers TargetPlayerNumber; //For which player

    protected override void Start()
    {
        base.Start();

        manager = FindObjectOfType<LocalChampionSelectionManager>();
        EventSystemGroup group = GetComponent<EventSystemGroup>();

        if (group != null)
        {
            switch (group.EventSystemID) //Set player number depending on eventsystemID
            {
                case 1:
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player1;
                    break;
                case 2:
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player2;
                    break;
                case 3:
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player3;
                    break;
                case 4:
                    TargetPlayerNumber = UserControl.PlayerNumbers.Player4;
                    break;
            }
        }
    }

    public override void onClick()
    {
        base.onClick();

        //Get all currently selected trinket Buttons
        LocalChampionSelectionButtonTrinket[] prevSelected = GameObject.FindObjectsOfType<LocalChampionSelectionButtonTrinket>().Where(x =>
        x.TargetPlayerNumber == this.TargetPlayerNumber && x.currentlySelected).ToArray();

        //If no other trinket is selected, set trinket1
        if (prevSelected.Length == 0)
            manager.setTrinket1(TargetPlayerNumber, trinket);
        //If there is another trinket selected already, set trinket2
        else if (prevSelected.Length == 1)
        {
            manager.setTrinket2(TargetPlayerNumber, trinket);
            prevSelected[0].selectedPos = 2;
        }
        //when there are already 2 trinkets selected, let the one selected first lose focus and overwrite the trinket that button set the value for
        else if (prevSelected.Length == 2)
        {
            LocalChampionSelectionButtonTrinket selectedFirst = prevSelected.First(x => x.selectedPos == 2);
            manager.setTrinket(TargetPlayerNumber, trinket, selectedFirst.trinket);
            selectedFirst.loseFocus();

            prevSelected.First(x => x.selectedPos == 1).selectedPos = 2;
        }

        selectedPos = 1;

        currentlySelected = true;
        text.enabled = true;
    }

    public override void loseFocus()
    {
        base.loseFocus();
        selectedPos = 0;
    }
}
