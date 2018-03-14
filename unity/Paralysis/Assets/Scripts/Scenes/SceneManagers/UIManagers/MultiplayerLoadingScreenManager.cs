using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLoadingScreenManager : MonoBehaviour {

    //List holding the champions on each team. Fill these externally
    public static List<ChampionAndTrinketDatabase.Champions> Team1 = new List<ChampionAndTrinketDatabase.Champions>();
    public static List<ChampionAndTrinketDatabase.Champions> Team2 = new List<ChampionAndTrinketDatabase.Champions>();


    //Spawnpoints for the champion images
    public Transform[] spawnPointsT1;
    public Transform[] spawnPointsT2;
    public GameObject[] prefabs;
    public ChampionAndTrinketDatabase.Champions[] championNames;

    //Dictianory holding the prefab to load for every chmapion
    private Dictionary<ChampionAndTrinketDatabase.Champions, GameObject> PrefabsForChampions = new Dictionary<ChampionAndTrinketDatabase.Champions, GameObject>();

    // Use this for initialization
    void Start () {

        //For testing, fill teams automatically if empty
        if(Team1.Count == 0)
        {
            Team1.Add(ChampionAndTrinketDatabase.Champions.Alchemist);
            Team1.Add(ChampionAndTrinketDatabase.Champions.Archer);
            Team1.Add(ChampionAndTrinketDatabase.Champions.Assassin);
        }
        if (Team2.Count == 0)
        {
            Team2.Add(ChampionAndTrinketDatabase.Champions.Infantry);
            Team2.Add(ChampionAndTrinketDatabase.Champions.Assassin);
            Team2.Add(ChampionAndTrinketDatabase.Champions.Knight);
        }
        /////////////////////////////////////////////////////////

        //Fill dictionary
        for (int i = 0; i < prefabs.Length; i++)
        {
            PrefabsForChampions.Add(championNames[i], prefabs[i]);
        }

        //Fill team1
        for (int i = 0; i < Team1.Count; i++)
        {
            Instantiate(PrefabsForChampions[Team1[i]], spawnPointsT1[i], false);
        }

        //Fill team 2
        for (int i = 0; i < Team2.Count; i++)
        {
            GameObject go = Instantiate(PrefabsForChampions[Team2[i]], spawnPointsT2[i], false);

            //Mirror image
            Transform image = go.transform.Find("ChampionImage").transform;

            image.localScale = new Vector3(image.localScale.x * -1, image.localScale.y, image.localScale.z);
            image.localPosition = new Vector3(image.localPosition.x * -1, image.localPosition.y, image.localPosition.z);
        }
    }
}
