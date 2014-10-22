using KXSGCodec;
using UnityEngine;
using System.Collections;

public class NoticeItemControl : KxItemRender
{
    public GameNoticeMsgInfo NoticeData;

    private GameObject TitleLabel;
    private GameObject NewSprite;
    private GameObject HotSprite;
    private GameObject DetailLabel;
    private GameObject ArrowDown;

    public bool IsOpened = false;

	// Use this for initialization
	void Start ()
	{
        TitleLabel = transform.FindChild("Label title").gameObject;
        NewSprite = transform.FindChild("Sprite new").gameObject;
        HotSprite = transform.FindChild("Sprite hot").gameObject;
        DetailLabel = transform.FindChild("Label info").gameObject;
	    ArrowDown = transform.FindChild("Sprite arrow down").gameObject;

        NewSprite.SetActive(false);
        HotSprite.SetActive(false);
        ArrowDown.SetActive(false);
        SetContent();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void SetData<T>(T data)
    {
        //throw new System.NotImplementedException();
        NoticeData = data as GameNoticeMsgInfo;
        NoticeData.Content = "";
        SetContent();
    }

    public void SetContent(string str)
    {
        DetailLabel.SetActive(true);
        IsOpened = true;
        var lb = DetailLabel.GetComponent<UILabel>();
        lb.text = str;
        ItemHeight = 88 + lb.height + 20;
        NoticeData.Content = str;
        ArrowDown.SetActive(true);
//        var cld = gameObject.GetComponent<BoxCollider>();
//        cld.size = new Vector3(705, ItemHeight, 0);
//        cld.center = new Vector3(0, 44 - ItemHeight / 2, 0);
        //Logger.Log("设置公告内容" + lb.height);
        //lb.height
    }

    private void SetContent()
    {
        if (DetailLabel == null) return;

        if (NoticeData.Tag == 0)
        {
            HotSprite.SetActive(true);
        }
        else if (NoticeData.Tag == 1)
        {
            NewSprite.SetActive(true);
        }
        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = NoticeData.Title;
    }

    public void CloseContent()
    {
        DetailLabel.SetActive(false);
        ItemHeight = 88;
        IsOpened = false;
        ArrowDown.SetActive(false);
//        var cld = gameObject.GetComponent<BoxCollider>();
//        cld.size = new Vector3(705, ItemHeight, 0);
//        cld.center = new Vector3(0, 0, 0);
    }
}
