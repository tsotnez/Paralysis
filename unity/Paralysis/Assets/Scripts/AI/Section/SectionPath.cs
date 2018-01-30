﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionPath : MonoBehaviour {

    public Section TargetSection { get { return targetSection; } }
    private Section targetSection;
    public Section MySection { get { return mySection; } }
    private Section mySection;

    public SectionPathNode[] Nodes { get { return nodes; } }
    private SectionPathNode[] nodes;

    public SectionPathNode FirstNode { get { return nodes[0]; } }
    public SectionPathNode LastNode { get { return nodes[nodes.Length - 1]; } }

    [HideInInspector]
    public bool showGizmos = false;

    void Awake()
    {
        mySection = GetComponentInParent<Section>();
        setNodes();
    }

    void Start()
    {
        targetSection = AISectionManager.Instance.getSectionForPosition(nodes[transform.childCount - 1].transform.position);
    }

    void setNodes()
    {
        int numChildren = transform.childCount;
        nodes = new SectionPathNode[numChildren];

        for(int i = 0; i < numChildren; i++)
        {            
            nodes[i] = transform.GetChild(i).GetComponent<SectionPathNode>();
        }
    }

    void OnDrawGizmos()
    {
        setNodes();
        if(nodes == null || nodes.Length <= 1) return;

        Gizmos.color = Color.green;
        nodes[0].showGizmos = showGizmos;
        for(int i = 1; i < nodes.Length; i++)
        {
            nodes[i].showGizmos = showGizmos;
            if(showGizmos){
                Gizmos.DrawLine(nodes[i-1].transform.position, nodes[i].transform.position);
            }
        }
    }
}
