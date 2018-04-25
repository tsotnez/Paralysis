﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Section : MonoBehaviour {

    public float width = 10f;
    public float height = 10f;
    public InnerSection[] innerSections;
    public Color gizmoColor = Color.cyan;
    public bool showGizmos = false;
    [HideInInspector]
    public bool nonTargetable = false;

    //TODO
    public Vector2[] retreatPoints;

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

    public SectionPath getOptimalPathForSection(Section targetSection, Vector2 currentPosition, Vector2 targetPosition)
    {
        if (targetSection == this || targetSection == null)
        {
            if (targetSection != null)
            {
                Debug.LogError("No path to current section targetSection: " + targetSection.name);
            }
            else
            {
                Debug.LogError("Target section was null." + targetSection.name);
            }
            return null;
        }

        List<SectionPath> paths = new List<SectionPath>();
        foreach(SectionPath sectionP in sectionPaths)
        {
            if (sectionP.TargetSection == null)
            {
                Debug.LogError("Section path target section was null:" + sectionP.name);
            }
            else if(sectionP.TargetSection == targetSection)
            {
                paths.Add(sectionP);
            }
        }

        if(paths.Count == 0)
        {
            Debug.LogError("No paths for section: " + targetSection.name);
            return null;
        }
        else if(paths.Count == 1)
        {
            return paths[0];
        }
        else
        {
            SectionPath retPath = null;
            float actMinDist = float.MaxValue;
            foreach(SectionPath path in paths)
            {
                float dist = Mathf.Abs(Vector2.Distance(path.FirstNode.transform.position, 
                    currentPosition)) + Mathf.Abs(Vector2.Distance(path.LastNode.transform.position, targetPosition));
                if(dist < actMinDist)
                {
                    retPath = path;
                    actMinDist = dist;
                }
            }
            return retPath;
        }
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
        if(position.y <= maxY && position.y >= minY && position.x <= maxX && position.x >= minX)
        {
            return true;
        }
            

        if(innerSections != null && innerSections.Length > 0)
        {
            foreach(InnerSection innerSec in innerSections)
            {
                if(innerSec.contains(position))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Vector2 getBestRetreatPoint(Vector2 currentPos, Vector2 enemyPosition)
    {
        if (retreatPoints == null || retreatPoints.Length == 0)
        {
            return transform.position;
        }
        else if (retreatPoints.Length == 1)
        {
            return retreatPoints[0];
        }
        else
        {
            Vector2 returnPoint = Vector2.zero;
            float maxDistance = -float.MaxValue;
            foreach(Vector2 retreatPoint in retreatPoints)
            {
                float actDistance = Mathf.Abs(Vector2.Distance(retreatPoint, enemyPosition));
                if (actDistance > maxDistance)
                {
                    maxDistance = actDistance;
                    returnPoint = retreatPoint;
                }
            }

            return returnPoint;
        }
    }

    public Vector2[] getRetreatPoints()
    {
        if (retreatPoints == null || retreatPoints.Length == 0)
        {
            return new Vector2[] {transform.position};
        }
        else
        {
            return retreatPoints;
        }
    }

    void OnDrawGizmos()
    {
        if(nonTargetable)return;

        for(int i = 0; i < transform.childCount; i++)
        {            
            SectionPath sectionPath = transform.GetChild(i).GetComponent<SectionPath>();
            if(sectionPath != null)
            {
                sectionPath.showGizmos = showGizmos;
            }
        }

        if(showGizmos){

            Gizmos.color =  gizmoColor;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
            GizmoStar.drawStar(transform.position, gizmoColor, .15f);

            foreach (Vector2 retreatPoint in retreatPoints)
            {
                GizmoStar.drawStar(retreatPoint, gizmoColor, .1f, .66f);
            }

            if(innerSections != null && innerSections.Length > 0){
                foreach(InnerSection innerSec in innerSections)
                {
                    Gizmos.DrawWireCube(innerSec.middle, new Vector3(innerSec.width, innerSec.height));
                }
            }
        }

    }

    [Serializable]
    public class InnerSection {
        public Vector2 middle = Vector2.zero;
        public float width = 5f;
        public float height = 5;

        [HideInInspector]
        public float maxY;
        [HideInInspector]
        public float maxX;
        [HideInInspector]
        public float minY;
        [HideInInspector]
        public float minX;

        public InnerSection()
        {
            maxY = middle.y + (middle.y/2);
            minY = middle.y - (middle.y/2);
            maxX = middle.x + (middle.x/2);
            minX = middle.x - (middle.x/2);
        }

        public bool contains(Vector2 position)
        {
            return position.y <= maxY && position.y >= minY && position.x <= maxX && position.x >= minX;
        }
    }

}
