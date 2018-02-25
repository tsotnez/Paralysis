using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowFrameOnHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject frame;

    void Start()
    {
        if (frame == null)
            frame = transform.Find("Frame").gameObject;
    }

    private void onEnter()
    {
        try
        {
            frame.SetActive(true);
        }
        catch(UnassignedReferenceException ex)
        {
            Debug.Log(gameObject);
        }
    }

    private void onExit()
    {
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
