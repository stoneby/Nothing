using UnityEngine;
using System.Collections;

public class SingletonOfMono : MonoBehaviour
{
    private static SingletonOfMono instance;

    public static SingletonOfMono Instance
    {
        get { return instance ?? (instance = new SingletonOfMono()); }
    }

    public string Message;
    public TestObject Test;

    public SingletonOfMono()
    {
        Debug.Log("This is constructor");
    }

    void Awake()
    {
        Debug.Log("I am awake.");
    }

	// Use this for initialization
	void Start () {
	    Debug.Log("I am start");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
