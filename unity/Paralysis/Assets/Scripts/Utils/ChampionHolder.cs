using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionHolder : MonoBehaviour {

    public enum Champions { ARCHER = 0, KNIGHT = 1, INFANTRY = 2, ALCHEMIST = 3, ASSASSIN = 4 };
    public GameObject[] champions;

    public GameObject getChampionForID(int id)
    {
        switch(id)
        {
        case (int)Champions.ARCHER:
            return champions[0];
        case (int)Champions.KNIGHT:
            return champions[1];
        case (int)Champions.INFANTRY:
            return champions[2];
        case (int)Champions.ALCHEMIST:
            return champions[3];
        case (int)Champions.ASSASSIN:
            return champions[4];
        default:
            return null;
        }
    }

}
