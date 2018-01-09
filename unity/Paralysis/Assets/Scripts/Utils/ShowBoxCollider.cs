using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBoxCollider : MonoBehaviour {

	public Color color = Color.red;

	void OnDrawGizmos()
	{
		BoxCollider2D bCollider = GetComponent<BoxCollider2D>();
		Gizmos.color = color;
		Gizmos.DrawWireCube(bCollider.bounds.center, new Vector3(bCollider.bounds.size.x, bCollider.bounds.size.y));
	}
}
