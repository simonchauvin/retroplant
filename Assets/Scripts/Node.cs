using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public Vector3 position { get; set; }
    public float maxAngle { get; set; }
    public float scale { get; set; }
    public float length { get; set; }
    private List<Node> children;


	public Node (Vector2 startPosition, float startMaxAngle, float startScale, float startLength)
    {
        position = startPosition;
        maxAngle = startMaxAngle;
        scale = startScale;
        length = startLength;
        children = new List<Node>();
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
