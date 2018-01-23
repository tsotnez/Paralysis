using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public bool showGridGrid = false;
    public LayerMask unwalkableMask;

    public int width = 50;
    public int height = 50;
    private Vector2 gridWorldSize;
    public const float NODE_DIAMETER = .5f;
    public const float NODE_RADIUS = NODE_DIAMETER / 2;

    private Grid grid;
    public Grid Grid { get { return grid; } }
    public int MaxSize { get { return gridSizeX * gridSizeY; } }

    private int gridSizeX;
    private int gridSizeY;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        InitializeGrid();
    }

    private void InitializeGrid()
    {        
        gridWorldSize = new Vector2(width, height);

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / NODE_DIAMETER);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / NODE_DIAMETER);

        bool[,] tilesmap = new bool[gridSizeX, gridSizeY];
        grid = new Grid(gridSizeX, gridSizeY, tilesmap);

        //Set walkable nodes
        foreach (Node n in grid.nodes)
        {
            n.worldPosition = getPointFromNode(n);
            n.walkable = !Physics2D.OverlapCircle(new Vector3(n.worldPosition.x, n.worldPosition.y, 0), NODE_RADIUS, unwalkableMask);
        }

        //Set grounded nodes
        foreach(Node n in grid.nodes)
        {
            if(n.walkable)
            {
                RaycastHit2D hit = Physics2D.Raycast(n.worldPosition, Vector2.down, NODE_DIAMETER + .1f, unwalkableMask);
                if(hit)
                {
                    n.setGrounded(true);
                }
            }
        }
    }

    public Node getNodeFromPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        Node retNode = grid.nodes[x, y];

        return retNode;
    }

    public Vector2 getPointFromNode(Node node)
    {
        int x = node.gridX;
        int y = node.gridY;

        float startX = (transform.position.x - (width / 2)) + (NODE_RADIUS);
        float startY = (transform.position.y - (height / 2)) + (NODE_RADIUS);

        float newX = startX + (x * NODE_DIAMETER);
        float newY = startY + (y * NODE_DIAMETER);

        return new Vector2(newX, newY);
    }

    void OnDrawGizmos()
    {
        if (!showGridGrid) return;

        Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX * NODE_DIAMETER, gridSizeY * NODE_DIAMETER, 1));
        float size = NODE_DIAMETER - .1f;

        Color whiteC = new Color(1, 1, 1, .5f);
        Color redC = new Color(1, 0, 0, .5f);
        Color greenC = new Color(0, 1, 0, .5f);

        InitializeGrid();
        foreach (Node n in grid.nodes)
        {
            Gizmos.color = (n.walkable) ? whiteC : redC;
            if(n.grounded)Gizmos.color = greenC;
            Gizmos.DrawCube(n.worldPosition, Vector3.one * (size));
        }
    }
}
