using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Holds information about the champions.
/// </summary>
public class ChampionDatabase
{
    public enum Champions
    {
        Archer, Knight, Infantry, Alchemist, Assassin
    }

    /// <summary>
    /// Returns an Array with all possible Champions according to the enum.
    /// </summary>
    /// <returns></returns>
    public static Champions[] GetAllChampions()
    {
        return (Champions[]) Enum.GetValues(typeof(Champions));
    }
}
