using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISectionManager : MonoBehaviour {

    public static AISectionManager Instance;
    private Section[] sections;
    private Section nonTargetableSection;

    void Awake()
    {
        Instance = this;

        int numChildren = transform.childCount;
        sections = new Section[numChildren];

        for(int i = 0; i < numChildren; i++)
        {            
            sections[i] = transform.GetChild(i).GetComponent<Section>();
        }

        nonTargetableSection = GetComponent<Section>();
        nonTargetableSection.nonTargetable = true;
    }

    public SectionPathNode getClosestNodeForSection(Section section)
    {
        //TODO
        return null;
    }

    public Section getRetreatSection(Vector2 currentPosition, Vector2 targetPosition)
    {
        float maxDistance = -9999999;
        Section retSection = null;
        foreach(Section section in sections)
        {
            if (!section.nonTargetable)
            {
                float actDistance = Mathf.Abs(Vector2.Distance(section.getRetreatPosition(), targetPosition));

                //Give bonus for sections lower, so we don't need to jump
                float yDifference = (currentPosition.y - section.transform.position.y) * 2f;
                actDistance += yDifference;

                if (actDistance > maxDistance)
                {
                    maxDistance = actDistance;
                    retSection = section;
                }
            }
        }

        if (retSection == null)
        {
            Debug.LogError("couldn't find a retreat section...");
        }
        return retSection;
    }

    public Section getSectionForPosition(Vector2 position)
    {
        foreach(Section section in sections)
        {
            if(section.contains(position))
            {
                return section;
            }
        }

        return nonTargetableSection;
    }
}
