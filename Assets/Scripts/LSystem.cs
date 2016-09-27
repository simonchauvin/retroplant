﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LSystem : MonoBehaviour
{
    public Transform[] unitPrefabs;
    public float startSize;
    public int minLength;
    public int maxNumberOfChildren;
    public float angleDecreaseFactor;
    public float lengthDecreaseFactor;
    public float scaleDecreaseFactor;
    public float growthInterval;
	public int startLength;
	public float maxAngle;
    public bool sequenced;

    private List<Node> nodes;
    private Vector2 currentPosition;
    private Node currentNode;
    private Transform currentUnitPrefab;
    private float currentAngle;
    private Vector2 lastPosition;
    private bool ready;
    private int deepest;
    public float originAngle { get; set; }


	void Start ()
    {
        nodes = new List<Node>();
        currentPosition = transform.position;
        // TODO FIX originAngle = Vector3.Angle(Vector3.up, Vector3.zero - transform.position);
        currentAngle = 0f - originAngle;
        lastPosition = currentPosition;
        ready = true;
        deepest = 0;
	}
	
	void Update ()
    {
        // TODO When too many branches cross make all their nodes fall and die
        if (ready)
        {
            // Adding new node
            Node newNode = null;
            if (currentNode == null)
            {
                newNode = new Node(currentPosition, currentAngle, maxAngle * angleDecreaseFactor, startSize * scaleDecreaseFactor, startLength * lengthDecreaseFactor, null, 0);
            }
            else
            {
                newNode = new Node(currentPosition, currentAngle, currentNode.maxAngle * angleDecreaseFactor, currentNode.scale * scaleDecreaseFactor, currentNode.length * lengthDecreaseFactor, currentNode, currentNode.depth + 1);
                if (currentNode.depth + 1 > deepest)
                {
                    deepest = currentNode.depth + 1;
                }
                currentNode.addChildren(newNode);
            }
            nodes.Add(newNode);

            int index = -1;
            List<int> indices = new List<int>();
            float value = Random.value;
            // TODO make it work for more than 2 children
            float randChildren = Random.value;
            // TODO randomize number of children (2-3 is very rare)
            while ((index < 0 || (nodes[index].getChildren().Count > 0 && randChildren > 0.5f) || nodes[index].getChildren().Count >= maxNumberOfChildren || (nodes[index].getChildren().Count > 0 && value < nodes[index].depth / deepest) || nodes[index].length <= minLength) && indices.Count < nodes.Count)
            {
                // TODO compute proba knowing the surrounding nodes children count
                index = Random.Range(0, nodes.Count);
                if (!indices.Contains(index))
                {
                    indices.Add(index);
                }
            }

            // Start growing the new branch
            if (index >= 0 && nodes[index].getChildren().Count < maxNumberOfChildren && nodes[index].length > minLength)
            {
                currentNode = nodes[index];

                // Moving
                // TODO generalize
                /*if (sequenced && currentUnitPrefab)
                {
                    if (currentUnitPrefab.GetComponent<Unit>().type == 0)
                    {
                        currentUnitPrefab = unitPrefabs[1];
                        currentUnitPrefab.GetComponent<Unit>().type = 1;
                    }
                    else
                    {
                        currentUnitPrefab = unitPrefabs[0];
                        currentUnitPrefab.GetComponent<Unit>().type = 0;
                    }
                }
                else
                {
                    currentUnitPrefab = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
                }*/

                currentUnitPrefab = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
                currentPosition = currentNode.position;
                currentAngle = Random.Range(currentNode.maxAngle, -currentNode.maxAngle) - originAngle;
                if (currentNode.getChildren().Count > 0)
                {
                    // TODO make it work for children count > 2
                    while (Mathf.Abs(currentNode.getChildren()[0].prevAngle - currentAngle) < (currentNode.maxAngle * 2f) * 0.3f)
                    {
                        currentAngle = Random.Range(currentNode.maxAngle, -currentNode.maxAngle) - originAngle;
                    }
                }
                float unitHeight = currentUnitPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
                lastPosition = currentPosition;

                // Drawing
                ready = false;
                StartCoroutine("grow");
            }
        }
    }

    IEnumerator grow ()
    {
        int length = Mathf.CeilToInt(currentNode.length);
        float unitHeight = currentUnitPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        for (int i = 0; i < length; i++)
        {
            if ((i == 0 && currentNode.getChildren().Count <= 0) || i > 0)
            {
                Transform unit = Instantiate(currentUnitPrefab, currentPosition, Quaternion.identity) as Transform;
                unit.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -currentAngle));
                unit.localScale = new Vector3(currentNode.scale, currentNode.scale, currentNode.scale);
                unit.parent = transform;

                // TODO should be done only once before starting growing the branch
                RaycastHit2D hitInfo = Physics2D.BoxCast(currentPosition, unit.GetComponent<SpriteRenderer>().bounds.size, -currentAngle, currentPosition - lastPosition, unitHeight, LayerMask.GetMask("Drawing"));
                if (hitInfo)
                {
                    //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Mathf.Infinity);
                    //Debug.DrawLine(currentPosition, currentPosition + (hitInfo.normal), Color.blue, Mathf.Infinity);
                    currentAngle -= Vector3.Angle(currentPosition - lastPosition, hitInfo.normal) - 90f - originAngle;// + Vector3.Angle(Vector3.up, hitInfo.normal);
                    unit.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -currentAngle));
                }
            }

            lastPosition = currentPosition;
            currentPosition.x += unitHeight * currentNode.scale * Mathf.Sin(currentAngle * (Mathf.PI / 180f));
            currentPosition.y += unitHeight * currentNode.scale * Mathf.Cos(currentAngle * (Mathf.PI / 180f));

            currentUnitPrefab = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
            unitHeight = currentUnitPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

            yield return new WaitForSeconds(growthInterval);
        }

        ready = true;
    }
}
