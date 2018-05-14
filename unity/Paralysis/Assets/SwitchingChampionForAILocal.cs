using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchingChampionForAILocal : MonoBehaviour, IPointerClickHandler, ISubmitHandler {

    /// <summary>
    /// In which direction should be looped through the champions
    /// </summary>
    public int Direction = 1;

    [HideInInspector]
    public int IndexOfCurrentChamp = -1;
    public UserControl.PlayerNumbers targetPlayer;

    /// <summary>
    /// Contains the right button when this is a left button and vice versa
    /// </summary>
    [HideInInspector]
    public SwitchingChampionForAILocal Opposite;

    private LocalChampionSelectionManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        NextChamp();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        NextChamp();
    }

    private void NextChamp()
    {
        if (IndexOfCurrentChamp + Direction > ChampionDatabase.GetAllChampions().Length - 1) // upper end of array... start from the bottom
            IndexOfCurrentChamp = 0;
        else if (IndexOfCurrentChamp + Direction < 0) // bottom end of the array.. jump to top
            IndexOfCurrentChamp = ChampionDatabase.GetAllChampions().Length - 1;
        else
            IndexOfCurrentChamp += Direction;

        manager.setChampion(targetPlayer,  manager.ChampionPrefabs[ChampionDatabase.GetAllChampions()[IndexOfCurrentChamp]]);

        //Pass current index to opposite button
        Opposite.IndexOfCurrentChamp = this.IndexOfCurrentChamp;
    }

    // Use this for initialization
    void Start () {
        manager = LocalChampionSelectionManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
