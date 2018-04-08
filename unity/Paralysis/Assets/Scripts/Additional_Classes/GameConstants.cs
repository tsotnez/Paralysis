using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game constants. Holds all the constants
/// </summary>
public static class GameConstants
{
    // Default range for attacks
    public const float MEELEATTACKRANGE = 1.5f;
    public const float RANGEATTACKRANGE = 7f;

    // Layers
    public static int[] TEAMLAYERS = { 11, 12, 15, 16, 17, 18, 19, 20 };
    public static int WALL_LAYER = 10;

    // Scenes
    public static string MAIN_MENU_SCENE = "MainMenu";
    public static string TEAM_ALLOCATION_SCENE = "TeamAllocation";
    public static string NETWORK_TEST_SCENE = "Network test";
    public static string NETWORK_ROOM_SCENE = "NetworkRoom";
    public static string NETWORK_HOST_PRIVATE_ROOM = "NetworkHostPrivateGame";
    public static string NETWORK_CHAMP_SELECT = "NetworkChampionSelection";

    // Tags
    public static string MAIN_PLAYER_TAG = "MainPlayer";
    public static string SPAWN_POINT_TAG = "SpawnPoint";


    // Controller
    public static string NAME_OF_XBOX360CONTROLLER_IN_ARRAY = "Controller (XBOX 360 For Windows)";

    // Transform Names
    public static string EFFECT_OVERLAY = "effectOverlay";
}
