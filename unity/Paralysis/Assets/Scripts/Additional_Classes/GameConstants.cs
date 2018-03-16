using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game constants. Holds all the constants
/// </summary>
public static class GameConstants
{
    // Default range for attacks
    public const float MeeleAttackRange = 1.5f;
    public const float RangeAttackRange = 7f;

    public static int[] TEAMLAYERS = { 11, 12, 15, 16, 17, 18, 19, 20 };

    public static string MAIN_MENU_SCENE = "MainMenu";
    public static string TEAM_ALLOCATION_SCENE = "TeamAllocation";
    public static string NETWORK_TEST_SCENE = "Network test";
    public static string NETWORK_ROOM_SCENE = "NetworkRoom";
    public static string NETWORK_HOST_PRIVATE_ROOM = "NetworkHostPrivateGame";
    public static string NETWORK_CHAMP_SELECT = "NetworkChampionSelection";

    public static string MAIN_PLAYER_TAG = "MainPlayer";
    public static string SPAWN_POINT_TAG = "SpawnPoint";

    public static string NAME_OF_XBOX360CONTROLLER_IN_ARRAY = "Controller (XBOX 360 For Windows)";
}
