using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour {

    public float width = 10f;
    public float height = 10f;
    public Color gizmoColor = Color.cyan;
    public bool nonTargetable = false;

    public SectionPath[] SectionPaths { get { return SectionPaths; } }
    private SectionPath[] sectionPaths;

    private float maxY;
    private float minY;
    private float maxX;
    private float minX;

    private void Awake()
    {
        int numChildren = transform.childCount;
        sectionPaths = new SectionPath[numChildren];

        for(int i = 0; i < numChildren; i++)
        {            
            sectionPaths[i] = transform.GetChild(i).GetComponent<SectionPath>();
        }

        maxY = transform.position.y + (height/2);
        minY = transform.position.y - (height/2);
        maxX = transform.position.x + (width/2);
        minX = transform.position.x - (width/2);
    }

    public SectionPath getPathForTargetSection(Section targetSection)
    {
        foreach(SectionPath sectionP in sectionPaths)
        {
            if(sectionP.TargetSection == targetSection)
            {
                return sectionP;
            }
        }

        return null;
    }

    public bool contains(Vector2 position)
    {
        return position.y <= maxY && position.y >= minY && position.x <= maxX && position.x >= minX;
    }

    void OnDrawGizmos()
    {
        Gizmos.color =  gizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
        GizmoStar.drawStar(transform.position, gizmoColor, .15f);
    }

}
