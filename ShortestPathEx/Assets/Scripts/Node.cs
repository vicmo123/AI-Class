using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get { return this.G + this.H; } }
    public Spot P { get; set; }

    public abstract Dictionary<Vector2, Spot>.ValueCollection GetNeighbours(Spot source, List<Vector2> walls);
    public abstract int ComputeH(Spot a, Spot b);
    public abstract bool Equals(Spot anotherNode);
    public abstract int Cost(Spot neighbour);
}
