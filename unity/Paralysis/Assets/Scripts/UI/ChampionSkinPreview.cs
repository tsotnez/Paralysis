using UnityEngine;

public class ChampionSkinPreview : MonoBehaviour {

    public GameObject skinPrefab;

    public void onSelect()
    {
        FindObjectOfType<ChampionSelectionManager>().setChampion(UserControl.PlayerNumbers.Player1, skinPrefab);
    }
}
