using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System;

public class PathFinding  : MonoBehaviour {

    private const int DIAG_COST = 13;
    private const int COST = 10;
    private const int GROUNDED_DISCOUNT = 5;
    private Grid grid;

    void Start()
    {
        grid = GridManager.Instance.Grid;
    }

    public void FindPath(PathRequest request, Action<PathResult> callback, bool ignorePrices)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = GridManager.Instance.getNodeFromPoint(request.pathStart);
        Node targetNode = GridManager.Instance.getNodeFromPoint(request.pathEnd);

        //if the target node is not walkable 
        if(!targetNode.walkable)
        {
            bool foundNode = false;
            if(targetNode.getNeighbors() == null)
            {
                targetNode.setNeighbors(grid.GetNeighbours(targetNode));
            }

            foreach(Node node in targetNode.getNeighbors())
            {
                if(node.walkable)
                {
                    foundNode = true;
                    targetNode = node;
                    break;
                }
            }

            if(!foundNode)
            {
                UnityEngine.Debug.LogError("Unable to find suitable node for target.");
                callback(new PathResult(null, false, request.callback));
            }
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = true;
                break;
            }

            // set neighbors if we havent yet...
            if (currentNode.getNeighbors() == null)
            {
                currentNode.setNeighbors(grid.GetNeighbours(currentNode));
            }

            foreach (Node neighbour in currentNode.getNeighbors())
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) * (ignorePrices ? 1 : (int)(10.0f * neighbour.price));
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }

        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        int retVal = 0;

        if (dstX > dstY)
        {
            retVal = DIAG_COST * dstY + COST * (dstX - dstY);
        }
        else
        {            
            retVal = DIAG_COST * dstX + COST * (dstY - dstX);
        }

        if(nodeB.grounded)retVal = retVal - GROUNDED_DISCOUNT;

        return retVal;
    }
}
