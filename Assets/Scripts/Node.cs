using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    public Vector2 position { get; set; }
    public float unitHeight { get; set; }
    public float prevAngle { get; set; }
    public float maxAngle { get; set; }
    public float length { get; set; }
    private List<Unit> units;
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
        units = new List<Unit>();
    }

    public void addChild (Node newNode)
    {
        children.Add(newNode);
    }

    public void addUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void removeChild(Node oldNode)
    {
        children.Remove(oldNode);
    }

    public List<Node> cut()
    {
        List<Node> nodesToRemove = new List<Node>();
        for (int i = 0; i < children.Count; i++)
        {
            List<Node> nodes = children[i].cut();
            for (int j = 0; j < nodes.Count; j++)
            {
                nodesToRemove.Add(nodes[j]);
            }
        }
        children.Clear();
        children = null;
        for (int i = 0; i < units.Count; i++)
        {
            GameObject.Destroy(units[i].gameObject);
        }
        units.Clear();
        units = null;
        this.parent.removeChild(this);
        this.parent = null;

        return nodesToRemove;
    }

    public List<Node> getChildren ()
    {
        return children;
    }

    public Unit getUnit (int id)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].id == id)
            {
                return units[i];
            }
        }
        return null;
    }

    public List<Unit> getUnits()
    {
        return units;
    }
}
