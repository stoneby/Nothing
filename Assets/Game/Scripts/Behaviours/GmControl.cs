using KXSGCodec;
using UnityEngine;

public class GmControl : MonoBehaviour
{
    private GameObject btnOpen;
    private GameObject btnConfirm;
    private GameObject gmBox;
    private GameObject inputInfo;

    private bool isShow;
    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(true);

        btnOpen = transform.FindChild("Button open").gameObject;
        var btn = btnOpen.GetComponent<UIButton>();
        btn.onClick.Add(new EventDelegate(OpenBox));

        var spobj = btnOpen.transform.FindChild("Background").gameObject;
        spobj.SetActive(false);

        btnConfirm = transform.FindChild("Container box/Image Button").gameObject;
        btn = btnConfirm.GetComponent<UIButton>();
        btn.onClick.Add(new EventDelegate(GmConfirm));

        gmBox = transform.FindChild("Container box").gameObject;
        gmBox.transform.localPosition = new Vector3(0, 450, 0);
        gmBox.SetActive(isShow);

        inputInfo = transform.FindChild("Container box/Input").gameObject;
        var thein = inputInfo.GetComponent<UIInput>();
        thein.defaultText = "请输入GM指令";
    }

    private void OpenBox()
    {
        if (!ServiceManager.IsTest)
        {
            return;
        }

        isShow = !isShow;
        gmBox.SetActive(isShow);

        if (isShow)
        {
            var tween = gmBox.AddComponent<TweenPosition>();
            tween.from = new Vector3(0, 450, 0);
            tween.to = new Vector3(0, -100, 0);
            tween.duration = 0.4f;
            tween.PlayForward();
            Destroy(tween, 0.5f);
        }
        else
        {
            var tween = gmBox.AddComponent<TweenPosition>();
            tween.from = new Vector3(0, -100, 0);
            tween.to = new Vector3(0, 450, 0);
            tween.duration = 0.4f;
            tween.PlayForward();
            Destroy(tween, 0.5f);
        }
    }

    private void GmConfirm()
    {
        var thein = inputInfo.GetComponent<UIInput>();
        var str = thein.value;
        if (string.IsNullOrEmpty(str))
        {
            PopTextManager.PopTip("请输入GM指令");
            return;
        }

        var csMsg = new CSDebugCmdMsg();
        csMsg.Cmd = str;
        NetManager.SendMessage(csMsg);
    }
}
