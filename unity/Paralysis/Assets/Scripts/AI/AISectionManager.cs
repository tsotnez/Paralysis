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

        nonTargetableSection = new Section();
        nonTargetableSection.nonTargetable = true;
    }

    public SectionPathNode getClosestNodeForSection(Section section)
    {
        //TODO
        return null;
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
