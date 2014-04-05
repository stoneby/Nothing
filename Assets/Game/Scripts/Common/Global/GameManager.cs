using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int Counter { get; set; }

    public void Play()
    {
        Debug.Log("Play");    
    }

    public void Print()
    {
        Debug.Log("Counter: " + Counter);
    }

    void Awake()
    {
        Debug.Log("I am singleton." + name);
    }
}
