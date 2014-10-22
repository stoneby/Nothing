using KXSGCodec;
using Template.Auto.Quest;
using Template.Auto.Reward;
using Template.Auto.Sign;
using UnityEngine;
using System.Collections;

public class TaskItemControl : MonoBehaviour
{
    private GameObject BgSprite;
    private GameObject TitleLabel;
    private GameObject GetLabel;
    private GameObject RewardItem;
    private GameObject GetButton;

    private GameObject BtnBgScript;
    private GameObject BtnLabel;

    public QusetMsgInfo questInfo;
    public QuestTemplate questTemp;

    private UIEventListener ButtonUIEventListener;

    private UIEventListener GotoUIEventListener;

    public RewardTemplate RewardTemplate;

    public delegate void OnSelectedCallback(GameObject obj);

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    private OnSelectedCallback OnSelected;

    public int SetData(QusetMsgInfo theinfo, OnSelectedCallback selectfunc = null)
    {
        if (BgSprite == null)
        {
            BgSprite = transform.FindChild("Sprite bg").gameObject;
            TitleLabel = transform.FindChild("Label title").gameObject;
            GetLabel = transform.FindChild("Label gift").gameObject;
            RewardItem = transform.FindChild("SignItem").gameObject;
            GetButton = transform.FindChild("Button").gameObject;
            BtnBgScript = transform.FindChild("Button/Background").gameObject;
            BtnLabel = transform.FindChild("Button/Label").gameObject;

            ButtonUIEventListener = UIEventListener.Get(GetButton);
            ButtonUIEventListener.onClick += ClickBtnHandler;

            GotoUIEventListener = UIEventListener.Get(TitleLabel);
            GotoUIEventListener.onClick += GotoBtnHandler;
        }
        OnSelected = selectfunc;
        questInfo = theinfo;
        questTemp = SystemModelLocator.Instance.QuestTemplates.QuestTmpls[questInfo.Id];
        var lb = TitleLabel.GetComponent<UILabel>();
        lb.text = questTemp.Desc;
        //0未完成 1已完成未领取 2 已领取
        if (questInfo.Status == 0)
        {
            BgSprite.SetActive(true);
            GetLabel.SetActive(true);
            RewardItem.SetActive(true);
            var item = RewardItem.GetComponent<SignItemControl>();
            item.SetData(questTemp.RewardId, false, false, SelectHandler);
            RewardTemplate = item.RewardTemplate;
            GetButton.SetActive(false);
            return 0;
        }
        else if (questInfo.Status == 1)
        {
            BgSprite.SetActive(true);
            GetLabel.SetActive(false);
            RewardItem.SetActive(false);
            GetButton.SetActive(true);
            lb = BtnLabel.GetComponent<UILabel>();
            lb.text = "领取";
            lb.color = new Color(250,230,0);
            var btn = GetButton.GetComponent<UIButton>();
            btn.enabled = true;
            BtnBgScript.SetActive(true);
//            var sp = BtnBgScript.GetComponent<UISprite>();
//            sp.color = new Color(255, 255, 255);
            return 0;
        }
        else
        {
            BgSprite.SetActive(false);
            GetLabel.SetActive(false);
            RewardItem.SetActive(false);
            GetButton.SetActive(true);
            lb = BtnLabel.GetComponent<UILabel>();
            lb.text = "已领取";
            lb.color = new Color(100, 100, 100);
            var btn = GetButton.GetComponent<UIButton>();
            btn.enabled = false;
            BtnBgScript.SetActive(false);
            return 1;
        }
    }

    private void ClickBtnHandler(GameObject obj)
    {
        if (questInfo.Status == 1)
        {
            var item = RewardItem.GetComponent<SignItemControl>();
            if (item.CheckFull())
            {
                return;
            }
        }

        if (OnSelected != null)
        {
            OnSelected(gameObject);
        }
    }

    

    private void SelectHandler(GameObject obj)
    {

        if (OnSelected != null)
        {
            OnSelected(gameObject);
        }
    }

    private void GotoBtnHandler(GameObject obj)
    {
        if (questInfo.Status != 0) return;

        WindowManager.Instance.Show<TaskWindow>(false);
        switch (questTemp.LinkWin)
        {
            case 1:
            case 2:
            case 3:
                MissionModelLocator.Instance.ShowRaidWindow(questTemp.LinkWin);
                break;
            case 4://武将界面
                MainMenuBarWindow.OpenHeroWin();
                break;
            case 5://装备界面
                MainMenuBarWindow.OpenEquipWin();
                break;
            case 6://编队
                MainMenuBarWindow.OpenTeamWin();
                break;
            case 7://好友
                var msg = new CSFriendLoadingAll();
                NetManager.SendMessage(msg);
                break;
            case 8://抽卡
                WindowManager.Instance.Show<ChooseCardWindow>(true);
                break;
            default:
                break;
        }
    }
}
