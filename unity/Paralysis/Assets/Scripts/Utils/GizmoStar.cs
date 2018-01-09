using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoStar : MonoBehaviour
{
	public Color color = Color.green;
	public float starSize = .25f;

	void OnDrawGizmos() 
	{
		Gizmos.color = color;
		Gizmos.DrawLine (new Vector2 (transform.position.x, transform.position.y - starSize), 
			new Vector2 (transform.position.x, transform.position.y + starSize));
		Gizmos.DrawLine (new Vector2 (transform.position.x + starSize, transform.position.y), 
			new Vector2 (transform.position.x - starSize, transform.position.y));
	}
}
