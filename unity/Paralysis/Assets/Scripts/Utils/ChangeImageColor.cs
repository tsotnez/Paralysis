using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour {

    MaskableGraphic target;

    public Color onHoverCol;
    public Color onClickCol;
    public bool keepColorOnClick = false;

    private Color defaultCol;

    void Awake()
    {
        if(target == null)
            target = GetComponent<Image>();

        defaultCol = target.color;
    }

	public void onEnter()
    {
        if(target.color != onClickCol) //Only transition to hoverCol when not selected already
            target.color = onHoverCol;
    }

    public void onExit()
    {
        StopAllCoroutines();
        if(!keepColorOnClick || (keepColorOnClick && target.color != onClickCol))
            target.color = defaultCol;
    }

    public void resetTargetColor()
    {
        target.color = defaultCol;
    }

    public void onClick()
    {
        StopAllCoroutines();

        //If clicked color is supposed to be kept, reset all siblings on click
        foreach (Transform child in transform.parent.transform)
        {
            ChangeImageColor c = child.gameObject.GetComponent<ChangeImageColor>();
            if(c != null)
            {
                c.resetTargetColor();
            }
        }

        StartCoroutine(flashColor());
    }

    private IEnumerator flashColor()
    {
        target.color = onClickCol;
        yield return new WaitForSeconds(.05f);
        if(!keepColorOnClick)
            target.color = onHoverCol;
    }
}
