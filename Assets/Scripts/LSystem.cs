using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LSystem : MonoBehaviour
{
    public Transform unit1Prefab;
    public Transform unit2Prefab;
    public Transform unit3Prefab;
    public Transform unit4Prefab;
    public Transform unit5Prefab;
    public float startSize;
    public int minSize;
    public int maxNumberOfChildren;
    public float angleDecreaseFactor;
    public float lengthDecreaseFactor;
    public float scaleDecreaseFactor;
    public float growthInterval;

	public int depth;
	public string axiom;
	public string varXRewriting;
	public string varFRewriting;
	public int numberOfUnitsPerBranch;
	public float maxAngle;

	private ArrayList currentStack;
	private Vector2 lastBranchPosition;
    private Stack savedPositions;
	private Stack savedAngles;
	private int currentIndex;

    private List<Node> nodes;
    private float unitSize;
    private float growthTimer;


	void Start ()
    {
		currentIndex = 0;
		savedPositions = new Stack();
		savedAngles = new Stack();
		lastBranchPosition = transform.position;
        nodes = new List<Node>();
        nodes.Add(new Node(transform.position, maxAngle, startSize, numberOfUnitsPerBranch));
        unitSize = unit1Prefab.GetComponent<SpriteRenderer>().bounds.size.y;
        growthTimer = 0f;
        currentStack = new ArrayList();
		for (int i = 0; i < axiom.Length; i++)
        {
			currentStack.Add(axiom[i]);
		}
		// Apply rewriting rules
		ArrayList nextStack;
		for (int i = 0; i < depth; i++)
        {
			nextStack = new ArrayList();
			for (int j = 0; j < currentStack.Count; j++)
            {
				string symbol = currentStack[j].ToString();
				switch (symbol)
                {
				    case "F":
					    for (int n = 0; n < varFRewriting.Length; n++)
                        {
						    nextStack.Add(varFRewriting[n]);
					    }
					    break;
				    case "X":
					    for (int n = 0; n < varXRewriting.Length; n++)
                        {
						    nextStack.Add(varXRewriting[n]);
					    }
					    break;
				    case "+":
                        nextStack.Add("+");
                        break;
				    case "-":
                        nextStack.Add("-");
                        break;
				    case "[":
					    nextStack.Add("[");
					    break;
				    case "]":
					    nextStack.Add("]");
					    break;
				}
			}
			currentStack = nextStack;
		}
	}
	
	void Update ()
    {
		// Grow one branch on click hold
		/*if (currentIndex < currentStack.Count)
        {
			Vector2 position = new Vector2();
			string symbol = currentStack[currentIndex].ToString();
			currentIndex++;
			switch (symbol)
            {
			    case "F":
				    // Moving
				    position.x = lastBranchPosition.x + numberOfUnitPerBranch * unitSize * Mathf.Sin(lastAngle * (Mathf.PI / 180f));
				    position.y = lastBranchPosition.y + numberOfUnitPerBranch * unitSize * Mathf.Cos(lastAngle * (Mathf.PI / 180f));
                    // Drawing
                    Transform unit = Instantiate(unit1Prefab, position, Quaternion.identity) as Transform;
                    unit.localScale = new Vector3(lastScale, lastScale, lastScale);
                    lastScale = lastScale * scaleDecreaseFactor;
                    unit.parent = transform;
				    // Current branch is now the previous one
				    lastBranchPosition = position;
				    break;
			    case "+":
				    lastAngle += growthAngle;
				    break;
			    case "-":
				    lastAngle -= growthAngle;
				    break;
			    case "[":
				    // Save angles and positions
				    savedPositions.Push(lastBranchPosition);
				    savedAngles.Push(lastAngle);
				    break;
			    case "]":
				    lastBranchPosition = (Vector2) savedPositions.Pop();
				    lastAngle = (float) savedAngles.Pop();
				    break;
			}
		}*/

        if (growthTimer >= growthInterval)
        {
            int index = -1;
            List<int> indices = new List<int>();
            int hack = 0;
            // TODO randomize number of children (3 is very rare)
            // TODO this rule limits broussaile but should be parametrized to maxNumberOfChildren: (index - 1 >= 0 && nodes[index - 1].getChildren().Count > 0) || (index + 1 < nodes.Count && nodes[index + 1].getChildren().Count > 0)
            while (hack < 500 && (index < 0 || nodes[index].getChildren().Count >= maxNumberOfChildren) && indices.Count < nodes.Count)
            {
                index = Random.Range(0, nodes.Count);
                if (!indices.Contains(index))
                {
                    indices.Add(index);
                }
                hack++;
            }

            if (index >= 0 && nodes[index].getChildren().Count < maxNumberOfChildren && nodes[index].length > minSize)
            {
                Node node = nodes[index];

                // Moving
                Vector2 position = new Vector2();
                float angle = Random.Range(node.maxAngle, -node.maxAngle);

                // Drawing
                // TODO coroutine LERP
                int length = Mathf.CeilToInt(node.length);
                for (int i = node.getChildren().Count > 0 ? 1 : 0; i < length; i++)
                {
                    position.x = node.position.x + i * unitSize * node.scale * Mathf.Sin(angle * (Mathf.PI / 180f));
                    position.y = node.position.y + i * unitSize * node.scale * Mathf.Cos(angle * (Mathf.PI / 180f));
                    
                    Transform unit = Instantiate(unit1Prefab, position, Quaternion.identity) as Transform;
                    unit.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -angle));
                    unit.localScale = new Vector3(node.scale, node.scale, node.scale);
                    unit.parent = transform;
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
