using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionPathNode : MonoBehaviour {
    
    public bool isJumpNode1 = false;
    [Range(-1, 1)]
    public float jumpForce1X = 0;
    public bool isJumpNode2 = false;

    [Range(-1, 1)]
    public float jumpForce2X = 0;
    public bool isFallThroughNode = false;

    public float doubleJumpWait = .5f;

    public Section TargetSection { get { return targetSection; } }
    private Section targetSection;

    [HideInInspector]
    public bool showGizmos = false;

    void Start()
    {
        targetSection = AISectionManager.Instance.getSectionForPosition(transform.position);
    }

    void OnDrawGizmos()
    {
        if(showGizmos){
            GizmoStar.drawStar(transform.position, Color.yellow, .1f, .66f);
        }
    }

}
