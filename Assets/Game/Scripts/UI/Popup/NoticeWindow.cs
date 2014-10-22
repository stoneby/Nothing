using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class NoticeWindow : Window
{
    private bool haveInit = false;
    private GameObject ItemsContainer;

    private GameObject BgContainer;

    private UIEventListener BgUIEventListener;
    //private UIEventListener UpEventListener;

    #region Window

    public override void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        if (!haveInit)
        {
            haveInit = true;
            ItemsContainer = transform.FindChild("VList").gameObject;
            BgContainer = transform.FindChild("Container click").gameObject;
            BgUIEventListener = UIEventListener.Get(BgContainer);
            
            BgUIEventListener.onClick += BgClickHandler;
            
        }

        var table = ItemsContainer.GetComponent<KxVListRender>();
        table.Init(SystemModelLocator.Instance.NoticeListMsg.NoticeList, 
            "Prefabs/Component/NoticeItem", 705, 490, 705, 88, OnItemClicktHandler, false, true);
    }

    public override void OnExit()
    {
        GlobalWindowSoundController.Instance.PlayCloseSound();
    }

    //private int OpenedUuid;
    private void OnItemClicktHandler(GameObject obj)
    {
        SystemModelLocator.Instance.NoticeItem = obj.GetComponent<NoticeItemControl>();
        if (SystemModelLocator.Instance.NoticeItem.IsOpened)
        {
            SystemModelLocator.Instance.NoticeItem.CloseContent();
        }
        else if (SystemModelLocator.Instance.NoticeItem.NoticeData.Content != "")
        {
            SystemModelLocator.Instance.NoticeItem.SetContent(SystemModelLocator.Instance.NoticeItem.NoticeData.Content);
        }
        else
        {
            var csMsg = new CSGameNoticeDetailMsg();
            csMsg.Uuid = SystemModelLocator.Instance.NoticeItem.NoticeData.Uuid;
            NetManager.SendMessage(csMsg);
        }
    }

    private void BgClickHandler(GameObject obj)
    {
        WindowManager.Instance.Show(typeof(NoticeWindow), false);
    }

    

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion
}
