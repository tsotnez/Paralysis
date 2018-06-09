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

    /// <summary>
    /// Stores the content of this control which is defines as the children of the content GO
    /// </summary>
    private RectTransform[] content;
    /// <summary>
    /// Transforms of all slots
    /// </summary>
    public RectTransform[] slots;
    private MyStandaloneInputModule module;
    /// <summary>
    /// Is the mouse in the client area?
    /// </summary>
    private bool hovered = false;
    /// <summary>
    /// Index of center (Highlighted) Slot
    /// </summary>
    private int centerIndex;
    /// <summary>
    /// Matches content and their slot
    /// </summary>
    private Dictionary<int, int> SlotsOfContent = new Dictionary<int, int>();
    /// <summary>
    /// List containing indeces of those content GOs which cannot be assigned to a slot because there are more content GOs than there are slots
    /// </summary>
    private List<int> offScreenContent = new List<int>();
    /// <summary>
    /// Is Start() finished?
    /// </summary>
    private bool initialized = false;

    [HideInInspector]
    public GameObject SelectedGO = null;

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

        initialized = true;
	}

    /// <summary>
    /// Moves content to specified slot
    /// </summary>
    /// <param name="contentIndex">Index of content</param>
    /// <param name="slotIndex">Index of target slot</param>
    private void MoveToSlot(int contentIndex, int slotIndex)
    {
        //If we are dealing with a content which doesnt fit into our slots anymore...
        if(contentIndex >= slots.Length && !initialized)
        {
            offScreenContent.Add(contentIndex);
            return;
        }

        if (slotIndex >= slots.Length)
        {
            //Overflow on the right
            slotIndex = 0;
            if(content.Length > slots.Length)
            {
                //If there is more content than slots, move content to offScreen List and move last offScreenContent entry to first slot
                offScreenContent.Insert(0, contentIndex);
                SlotsOfContent[contentIndex] = -1;
                contentIndex = offScreenContent[offScreenContent.Count - 1];
            }
        }
        else if (slotIndex < 0)
        {
            //Overflow on the left
            slotIndex = slots.Length - 1;
            if (content.Length > slots.Length)
            {
                //If there is more content than slots, move content to offScreen List and move first offScreenContent entry to last slot
                offScreenContent.Add(contentIndex);
                SlotsOfContent[contentIndex] = -1;
                contentIndex = offScreenContent[0];
            }
        }

        RectTransform targetSlot = slots[slotIndex];
        RectTransform targetContent = content[contentIndex];

        targetContent.SetParent(targetSlot);
        targetContent.anchorMax = new Vector2(1f, 1f);
        targetContent.anchorMin = new Vector2(0f, 0f);
        targetContent.anchoredPosition = Vector2.zero;
        targetContent.offsetMax = new Vector2(0, 0);
        targetContent.offsetMin = new Vector2(0, 0);

        //Update currently selected GO
        if (slotIndex == centerIndex)
            SelectedGO = targetContent.gameObject;

        //Manage dictionary
        if (!SlotsOfContent.ContainsKey(contentIndex))
            SlotsOfContent.Add(contentIndex, slotIndex);
        else
            SlotsOfContent[contentIndex] = slotIndex;
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

    /// <summary>
    /// Selects the next champion to the speciefied direction and moves every child accordingly
    /// </summary>
    /// <param name="direction">The direction to scroll in, positive being to the right, negative to the left</param>
    public void Scroll(int direction)
    {
        int i = 0;
        foreach (RectTransform t in content)
        {
            int contentIndex = Array.IndexOf(content, t);
            int currentSlot = SlotsOfContent[contentIndex];

            if (currentSlot == -1) //when content is off Screen ignore it
                continue;

            MoveToSlot(contentIndex, currentSlot + direction);
            i++;
        }
    }
}
