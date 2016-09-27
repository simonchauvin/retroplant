using UnityEngine;
using System.Collections;

public class PlantManager : MonoBehaviour
{
    public LSystem[] plants;
    public float worldSize;

    private GameObject[] plantss;


    void Start ()
    {
        plantss = new GameObject[3];
        for (int i = 0; i < plants.Length; i++)
        {
            //LSystem plant = Instantiate(plants[i], Random.insideUnitCircle.normalized * worldSize, Quaternion.identity) as LSystem;
            //plant.transform.parent = transform;
        }
    }

    void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Space))
        {
            if (plantss[0] != null)
            {
                Destroy(plantss[0].gameObject);
                Destroy(plantss[1].gameObject);
                Destroy(plantss[2].gameObject);
            }

            LSystem plant = Instantiate(plants[0], new Vector3(0f, 23f, 0f), Quaternion.identity) as LSystem;
            plant.transform.parent = transform;
            plant.originAngle = 180f;
            plantss[0] = plant.gameObject;

            plant = Instantiate(plants[1], new Vector3(-28f, -23f, 0f), Quaternion.identity) as LSystem;
            plant.transform.parent = transform;
            plant.originAngle = -60f;
            plantss[1] = plant.gameObject;

            plant = Instantiate(plants[2], new Vector3(28f, -23f, 0f), Quaternion.identity) as LSystem;
            plant.transform.parent = transform;
            plant.originAngle = 60f;
            plantss[2] = plant.gameObject;
        }
	}
}
