using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LSystem : MonoBehaviour
{
    public Transform[] unitPrefabs;
    public float startSize;
    public int minSize;
    public int maxNumberOfChildren;
    public float angleDecreaseFactor;
    public float lengthDecreaseFactor;
    public float scaleDecreaseFactor;
    public float growthInterval;
	public int numberOfUnitsPerBranch;
	public float maxAngle;

    private List<Node> nodes;
    private float growthTimer;


	void Start ()
    {
        nodes = new List<Node>();
        nodes.Add(new Node(transform.position, maxAngle, startSize, numberOfUnitsPerBranch));
        growthTimer = 0f;
	}
	
	void Update ()
    {
        if (growthTimer >= growthInterval)
        {
            int index = -1;
            List<int> indices = new List<int>();
            // TODO randomize number of children (3 is very rare)
            // TODO this rule limits broussaile but should be parametrized to maxNumberOfChildren: (index - 1 >= 0 && nodes[index - 1].getChildren().Count > 0) || (index + 1 < nodes.Count && nodes[index + 1].getChildren().Count > 0)
            while ((index < 0 || nodes[index].getChildren().Count >= maxNumberOfChildren || nodes[index].length <= minSize) && indices.Count < nodes.Count)
            {
                index = Random.Range(0, nodes.Count);
                if (!indices.Contains(index))
                {
                    indices.Add(index);
                }
            }

            if (index >= 0 && nodes[index].getChildren().Count < maxNumberOfChildren && nodes[index].length > minSize)
            {
                Node node = nodes[index];

                // Moving
                Transform unitPrefab = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
                Vector2 position = node.position;
                float angle = Random.Range(node.maxAngle, -node.maxAngle);
                float unitHeight = unitPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
                Vector2 lastPosition = position;

                // Drawing
                // TODO coroutine LERP
                int length = Mathf.CeilToInt(node.length);
                for (int i = 0; i < length; i++)
                {
                    if ((i == 0 && node.getChildren().Count <= 0) || i > 0)
                    {
                        Transform unit = Instantiate(unitPrefab, position, Quaternion.identity) as Transform;
                        unit.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));
                        unit.localScale = new Vector3(node.scale, node.scale, node.scale);
                        unit.parent = transform;

                        RaycastHit2D hitInfo = Physics2D.BoxCast(position, unit.GetComponent<SpriteRenderer>().bounds.size, -angle, position - lastPosition, unitHeight, LayerMask.GetMask("Drawing"));
                        if (hitInfo)
                        {
                            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Mathf.Infinity);
                            Debug.DrawLine(position, position + (hitInfo.normal), Color.blue, Mathf.Infinity);
                            angle -= Vector3.Angle(position - lastPosition, hitInfo.normal) * 2f;// + Vector3.Angle(Vector3.up, hitInfo.normal);
                            unit.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));
                        }
                    }

                    lastPosition = position;
                    position.x += unitHeight * node.scale * Mathf.Sin(angle * (Mathf.PI / 180f));
                    position.y += unitHeight * node.scale * Mathf.Cos(angle * (Mathf.PI / 180f));
                    
                    unitPrefab = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
                    unitHeight = unitPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
                }

                Node newNode = new Node(position, node.maxAngle * angleDecreaseFactor, node.scale * scaleDecreaseFactor, node.length * lengthDecreaseFactor);
                nodes.Add(newNode);
                node.addChildren(newNode);
            }

            growthTimer = 0f;
        }
        else
        {
            growthTimer += Time.deltaTime;
        }
	}
}
