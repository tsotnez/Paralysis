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


    }

    // Update is called once per frame
    void Update()
    {

        //Find all content game objects and add them to list, TODO: PERFORMANCE WISE BAD
        foreach (Transform child in contentTrans)
        {
            content.Add(child.gameObject);
        }

        GameObject selected = es.currentSelectedGameObject;

        //Is the currently selected GO one of the content GOs?
        if (content.Contains(selected))
        {
            float maxY = maskRect.rect.height * 0.5f;
            float minY = -maxY;

            //Holds the position of the Content GO relative to the viewport GO
            Vector3 relativePosition = selected.transform.position - viewPort.position;
            float selectedY = relativePosition.y * 100; //Mit hundert multiplizieren weil trabnsform.position in prozent

            //Check if selected GO is covered by mask
            if (selectedY + selected.GetComponent<RectTransform>().rect.height > maxY)
            {
                //Nach rechts
                scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value + LerpSpeed * Time.deltaTime, 0, 1);
            }
            else if (selectedY - selected.GetComponent<RectTransform>().rect.height < minY)
            {
                //Nach links
                scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value - LerpSpeed * Time.deltaTime, 0, 1);
            }
        }
    }
}
