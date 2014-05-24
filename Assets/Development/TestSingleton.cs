using System.Threading;
using UnityEngine;

public class TestSingleton : MonoBehaviour
{
    private void ThreadStart()
    {
        Debug.Log("New thread start");

        SingletonOfMono.Instance.Message = "Hello world";
        SingletonOfMono.Instance.Test = new TestObject();
        Debug.Log(SingletonOfMono.Instance.Message);
        Debug.Log(SingletonOfMono.Instance.Test.Name);
    }

    // Use this for initialization
    void Start()
    {
        var thread = new Thread(ThreadStart);
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class TestObject
{
    public string Name = "TestObject";
}
