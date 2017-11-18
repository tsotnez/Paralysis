using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketPreview : MonoBehaviour {

    public Trinket.Trinkets trinket;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(setTrinketAsSelected);
        GetComponentInChildren<Text>().text = gameObject.name;
    }

    void setTrinketAsSelected()
    {
        AvailableTrinketsSelection parent = transform.parent.GetComponent<AvailableTrinketsSelection>();
        parent.targetSlot.setSelectedTrinket(trinket);
        parent.targetSlot.gameObject.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        parent.targetSlot.transform.GetComponentInChildren<Text>().text = GetComponentInChildren<Text>().text;
        parent.gameObject.SetActive(false);
    }
}
