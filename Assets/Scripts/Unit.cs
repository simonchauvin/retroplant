using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public static int nextId;

    public int id { get; private set; }
    public int type { get; set; }
    public int age;


	void Start ()
    {
        id = Unit.nextId;
        Unit.nextId++;
	}
	
	void Update ()
    {
	    
	}
}
