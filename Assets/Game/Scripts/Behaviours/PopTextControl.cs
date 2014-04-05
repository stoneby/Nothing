using UnityEngine;
using System.Collections;

public class PopTextControl : MonoBehaviour
{
    private Vector3 temppos;
    private float tempPopHeight;

    public void ShowText(string str, float popheight, Vector3 pos)
    {
        temppos = pos;
        tempPopHeight = popheight;
        var lb = gameObject.GetComponent<UILabel>();
        lb.text = str;
        StartCoroutine(PlayText());
    }

    IEnumerator PlayText()
    {
        var tp = gameObject.GetComponent<TweenPosition>();
        tp.from = new Vector3(temppos.x, temppos.y, temppos.z);
        tp.to = new Vector3(temppos.x, temppos.y + tempPopHeight, temppos.z);
        tp.duration = 0.2f;
        tp.PlayForward();
        yield return new WaitForSeconds(.3f);
        tp.duration = 0.5f;
        tp.PlayReverse();
    }
}
