using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour {

    public MaskableGraphic target;

    public Color onHoverCol;
    public Color onClickCol;

    private Color defaultCol;

    void Start()
    {
        if(target == null)
            target = GetComponent<Image>();

        defaultCol = target.color;
    }

	public void onEnter()
    {
        target.color = onHoverCol;
    }

    public void onExit()
    {
        StopAllCoroutines();
        target.color = defaultCol;
    }

    public void onClick()
    {
        StopAllCoroutines();
        StartCoroutine(flashColor());
    }

    private IEnumerator flashColor()
    {
        target.color = onClickCol;
        yield return new WaitForSeconds(.05f);
        target.color = onHoverCol;
    }
}
