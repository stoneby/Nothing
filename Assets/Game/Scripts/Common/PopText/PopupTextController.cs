using System.Collections;
using UnityEngine;

public class PopupTextController : Singleton<PopupTextController>
{
    public GameObject PopupLabel;

    public float UpValue;
    public float BottonValue;

    public float UpDuration;
    public float BottomDuration;

    public iTween.EaseType EasyType;

    public float TotalDuration
    {
        get { return UpDuration + BottomDuration; }
    }

    public void Show(GameObject parent, int value)
    {
        var popClone = NGUITools.AddChild(parent, PopupLabel);
        var label = popClone.GetComponent<UILabel>();
        label.text = "-" + value;

        StartCoroutine(DoShow(popClone));
    }

    private IEnumerator DoShow(GameObject popClone)
    {
        iTween.MoveTo(popClone,
            iTween.Hash("position", (popClone.transform.localPosition + new Vector3(0, UpValue, 0)), "time", UpDuration, "easetype", EasyType.ToString(),
                "islocal", true));

        yield return new WaitForSeconds(UpDuration);

        iTween.MoveTo(popClone,
            iTween.Hash("position", (popClone.transform.localPosition + new Vector3(0, BottonValue, 0)), "time", BottomDuration, "easetype", EasyType.ToString(),
                "islocal", true));

        yield return new WaitForSeconds(BottomDuration);

        Destroy(popClone);
    }
}
