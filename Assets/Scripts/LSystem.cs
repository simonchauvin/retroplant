using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LSystem : MonoBehaviour
{
    public Transform[] unitPrefabsAge0;
    public Transform[] unitPrefabsAge1;
    public Transform[] unitPrefabsAge2;
    public Transform[] unitPrefabsAge3;
    public Transform[] unitPrefabsAge4;
    public Node nodePrefab;
    public int minLength;
    public int maxNumberOfChildren;
    public float angleDecreaseFactor;
    public float lengthDecreaseFactor;
    public float scaleDecreaseFactor;
    public float growthInterval;
    public int startingAge;
	public int startLength;
	public float maxAngle;
    public float thinness;
    public bool sequenced;

    private List<Node> nodes;
    private Node currentNode;
    private bool ready;
    private int deepest;
    public float originAngle { get; set; }


	void Start ()
    {
        ready = true;
        deepest = 0;

        Node newNode = Instantiate(nodePrefab, transform.position, Quaternion.identity) as Node;
        newNode.init(transform.position, getUnitsFromDepth(0)[0].GetComponent<SpriteRenderer>().bounds.size.y, 0f - originAngle, maxAngle, startLength, null, 0);
        newNode.transform.parent = transform;
        Transform unit = Instantiate(getUnitsFromDepth(0)[0], transform.position, Quaternion.identity) as Transform;
        unit.parent = newNode.transform;
        newNode.addUnit(unit.GetComponent<Unit>());
        nodes = new List<Node>();
        nodes.Add(newNode);
        currentNode = nodes[0];
    }
	
	void FixedUpdate ()
    {
        // TODO When too many branches cross make all their nodes fall and die
        if (ready)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.forward, Mathf.Infinity, LayerMask.GetMask("Plant"));
                if (hit)
                {
                    cutOffBranch(hit.collider.GetComponent<Unit>());
                }
            }

            // Node selection
            int index = -1;
            List<int> indices = new List<int>();
            float value = Random.value;
            // TODO make it work for more than 2 children
            float randChildren = Random.value;
            // TODO randomize number of children (2-3 should be rare)
            while ((index < 0 || (nodes[index].getChildren().Count > 0 && randChildren > 0.5f) || nodes[index].getChildren().Count >= maxNumberOfChildren || (nodes[index].getChildren().Count > 0 && deepest > 0 && value < nodes[index].depth / deepest) || nodes[index].length <= minLength) && indices.Count < nodes.Count)
            {
                // TODO compute proba knowing the surrounding nodes children count
                index = Random.Range(0, nodes.Count);
                if (!indices.Contains(index))
                {
                    indices.Add(index);
                }
            }

            // Adding new node
            if (index >= 0 && nodes[index].depth + 1 <= startingAge)
            {
                currentNode = nodes[index];

                int depth = Random.value > 0.7f ? currentNode.depth + 1 : currentNode.depth;
                float length = currentNode.length * lengthDecreaseFactor;
                Vector2 unitSize = getUnitsFromDepth(depth)[0].GetComponent<SpriteRenderer>().bounds.size;
                float currentAngle = Random.Range(currentNode.maxAngle, -currentNode.maxAngle) - originAngle;
                if (currentNode.getChildren().Count > 0)
                {
                    // TODO make it work for children count > 2
                    while (Mathf.Abs(currentNode.getChildren()[0].prevAngle - currentAngle) < currentNode.maxAngle * thinness)
                    {
                        currentAngle = Random.Range(currentNode.maxAngle, -currentNode.maxAngle) - originAngle;
                    }
                }
                Vector2 newNodePosition = currentNode.position + new Vector2(length * unitSize.y * Mathf.Sin(currentAngle * Mathf.Deg2Rad), length * unitSize.y * Mathf.Cos(currentAngle * Mathf.Deg2Rad));
                RaycastHit2D hitInfo = Physics2D.BoxCast(currentNode.position, unitSize, -currentAngle, newNodePosition - currentNode.position, currentNode.unitHeight, LayerMask.GetMask("Drawing"));
                if (hitInfo)
                {
                    //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Mathf.Infinity);
                    //Debug.DrawLine(currentPosition, currentPosition + (hitInfo.normal), Color.blue, Mathf.Infinity);
                    currentAngle -= Vector3.Angle(newNodePosition - currentNode.position, hitInfo.normal) - 90f - originAngle;// + Vector3.Angle(Vector3.up, hitInfo.normal);
                }
                Node newNode = Instantiate(nodePrefab, newNodePosition, Quaternion.identity) as Node;
                newNode.init(newNodePosition, unitSize.y, currentAngle, currentNode.maxAngle * angleDecreaseFactor, length, currentNode, depth);
                newNode.transform.parent = transform;
                if (depth > deepest)
                {
                    deepest = depth;
                }
                currentNode.addChild(newNode);
                nodes.Add(newNode);

                currentNode = newNode;
                ready = false;
                StartCoroutine("grow");
            }
        }
    }

    public void cutOffBranch (Unit unit)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].getUnit(unit.id) != null)
            {
                List<Node> descendants = nodes[i].getDescendants();
                for (int j = 0; j < descendants.Count; j++)
                {
                    nodes.Remove(descendants[j]);
                    descendants[j].cut();
                    GameObject.Destroy(descendants[j].gameObject);
                }
                descendants.Clear();
                nodes[i].parent.removeChild(nodes[i]);
                GameObject.Destroy(nodes[i].gameObject);
                nodes.Remove(nodes[i]);
            }
        }
    }

    IEnumerator grow ()
    {
        int length = Mathf.CeilToInt(currentNode.length);
        int i = 0;
        Vector2 currentPosition = currentNode.position;
        if (currentNode.parent != null)
        {
            currentPosition = currentNode.parent.position;
        }
        Vector2 lastPosition;
        Transform[] units;
        Transform currentUnitPrefab;
        do
        {
            lastPosition = currentPosition;
            currentPosition += new Vector2(currentNode.unitHeight * Mathf.Sin(currentNode.prevAngle * Mathf.Deg2Rad), currentNode.unitHeight * Mathf.Cos(currentNode.prevAngle * Mathf.Deg2Rad));
            units = getUnitsFromDepth(currentNode.depth);
            currentUnitPrefab = units[Random.Range(0, units.Length)];

            Transform unit = Instantiate(currentUnitPrefab, currentPosition, Quaternion.identity) as Transform;
            unit.parent = currentNode.transform;
            currentNode.addUnit(unit.GetComponent<Unit>());

            yield return new WaitForSeconds(growthInterval);
            i++;
        }
        while (i < length);

        ready = true;
    }

    private Transform[] getUnitsFromDepth (int depth)
    {
        if (depth == 0)
        {
            return unitPrefabsAge4;
        }
        else if (depth == 1)
        {
            return unitPrefabsAge3;
        }
        else if (depth == 2)
        {
            return unitPrefabsAge2;
        }
        else if (depth == 3)
        {
            return unitPrefabsAge1;
        }
        else
        {
            return unitPrefabsAge0;
        }
    }
}
