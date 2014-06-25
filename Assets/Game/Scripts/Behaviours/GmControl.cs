using KXSGCodec;
using UnityEngine;
using System.Collections;

public class GmControl : MonoBehaviour
{
    private GameObject BtnOpen;
    private GameObject BtnConfirm;
    private GameObject GmBox;
    private GameObject InputInfo;

    private bool IsShow = false;
	// Use this for initialization
	void Start ()
	{
        gameObject.SetActive(true);

        BtnOpen = transform.FindChild("Button open").gameObject;
	    var btn = BtnOpen.GetComponent<UIButton>();
        btn.onClick.Add(new EventDelegate(OpenBox));

	    var spobj = BtnOpen.transform.FindChild("Background").gameObject;
        spobj.SetActive(false);

        BtnConfirm = transform.FindChild("Container box/Image Button").gameObject;
        btn = BtnConfirm.GetComponent<UIButton>();
        btn.onClick.Add(new EventDelegate(GmConfirm));

        GmBox = transform.FindChild("Container box").gameObject;
        GmBox.transform.localPosition = new Vector3(0, 450, 0);
        GmBox.SetActive(IsShow);

        InputInfo = transform.FindChild("Container box/Input").gameObject;
        var thein = InputInfo.GetComponent<UIInput>();
        thein.defaultText = "请输入GM指令";
	}
	
	// Update is called once per frame
	void Update ()
	{
	    
	}

    private void OpenBox()
    {
        IsShow = !IsShow;
        GmBox.SetActive(IsShow);
        
        if (IsShow)
        {
            var tween = GmBox.AddComponent<TweenPosition>();
            tween.from = new Vector3(0, 450, 0);
            tween.to = new Vector3(0, -100, 0);
            tween.duration = 0.4f;
            tween.PlayForward();
            Destroy(tween, 0.5f);
        }
        else
        {
            var tween = GmBox.AddComponent<TweenPosition>();
            tween.from = new Vector3(0, -100, 0);
            tween.to = new Vector3(0, 450, 0);
            tween.duration = 0.4f;
            tween.PlayForward();
            Destroy(tween, 0.5f);
        }
    }

    private void GmConfirm()
    {
        var thein = InputInfo.GetComponent<UIInput>();
        var str = thein.value;
        if (str == "" || str == null)
        {
            PopTextManager.PopTip("请输入GM指令");
            return;
        }

        var csMsg = new CSDebugCmdMsg();
        csMsg.Cmd = str;
        NetManager.SendMessage(csMsg);
    }
}
