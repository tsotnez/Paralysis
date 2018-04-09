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

    public Section getRetreatSection(Vector2 currentPosition, Vector2 enemyPosition)
    {
        float maxDistance = -float.MaxValue;
        Section retSection = null;
        foreach(Section section in sections)
        {
            if (!section.nonTargetable)
            {
                foreach (Vector2 retreatPoint in section.getRetreatPoints())
                {
                    float actDistance = Mathf.Abs(Vector2.Distance(retreatPoint, enemyPosition));

                    //Give penalty for sections higher, so we don't need to jump
                    float yDifference = (currentPosition.y - retreatPoint.y);
                    actDistance += yDifference;

                    if (actDistance > maxDistance)
                    {
                        maxDistance = actDistance;
                        retSection = section;
                    }
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
