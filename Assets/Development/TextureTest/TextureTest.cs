using UnityEngine;

public class TextureTest : MonoBehaviour
{
    public bool Visible;

    public void Test()
    {
        gameObject.SetActive(Visible);
        Visible = !Visible;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
