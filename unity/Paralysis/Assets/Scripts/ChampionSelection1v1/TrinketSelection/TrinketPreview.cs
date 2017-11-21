using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketPreview : MonoBehaviour {

    public Trinket.Trinkets trinket;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SetTrinketAsSelected);
        GetComponentInChildren<Text>().text = gameObject.name;
    }

    private void SetTrinketAsSelected()
    {
        AvailableTrinketsSelection parent = transform.parent.GetComponent<AvailableTrinketsSelection>();
        parent.targetSlot.SetSelectedTrinket(trinket);
        parent.targetSlot.gameObject.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        parent.targetSlot.transform.GetComponentInChildren<Text>().text = GetComponentInChildren<Text>().text;
        parent.gameObject.SetActive(false);
    }
}
