using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public static int nextId;

    public int id { get; private set; }
    public Vector2 position { get; set; }
    public float unitHeight { get; set; }
    public float prevAngle { get; set; }
    public float maxAngle { get; set; }
    public float length { get; set; }
    private List<Unit> units;
    private List<Node> children;
    public Node parent { get; private set; }
    public int depth { get; private set; }


	void Awake ()
    {
        id = Node.nextId;
        Node.nextId++;
        children = new List<Node>();
        units = new List<Unit>();
    }

    public void init(Vector2 startPosition, float unitHeight, float prevAngle, float startMaxAngle, float startLength, Node parent, int depth)
    {
        position = startPosition;
        this.unitHeight = unitHeight;
        this.prevAngle = prevAngle;
        maxAngle = startMaxAngle;
        length = startLength;
        this.parent = parent;
        this.depth = depth;
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

    public void cut ()
    {
        for (int i = 0; i < units.Count; i++)
        {
            GameObject.Destroy(units[i].gameObject);
        }
    }

    public List<Node> getDescendants()
    {
        List<Node> descendants = new List<Node>();
        for (int i = 0; i < children.Count; i++)
        {
            List<Node> returnedDescendants = children[i].getDescendants();
            for (int j = 0; j < returnedDescendants.Count; j++)
            {
                descendants.Add(returnedDescendants[j]);
            }
            descendants.Add(children[i]);
        }
        return descendants;
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
