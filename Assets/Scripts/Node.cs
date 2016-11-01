using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public Vector3 position { get; set; }
    public float unitHeight { get; set; }
    public float prevAngle { get; set; }
    public float maxAngle { get; set; }
    public float length { get; set; }
    private List<Node> children;
    public Node parent { get; private set; }
    public int depth { get; private set; }


	public Node (Vector2 startPosition, float unitHeight, float prevAngle, float startMaxAngle, float startLength, Node parent, int depth)
    {
        position = startPosition;
        this.unitHeight = unitHeight;
        this.prevAngle = prevAngle;
        maxAngle = startMaxAngle;
        length = startLength;
        this.parent = parent;
        children = new List<Node>();
        this.depth = depth;
    }

    public void addChildren (Node newNode)
    {
        children.Add(newNode);
    }

    public List<Node> getChildren ()
    {
        return children;
    }
}
