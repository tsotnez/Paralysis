using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoStar : MonoBehaviour
{
	public Color color = Color.green;
	public float starSize = .25f;

    public static void drawStar(Vector3 position, Color color, float starSizeX, float startSizeY)
    {        
        Gizmos.color = color;
        Gizmos.DrawLine (new Vector2 (position.x, position.y - startSizeY), new Vector2 (position.x, position.y + startSizeY));
        Gizmos.DrawLine (new Vector2 (position.x + starSizeX, position.y), new Vector2 (position.x - starSizeX, position.y));        
    }

    public static void drawStar(Vector3 position, Color color, float starSize)
    {
        drawStar(position, color, starSize, starSize);
    }

	void OnDrawGizmos() 
	{
        drawStar(transform.position, color, starSize);
	}
}
