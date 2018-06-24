using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowFrameOnHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool lockFrameVisible = false;

    public bool LockFrameVisible //When true, frame will be always visible
    {
        get { return lockFrameVisible; }
        set
        {
            lockFrameVisible = value;
            if (value)
                onEnter();
            else
                onExit();
        }
    }
    public GameObject frame;

    void Start()
    {
        if (frame == null)
            frame = transform.Find("Frame").gameObject;
    }

    public void onEnter()
    {
        try
        {
            frame.SetActive(true);
        }
        catch(UnassignedReferenceException)
        {
            Debug.Log(gameObject);
        }
    }

    public void onExit()
    {
        if(!lockFrameVisible)
            frame.SetActive(false);
    }

    #region Interface Implementations
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
        onExit();
    }

    public void OnSelect(BaseEventData eventData)
    {
        onEnter();
    }
    #endregion
}
