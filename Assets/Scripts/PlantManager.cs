using UnityEngine;
using System.Collections;

public class PlantManager : MonoBehaviour
{
    public LSystem plantPrefab;

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
                Destroy(plant);
            }

            plant = Instantiate(plantPrefab, new Vector3(0f, 23f, 0f), Quaternion.identity) as LSystem;
            plant.transform.parent = transform;
            plant.originAngle = 180f;
        }
	}
}
