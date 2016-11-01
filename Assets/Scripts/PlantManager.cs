using UnityEngine;
using System.Collections;

public class PlantManager : MonoBehaviour
{
    public LSystem plantPrefab;
    public Transform startingPoint;

    private LSystem plant;


    void Start ()
    {
        
    }

    void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Space))
        {
            if (plant != null)
            {
                Destroy(plant.gameObject);
            }

            plant = Instantiate(plantPrefab, startingPoint.position, Quaternion.identity) as LSystem;
            plant.transform.parent = transform;
            plant.originAngle = 0f;
        }
	}
}
