using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AStar
{
    private static AStar instance;

    public Action Recalculate;

    public static AStar Instance
    {
        get
        {
            if (instance == null)
                instance = new AStar();
            return instance;
        }
    }

    public List<TaleModel> FindPath(TaleModel start_tale, TaleModel end_tale, TaleModel[,] obstacleMap, bool is_road = true)
    {
        TaleModel startNode = start_tale;
        TaleModel endNode = end_tale;

        List<TaleModel> openSet = new List<TaleModel>();
        HashSet<TaleModel> closedSet = new HashSet<TaleModel>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            TaleModel currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].DistanceToTarget < currentNode.DistanceToTarget))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.position.x == endNode.position.x && currentNode.position.y == endNode.position.y)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (TaleModel neighbor in (is_road)?currentNode.Neighbors: currentNode.WalkNeighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                int newMovementCostToNeighbor = (int)currentNode.Cost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.Cost || !openSet.Contains(neighbor))
                {
                    neighbor.Cost = newMovementCostToNeighbor;
                    neighbor.DistanceToTarget = GetDistance(neighbor, endNode);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<TaleModel> RetracePath(TaleModel startNode, TaleModel endNode)
    {
        List<TaleModel> path = new List<TaleModel>();
        TaleModel currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        Vector3[] v_path = new Vector3[path.Count];
        for(int i =0; i<path.Count; i++)
        {
            v_path[i] = new Vector3(path[i].position.x, 0, path[i].position.y) ;
        }

        Recalculate?.Invoke();
        return path;
    }

    private int GetDistance(TaleModel nodeA, TaleModel nodeB)
    {
        int dstX = Mathf.Abs((int)nodeA.position.x - (int)nodeB.position.x);
        int dstY = Mathf.Abs((int)nodeA.position.y - (int)nodeB.position.y);

        return dstX + dstY;
    }

    private void PrintRoad(List<TaleModel> Path)
    {
        string path = "";
        for(int i=0; i<Path.Count;i++)
        {
            path += Path[i].position.x + ":" + Path[i].position.y+"|";
        }
        Debug.Log(path);
    }

    
}
