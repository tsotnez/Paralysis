using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketPreview : MonoBehaviour {

    public Trinket.Trinkets trinket;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(setTrinketAsSelected);
    }

    void setTrinketAsSelected()
    {
        AvailableTrinketsSelection parent = transform.parent.GetComponent<AvailableTrinketsSelection>();
        parent.targetSlot.setSelectedTrinket(trinket);
        parent.targetSlot.gameObject.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        Text temp = parent.targetSlot.transform.GetComponentInChildren<Text>();
        var temp2 = Trinket.trinketsForNames[trinket].GetProperty("DisplayName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        parent.gameObject.SetActive(false);
    }
}
