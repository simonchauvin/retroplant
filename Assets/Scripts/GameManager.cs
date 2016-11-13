using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private GameManager _instance;


    public GameManager GetInstance ()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<GameManager>();
        }
        return _instance;
    }

	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}
}
