using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int Counter { get; set; }

    public void Play()
    {
        Logger.Log("Play");    
    }

    public void Print()
    {
        Logger.Log("Counter: " + Counter);
    }

    void Start()
    {
        Logger.Log("I am singleton." + name);
    }
}
