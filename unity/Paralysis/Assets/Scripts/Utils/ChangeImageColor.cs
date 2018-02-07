using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, IPointerDownHandler, IPointerClickHandler {

    public bool Clicked = false;

    public Graphic target;

    public Color onHoverCol;
    public Color onClickCol;
    public bool keepColorOnClick = false;

    private Color defaultCol;

    void Awake()
    {
        if(target == null)
            target = GetComponent<Graphic>();

        defaultCol = target.color;   
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            OnPointerUp();
    }

	public void onEnter()
    {
        if(target.color != onClickCol) //Only transition to hoverCol when not selected already
            target.color = onHoverCol;
    }

    public void onExit()
    {
        StopAllCoroutines();
        if(!keepColorOnClick || !Clicked)
            target.color = defaultCol;
    }

    public void resetTargetColor()
    {
        target.color = defaultCol;
        Clicked = false;
    }

    public void onClick()
    {
        Clicked = true;
        StopAllCoroutines();

        //If clicked color is supposed to be kept, reset all siblings on click
        foreach (Transform child in transform.parent.transform)
        {
            ChangeImageColor c = child.gameObject.GetComponent<ChangeImageColor>();
            if(c != null && c != this)
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


    #region Interface Implementations
    //Interface Methods

    public void OnSelect(BaseEventData eventData)
    {
        onEnter();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        onExit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!Input.GetMouseButton(0))
            onExit();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        onClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        target.color = onClickCol;
    }

    public void OnPointerUp()
    {
        if (!keepColorOnClick)
        {
            target.color = defaultCol;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick();
    }
    #endregion
}
