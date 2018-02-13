using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerticalAutomaticScroll : MonoBehaviour {

    ScrollRect scrollRect;
    Mask mask;
    RectTransform maskRect;
    List<GameObject> content = new List<GameObject>();
    private EventSystem es;
    public float LerpSpeed = .5f;
    public Transform contentTrans;
    public Transform viewPort;

    // Use this for initialization
    void Start()
    {
        mask = GetComponentInChildren<Mask>();
        maskRect = (RectTransform)mask.gameObject.transform;
        scrollRect = GetComponent<ScrollRect>();
        es = EventSystem.current;

        //Find all content game objects and add them to list, TODO: PERFORMANCE WISE BAD
        foreach (Transform child in contentTrans)
        {
            content.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject selected = es.currentSelectedGameObject;

        //Is the currently selected GO one of the content GOs?
        if (content.Contains(selected) && selected != null)
        {
            float maxY = maskRect.rect.height;
            float minY = -maxY;

            //Holds the position of the Content GO relative to the viewport GO
            Vector3 relativePosition = viewPort.transform.InverseTransformPoint(selected.transform.position);
            float selectedY = relativePosition.y; //Mit hundert multiplizieren weil transform.position in prozent

            //Check if selected GO is covered by mask
            if (selectedY + selected.GetComponent<RectTransform>().rect.height > 0) //+ selected.GetComponent<RectTransform>().rect.height > maxY)
            {
                //Go up
                scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value + LerpSpeed * Time.deltaTime, 0, 1);
            }
            else if (selectedY - selected.GetComponent<RectTransform>().rect.height < minY)
            {
                //Go down
                scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value - LerpSpeed * Time.deltaTime, 0, 1);
            }
        }
    }
}
