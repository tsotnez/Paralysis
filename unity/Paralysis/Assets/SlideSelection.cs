using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This provides a slide selection UI Element. The selected element is presented bigger.
/// </summary>
public class SlideSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler  {

    public float CenterSlotSizeMultiplier = 1.4f;

    RectTransform[] content;
    public RectTransform[] slots;
    MyStandaloneInputModule module;
    bool hovered = false;
    int centerIndex;

    // Use this for initialization
    void Start () {
        Transform contentTrans = transform.Find("Content");
        content = new RectTransform[contentTrans.childCount];
        centerIndex = slots.Length / 2;

        slots[centerIndex].sizeDelta = new Vector2(slots[centerIndex].sizeDelta.x * CenterSlotSizeMultiplier, slots[centerIndex].sizeDelta.y * CenterSlotSizeMultiplier);

        //Get all children and save them in Array
        int i = 0;
        foreach (RectTransform t in contentTrans)
        {
            content[i] = t;
            i++;
        }

        i = 0;
        foreach (RectTransform t in content)
        {
            t.gameObject.SetActive(true);
            MoveToSlot(i, i);
            i++;
        }
	}
	
    private void MoveToSlot(int contentIndex, int slotIndex)
    {
        RectTransform targetSlot = slots[slotIndex];
        RectTransform targetContent = content[contentIndex];

        targetContent.SetParent(targetSlot);
        targetContent.anchorMax = new Vector2(1f, 1f);
        targetContent.anchorMin = new Vector2(0f, 0f);
        targetContent.anchoredPosition = Vector2.zero;
        targetContent.offsetMax = new Vector2(0, 0);
        targetContent.offsetMin = new Vector2(0, 0);
    }

	// Update is called once per frame
	void Update () {
        if (module == null)
        {
            module = ((MyStandaloneInputModule)EventSystem.current.currentInputModule);
            return;
        }

        if (!hovered)
            return;

        int hoveredIndex = IndexAtMousePosition();
	}

    private int IndexAtMousePosition()
    {
        for (int i = 0; i < content.Length; i++)
        {
            PointerEventData p = module.GetPointerData();
            if (p == null)
                break;

            if (p.pointerCurrentRaycast.gameObject == null)
                break;

            if (p.pointerCurrentRaycast.gameObject.transform == content[i])
                return i;
        }
        return -1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
