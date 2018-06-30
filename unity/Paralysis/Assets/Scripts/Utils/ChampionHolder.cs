using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHolder : MonoBehaviour {

    public GameObject[] champions;

    public GameObject getChampionForID(int id)
    {
        switch(id)
        {
        case (int)ChampionDatabase.Champions.Archer:
            return champions[0];
        case (int)ChampionDatabase.Champions.Knight:
            return champions[1];
        case (int)ChampionDatabase.Champions.Infantry:
            return champions[2];
        case (int)ChampionDatabase.Champions.Alchemist:
            return champions[3];
        case (int)ChampionDatabase.Champions.Assassin:
            return champions[4];
        default:
            Debug.LogError("Couldn't find champion game object for id: " + id);
            return null;
        }
    }

}
