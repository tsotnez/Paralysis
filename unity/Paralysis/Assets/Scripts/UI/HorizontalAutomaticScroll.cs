using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalAutomaticScroll : MonoBehaviour {

    ScrollRect scrollRect;
    Mask mask;
    RectTransform maskRect;
    List<GameObject> content = new List<GameObject>();

    [HideInInspector]
    public EventSystem es;
    public float LerpSpeed = .5f;
    public Transform contentTrans;
    public Transform viewPort;
    public EventSystem targetSystem;

	// Use this for initialization
	void Start () {
        mask = GetComponentInChildren<Mask>();
        maskRect = (RectTransform) mask.gameObject.transform;
        scrollRect = GetComponent<ScrollRect>();

        if (targetSystem = null)
        {
            es = EventSystem.current;
        }

        //Find all content game objects and add them to list
        foreach (Transform child in contentTrans)
        {
            content.Add(child.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (es == null)
        {
            Debug.LogError("Target Event System of Horizontal scroll script is null!");
            this.enabled = false;
            return;
        }

        GameObject selected = es.currentSelectedGameObject;

        //Is the currently selected GO one of the content GOs?
        if (content.Contains(selected) && selected != null)
        {
            float maxX = maskRect.rect.width * 0.5f;
            float minX = -maxX;

            //Holds the position of the Content GO relative to the viewport GO
            Vector3 relativePosition = selected.transform.position - viewPort.position;
            float selectedX = relativePosition.x * 100; //Mit hundert multiplizieren weil trabnsform.position in prozent

            //Check if selected GO is covered by mask
            if (selectedX + selected.GetComponent<RectTransform>().rect.width > maxX)
            {
                //Nach rechts
                scrollRect.horizontalScrollbar.value = Mathf.Clamp(scrollRect.horizontalScrollbar.value + LerpSpeed * Time.deltaTime, 0, 1);
            }
            else if (selectedX - selected.GetComponent<RectTransform>().rect.width < minX)
            {
                //Nach links
                scrollRect.horizontalScrollbar.value = Mathf.Clamp(scrollRect.horizontalScrollbar.value - LerpSpeed * Time.deltaTime, 0, 1);
            }
        }
    }
}
