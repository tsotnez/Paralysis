using UnityEngine;
using System.Collections.Generic;


public class Node
{
    public bool walkable;
    public bool grounded;
    public int gridX;
    public int gridY;
    public float price;

    public Vector2 worldPosition;
    public List<Node> neighbors;

    public int gCost;
    public int hCost;
    public Node parent;
    private int heapIndex;

    public Node(float _price, int _gridX, int _gridY)
    {
        walkable = _price != 0.0f;
        price = _price;
        gridX = _gridX;
        gridY = _gridY;
    }

    public Node(bool _walkable, int _gridX, int _gridY)
    {
        walkable = _walkable;
        price = _walkable ? 1f : 0f;
        gridX = _gridX;
        gridY = _gridY;
    }

    public void setNeighbors(List<Node> neighbors)
    {
        this.neighbors = neighbors;
    }

    public void setGrounded(bool grounded)
    {
        this.grounded = grounded;
    }

    public List<Node> getNeighbors()
    {
        return neighbors;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
