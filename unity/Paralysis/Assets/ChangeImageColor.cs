using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour {

    public Color onHoverCol;
    public Color onClickCol;

    private Color defaultCol;

    Image img;

    void Start()
    {
        img = GetComponent<Image>();
        defaultCol = img.color;
    }

	public void onEnter()
    {
        img.color = onHoverCol;
    }

    public void onExit()
    {
        img.color = defaultCol;
    }

    public void onClick()
    {
        img.color = onClickCol;
    }
}
