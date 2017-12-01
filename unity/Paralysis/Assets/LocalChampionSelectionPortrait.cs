using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalChampionSelectionPortrait : MonoBehaviour {

    private Image img;

    public Sprite Default;
    public Sprite Hover;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    /// <summary>
    /// Switches from shadow to colored image
    /// </summary>
    /// <param name="target"></param>
    public void switchTo(int target)
    {
        if (target == 0)
            img.sprite = Default;
        else if (target == 1)
            img.sprite = Hover;
    }

}
