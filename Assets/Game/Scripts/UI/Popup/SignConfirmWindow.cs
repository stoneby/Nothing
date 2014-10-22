using KXSGCodec;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class SignConfirmWindow : MonoBehaviour
{
    private UIEventListener BgUIEventListener;
    private UIEventListener BtnUIEventListener;

    private GameObject TitleLabel;
    private GameObject CountLabel;
    private GameObject IconItem;

    public string ConfirmType = "sign";

    #region Window

    public void OnEnter()
    {
        GlobalWindowSoundController.Instance.PlayOpenSound();
        if (TitleLabel == null)
        {
            TitleLabel = transform.FindChild("Label name").gameObject;
            CountLabel = transform.FindChild("Label count").gameObject;
            IconItem = transform.FindChild("SignItem").gameObject;

            BgUIEventListener = UIEventListener.Get(transform.FindChild("Container click").gameObject);
            BgUIEventListener.onClick += ClickBgHandler;

            BtnUIEventListener = UIEventListener.Get(transform.FindChild("Button").gameObject);
            BtnUIEventListener.onClick += ClickBtnHandler;
        }

        var item = IconItem.GetComponent<SignItemControl>();
        item.SetData(SystemModelLocator.Instance.RewardId, false, false);
        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = item.Name;
        lb = CountLabel.GetComponent<UILabel>();
        lb.text = "ÊýÁ¿£º" + item.Count;
    }

    private void ClickBgHandler(GameObject obj)
    {
        gameObject.SetActive(false);
        GlobalWindowSoundController.Instance.PlayCloseSound();
    }

    private void ClickBtnHandler(GameObject obj)
    {
        if (ConfirmType == "sign")
        {
            var msg = new CSSign();
            msg.Month = SystemModelLocator.Instance.SignLoadMsg.Month;
            NetManager.SendMessage(msg);
        }
        else
        {
            var msg = new CSQuestRecieveReward();
            msg.Id = SystemModelLocator.Instance.QuestId;
            msg.QuestType = 1;
            NetManager.SendMessage(msg);
        }
        gameObject.SetActive(false);
        //WindowManager.Instance.Show<SignConfirmWindow>(false);
    }


    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion
}
