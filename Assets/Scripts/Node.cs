using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public Vector3 position { get; set; }
    public float prevAngle { get; set; }
    public float maxAngle { get; set; }
    public float scale { get; set; }
    public float length { get; set; }
    private List<Node> children;
    public Node parent { get; private set; }
    public int depth { get; private set; }


	public Node (Vector2 startPosition, float prevAngle, float startMaxAngle, float startScale, float startLength, Node parent, int depth)
    {
        position = startPosition;
        this.prevAngle = prevAngle;
        maxAngle = startMaxAngle;
        scale = startScale;
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
