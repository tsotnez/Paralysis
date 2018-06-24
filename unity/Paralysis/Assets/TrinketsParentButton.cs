using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketsParentButton : MonoBehaviour {

    /// <summary>
    /// Are we setting the trinket for this slot currently?
    /// </summary>
    private bool currentlySelected = false;

    /// <summary>
    /// Button for second trinket
    /// </summary>
    public TrinketsParentButton sibling;

    ShowFrameOnHighlight sfoh;
    Button btn;
    public int trinketId;

    public bool CurrentlySelected
    {
        get
        {
            return currentlySelected;
        }

        set
        {
            currentlySelected = value;
            sfoh.LockFrameVisible = value;
        }
    }


    // Use this for initialization
    void Start () {
        btn = GetComponentInChildren<Button>();
        sfoh = GetComponentInChildren<ShowFrameOnHighlight>();
        btn.onClick.AddListener(onButtonClick);
	}

    private void onButtonClick()
    {
        transform.parent.Find("Trinkets").gameObject.SetActive(true); //Show trinket selection
        CurrentlySelected = true;
        sibling.CurrentlySelected = false;
    }


}
