using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Holds information about the champions.
/// </summary>
public class ChampionDatabase
{
    public enum Champions
    {
        Archer = 0, Knight = 1, Infantry = 2, Alchemist = 3, Assassin = 4
    }

    /// <summary>
    /// Returns an Array with all possible Champions according to the enum.
    /// </summary>
    /// <returns></returns>
    public static Champions[] GetAllChampions()
    {
        return (Champions[]) Enum.GetValues(typeof(Champions));
    }

    public static ChampionDatabase.Champions getChampionEnumForID(int id)
    {
        ChampionDatabase.Champions[] champs = ChampionDatabase.GetAllChampions();
        if (id < champs.Length)
        {
            return champs[id];
        } 
        else
        {
            Debug.LogError("Couldn't find champion name for id: " + id);
            return champs[0];
        }
    }
}
