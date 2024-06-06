using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { START,GOAL,WATER,GRASS,PATH}

public class Astar : MonoBehaviour
{
    //private TileType tileType;

    [SerializeField]
    private Tilemap tilemap;

    //private LayerMask layerMask;
    //  private Tile[] tiles;
    // private RuleTile water;

    private Node current;

    private Stack<Vector3> path;

    private HashSet<Node> openList;

    private HashSet<Node> closedList;

    private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

    private Vector3Int startPos, goalPos;

    public Tilemap MyTilemap { get => tilemap;  }

    private static HashSet<Vector3Int> noDiagonalTiles = new HashSet<Vector3Int>();

    public static HashSet<Vector3Int> NoDiagonalTiles { get => noDiagonalTiles; }

    // private HashSet<Vector3Int> changesTiles = new HashSet<Vector3Int>();

    // private List<Vector3Int> waterTiles = new List<Vector3Int>();

    // private bool start, goal;

    //public Stack<Vector3> MyPath { get => path; set => path = value; }


    public Stack<Vector3> Algorithm(Vector3 start,Vector3 goal)
    {
        startPos = MyTilemap.WorldToCell(start);
        goalPos = MyTilemap.WorldToCell(goal);

        current = GetNode(startPos);   

        openList = new HashSet<Node>();

        closedList = new HashSet<Node>();

        allNodes.Clear();

        openList.Add(current);

        path = null;

        while (openList.Count > 0 && path == null)
        {
            List<Node> neigbors = FindNeighbors(current.Position);

            ExamineNeighbors(neigbors, current);

            UpdateCurrentTile(ref current);

            path = GeneratePath(current);

      
        }

        if (path != null)
        {
            return path;
        }

        return null;
       
    }

    private List<Node> FindNeighbors(Vector3Int parentPosition)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                
                if (y != 0 || x!= 0)
                {
                    //Vector3Int neighborPos = new Vector3Int(parentPosition.x - x, parentPosition.y - y, parentPosition.z);
                    Vector3Int neighborPos = new Vector3Int(parentPosition.x + x, parentPosition.y + y, parentPosition.z);


                    if (neighborPos != startPos &&  !GameManager.MyInstance.Blocked.Contains(neighborPos))
                    { 
                        Node neighbor = GetNode(neighborPos);
                        neighbors.Add(neighbor);
                    }
                   
                }
            }
        }

        return neighbors;
    }

    private void ExamineNeighbors(List<Node> neighbors, Node current)
    {

        for (int i = 0; i < neighbors.Count; i++)
        {

            Node neighbor = neighbors[i];

            if (!ConntedDiagonally(current,neighbor))
            {
                continue;
            }

            int gScore = DetermineGScore(neighbors[i].Position, current.Position);

            if (gScore == 14 && NoDiagonalTiles.Contains(neighbor.Position) && NoDiagonalTiles.Contains(current.Position)) 
            {
                continue;
            }

            if (openList.Contains(neighbor))
            {
                if (current.G + gScore < neighbor.G)
                {
                    CalcValues(current, neighbor,goalPos ,gScore);
                }
            }
            else if (!closedList.Contains(neighbor))
            {
                CalcValues(current, neighbor, goalPos,gScore);

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                
            }

        }
    }

    private void CalcValues(Node parent, Node neighbor,Vector3Int goalPos ,int cost)
    {
        neighbor.Parent = parent;

        neighbor.G = parent.G + cost;

        neighbor.H = ((Math.Abs((neighbor.Position.x - goalPos.x)) + Math.Abs((neighbor.Position.y - goalPos.y))) * 10);

        neighbor.F = neighbor.G + neighbor.H;
    }


    private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
    {
        int gScore = 0;

        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;

        if (Math.Abs(x-y) % 2 == 1)
        {
            gScore = 10;
        }
        else
        {
            gScore = 14;
        }

        return gScore;
    }

    private void UpdateCurrentTile(ref Node current)
    {
        openList.Remove(current);

        closedList.Add(current);

        if (openList.Count > 0)
        {
            current = openList.OrderBy(x => x.F).First();
        }
    }

    private Node GetNode(Vector3Int position)
    {
        if (allNodes.ContainsKey(position))
        {
            return allNodes[position];
        }
        else
        {
            Node node = new Node(position);
            allNodes.Add(position, node);
            return node;
        }
    }


   

    /*
    private void ChangeTile(Vector3Int clickPos)
    {

        if (tileType == TileType.WATER)
        {
            MyTilemap.SetTile(clickPos, water);
            waterTiles.Add(clickPos);
        }
        else
        {
            if (tileType == TileType.START)
            {
                if (start)
                {
                    MyTilemap.SetTile(startPos, tiles[3]);
                }
                start = true;
                startPos = clickPos;
            }
            else if (tileType == TileType.GOAL)
            {
                if (goal)
                {
                    MyTilemap.SetTile(goalPos, tiles[3]);
                }
                goal = true;
                goalPos = clickPos;
            }
            MyTilemap.SetTile(clickPos, tiles[(int)tileType]);
        }

        changesTiles.Add(clickPos);

    }
   */
    /*
    private bool ConntedDiagonally( Node currentNode, Node neighbor)
    {
        Vector3Int direct = currentNode.Position - neighbor.Position;

        Vector3Int first = new Vector3Int(current.Position.x + (direct.x * -1), current.Position.y, current.Position.z);
        Vector3Int second = new Vector3Int(current.Position.x , current.Position.y + (direct.x * -1), current.Position.z);

        if (GameManager.MyInstance.Blocked.Contains(first) || GameManager.MyInstance.Blocked.Contains(second))
        {
            return false;
        }

        return true;
    }*/
    private bool ConntedDiagonally(Node currentNode, Node neighbor)
    {
        Vector3Int direction = neighbor.Position - currentNode.Position;

        Vector3Int first = new Vector3Int(currentNode.Position.x + direction.x, currentNode.Position.y, currentNode.Position.z);
        Vector3Int second = new Vector3Int(currentNode.Position.x, currentNode.Position.y + direction.y, currentNode.Position.z);

        if (GameManager.MyInstance.Blocked.Contains(first) || GameManager.MyInstance.Blocked.Contains(second))
        {
            return false;
        }

        return true;
    }

    private Stack<Vector3> GeneratePath(Node current)
    {
     
        if (current.Position == goalPos)
        {
            Stack<Vector3> finalPath = new Stack<Vector3>();

            /*GameObject aaaa = new GameObject();

            aaaa.name = "target pos";
            aaaa.transform.position = current.Position;*/

            while (current != null) //&& current.Position != null
            {
               
                finalPath.Push(current.Position);

                current = current.Parent;
            }

            

            return finalPath;
         }
        return null;
    }

   /* public void Reset()
    {
       // AstarDebugger.MyInstance.Reset(allNodes);

        foreach (Vector3Int position in changesTiles)
        {
            MyTilemap.SetTile(position, tiles[3]);
        }

        foreach (Vector3Int position in MyPath)
        {
            MyTilemap.SetTile(position, tiles[3]);
        }

        MyTilemap.SetTile(startPos, tiles[3]);
        MyTilemap.SetTile(goalPos, tiles[3]);
        waterTiles.Clear();

        allNodes.Clear();
        start = false;
        goal = false;
        MyPath = null;
        current = null;
    } */
}
