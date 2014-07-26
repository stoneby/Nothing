using UnityEngine;

public class WorldMapFriend : MonoBehaviour
{
    public GameObject Friend;

    public void Sync()
    {
        transform.position = Friend.transform.position;
    }

    private void Update()
    {
        Sync();
    }
}
