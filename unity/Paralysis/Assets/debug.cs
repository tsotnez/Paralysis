using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour {
    public GameObject[] champions;
    int currentChamp = 0;

    [SerializeField]
    private int startChar = 0;

    private void Start()
    {
        currentChamp = startChar;

        GameObject currentPlayer = GameObject.FindGameObjectWithTag("MainPlayer");

        Debug.Log(currentPlayer.name);
        GameObject newPlayer = Instantiate(champions[currentChamp], currentPlayer.transform.position, Quaternion.identity);

        Camera.main.GetComponent<CameraBehaviour>().changeTarget(newPlayer.transform);
        Destroy(currentPlayer);

    }

    //Switches the champion by changing the prefab
    public void changeChampion()
    {
        increaseIndex();

        GameObject currentPlayer = GameObject.FindGameObjectWithTag("MainPlayer");

        Debug.Log(currentPlayer.name);
        GameObject newPlayer = Instantiate(champions[currentChamp], currentPlayer.transform.position, Quaternion.identity);

        Camera.main.GetComponent<CameraBehaviour>().changeTarget(newPlayer.transform);
        Destroy(currentPlayer);
    }

    private void increaseIndex()
    {
        if ((currentChamp + 1) <= (champions.Length - 1)) currentChamp++;
        else currentChamp = 0;
    }
}
