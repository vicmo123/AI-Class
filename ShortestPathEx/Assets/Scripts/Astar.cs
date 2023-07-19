using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public static int MapWidth { get; set; }
    public static int MapHeight { get; set; }

    public static List<Spot> FindshortestPath(Vector2 start, Vector2 destination, List<Vector2> wallPos)
    {
        //Astart algorithm
        List<Spot> closedList = new List<Spot>();
        List<Spot> opennedList = new List<Spot>();
        List<Vector2> walls = wallPos;

        Spot startSpot = new Spot((int)start.x, (int)start.y, 1);
        Spot finishSpot = new Spot((int)destination.x, (int)destination.y, 1);

        opennedList.Add(startSpot);
        startSpot.ComputeH(startSpot, finishSpot);
        startSpot.G = 0;
        startSpot.P = null;

        bool pathFound = false;

        while (!(opennedList.Count == 0) && !pathFound)
        {
            Spot node = Astar.FindNode(opennedList);

            opennedList.Remove(node);
            closedList.Add(node);

            if (node.Equals(finishSpot))
            {
                pathFound = true;
            }
            else
            {
                foreach (Spot neighbour in node.GetNeighbours(node, walls))
                {
                    if (!Astar.Contains(closedList, neighbour))
                    {
                        if (!Astar.Contains(opennedList, neighbour))
                        {
                            neighbour.H = neighbour.ComputeH(neighbour, finishSpot);
                            neighbour.G = node.G + node.Cost(neighbour);
                            neighbour.P = node;

                            opennedList.Add(neighbour);
                        }
                        else
                        {
                            Spot neighbourInOpenedList = Astar.GetNode(opennedList, neighbour);

                            if (neighbourInOpenedList.G > node.G + node.Cost(neighbour))
                            {
                                neighbourInOpenedList.G = node.G + node.Cost(neighbour);
                                neighbourInOpenedList.P = node;
                            }
                        }
                    }
                }
            }
        }

        //  compute best path if found
        List<Spot> path = null;
        if (pathFound)
        {
            path = new List<Spot>();
            Spot currentNode = closedList[closedList.Count - 1];    //  destination node...
            while (!currentNode.Equals(startSpot))
            {
                path.Add(currentNode);
                currentNode = currentNode.P;
            }
            path.Add(startSpot);
        }

        return path;
    }

    private static Spot FindNode(List<Spot> listOfNodes)
    {
        Spot foundNode = listOfNodes[0];

        foreach (Spot n in listOfNodes)
        {
            if (n.F < foundNode.F)
            {
                foundNode = n;
            }
        }

        return foundNode;
    }

    private static bool Contains(List<Spot> list, Spot node)
    {
        foreach (Spot n in list)
        {
            if (n.Equals(node))
            {
                return true;
            }
        }
        return false;
    }

    private static Spot GetNode(List<Spot> list, Spot node)
    {
        foreach (Spot n in list)
        {
            if (n.Equals(node))
            {
                return n;
            }
        }

        return null;
    }
}

public class Spot : Node
{
    public int x { get; set; }
    public int y { get; set; }
    public int width { get; set; }

    public Spot(int _x, int _y, int _width)
    {
        x = _x;
        y = _y;

        width = _width;
    }

    public override int ComputeH(Spot a, Spot b)
    {
        var dx = System.Math.Abs(a.x - b.x);
        var dy = System.Math.Abs(a.y - b.y);
        return 1 * (dx + dy);
    }

    public override int Cost(Spot neighbour)
    {
        return 1;
    }

    public override bool Equals(Spot anotherNode)
    {
        if (this.x == anotherNode.x && this.y == anotherNode.y)
            return true;
        else
            return false;
    }

    public override Dictionary<Vector2, Spot>.ValueCollection GetNeighbours(Spot source, List<Vector2> walls)
    {
        var neighbors = new Dictionary<Vector2, Spot>();

        Vector2 right = new Vector2(source.x + 1, source.y);
        Vector2 left = new Vector2(source.x - 1, source.y);
        Vector2 top = new Vector2(source.x, source.y + 1);
        Vector2 bottom = new Vector2(source.x, source.y - 1);

        neighbors[right] = new Spot((int)right.x, (int)right.y, 1);
        neighbors[left] = new Spot((int)left.x, (int)left.y, 1);
        neighbors[top] = new Spot((int)top.x, (int)top.y, 1);
        neighbors[bottom] = new Spot((int)bottom.x, (int)bottom.y, 1);

        foreach (Vector2 wall in walls)
        {
            //Remove neighbour if its a wall
            if (wall == right)
                neighbors.Remove(right);
            if(wall == left)
                neighbors.Remove(left);
            if (wall == top)
                neighbors.Remove(top);
            if (wall == bottom)
                neighbors.Remove(bottom);
        }

        //Remove neighbour if its out of bound(Map)
        if (right.x >= Astar.MapWidth + 1)
            neighbors.Remove(right);
        if (left.x <= -1)
            neighbors.Remove(left);
        if (top.y >= Astar.MapHeight + 1)
            neighbors.Remove(top);
        if (bottom.y <= -1)
            neighbors.Remove(bottom);

        return neighbors.Values;
    }
}