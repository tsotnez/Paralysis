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

    public event EventHandler SelectedGOChangedHandler;

    public float CenterSlotSizeMultiplier = 1.4f;

    /// <summary>
    /// Stores the content of this control which is defines as the children of the content GO
    /// </summary>
    public RectTransform[] content;
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

        foreach (RectTransform slot in slots)
        {
            slot.GetComponent<Image>().enabled = false;
        }

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
        if (contentIndex >= slots.Length && !initialized)
        {
            content[contentIndex].gameObject.SetActive(false);
            offScreenContent.Add(contentIndex);
            SlotsOfContent.Add(contentIndex, -1);
            return;
        }

        if (slotIndex >= slots.Length)
        {
            //Overflow after clicked left arrow
            slotIndex = 0;
            if (content.Length > slots.Length)
            {
                //If there is more content than slots, move content to offScreen List and move last offScreenContent entry to first slot
                offScreenContent.Insert(0, contentIndex);
                content[contentIndex].gameObject.SetActive(false); //Hide element which is now off Screen
                SlotsOfContent[contentIndex] = -1; //Set index -1 for now off screen content
                contentIndex = offScreenContent[offScreenContent.Count - 1];
                offScreenContent.RemoveAt(offScreenContent.Count - 1);
            }
        }
        else if (slotIndex < 0)
        {
            //Overflow after clicked right arrow
            slotIndex = slots.Length - 1;
            if (content.Length > slots.Length)
            {
                //If there is more content than slots, move content to offScreen List and move first offScreenContent entry to last slot
                offScreenContent.Add(contentIndex);
                content[contentIndex].gameObject.SetActive(false);
                SlotsOfContent[contentIndex] = -1;
                contentIndex = offScreenContent[0];
                offScreenContent.RemoveAt(0);
            }
        }

        content[contentIndex].gameObject.SetActive(true);
        RectTransform targetSlot = slots[slotIndex];
        RectTransform targetContent = content[contentIndex];

        targetContent.SetParent(targetSlot.Find("Content"));
        targetContent.anchorMax = new Vector2(1f, 1f);
        targetContent.anchorMin = new Vector2(0f, 0f);
        targetContent.anchoredPosition = Vector2.zero;
        targetContent.offsetMax = new Vector2(0, 0);
        targetContent.offsetMin = new Vector2(0, 0);

        //Update currently selected GO
        if (slotIndex == centerIndex)
        {
            SelectedGO = targetContent.gameObject;
            OnSelectedGOChanged();
        }

        //Manage dictionary
        if (!SlotsOfContent.ContainsKey(contentIndex))
            SlotsOfContent.Add(contentIndex, slotIndex);
        else
            SlotsOfContent[contentIndex] = slotIndex;
    }

    protected virtual void OnSelectedGOChanged()
    {
        if (SelectedGOChangedHandler != null)
            SelectedGOChangedHandler(this, new EventArgs());
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
        StartCoroutine(ScrollRoutine(direction));
    }

    IEnumerator ScrollRoutine(int direction)
    {
        List<int> blackList = new List<int>();
        blackList.AddRange(offScreenContent);

        direction = -direction; //Moving the elements to the left is actually scrolling to the right and vice versa -> Brainfuck 
        int i = 0;


        foreach (RectTransform t in content)
        {
            int contentIndex = Array.IndexOf(content, t);
            int currentSlot = SlotsOfContent[contentIndex];

            if (blackList.Contains(contentIndex) || contentIndex == -1) //when content is off Screen ignore it
                continue;

            if (direction > 0)
                //Play right anim
                slots[currentSlot].Find("Content").GetComponent<Animator>().SetTrigger("playRight");
            else
                //Play left anim
                slots[currentSlot].Find("Content").GetComponent<Animator>().SetTrigger("playLeft");
            i++;
        }

        yield return new WaitForSeconds(0.16f);

        i = 0;
        foreach (RectTransform t in content)
        {
            int contentIndex = Array.IndexOf(content, t);
            int currentSlot = SlotsOfContent[contentIndex];

            if (blackList.Contains(contentIndex) || contentIndex == -1) //when content is off Screen ignore it
                continue;

            MoveToSlot(contentIndex, currentSlot + direction);
            i++;
        }
    }
}
