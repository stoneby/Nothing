using UnityEngine;

public class EnemySpriteControl : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var sp = gameObject.GetComponent<UISprite>();
        sp.color = new Color(0.015f, 0.015f, 0.015f, 1);
    }

    public void OnScallComplete()
    {
        iTweenEvent.GetEvent(gameObject, "ScallEndTween").Play();
    }
}
