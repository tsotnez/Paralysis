using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class holding all information about a player
/// </summary>

[System.Serializable]
public class Player {

    /// <summary>
    /// Prefab of selected champion
    /// </summary>
    public GameObject ChampionPrefab;

    public UserControl.PlayerNumbers playerNumber;
    public int TeamNumber;

    /// <summary>
    /// Input deviced used
    /// </summary>
    public UserControl.InputDevice inputDevice;

    /// <summary>
    /// Type of first trinket
    /// </summary>
    public Trinket.Trinkets trinket1 = Trinket.Trinkets.UseTrinket_GetImmun;
    /// <summary>
    /// Type of second trinket
    /// </summary>
    public Trinket.Trinkets trinket2 = Trinket.Trinkets.UseTrinket_GetImmun;

    public Player(UserControl.PlayerNumbers pPlayerNumber, UserControl.InputDevice pInputDevice, int pTeam)
    {
        playerNumber = pPlayerNumber;
        inputDevice = pInputDevice;
        TeamNumber = pTeam;
    }
}
