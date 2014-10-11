using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specific window controller.
/// </summary>
public class SignWindow : Window
{
    private GameObject TitleLabel;
    private GameObject CountLabel;
    private GameObject ItemContainer;
    private GameObject ItemPrefab;
    private GameObject SignConfirm;
    private GameObject SignCurrencyInfo;
    private GameObject SignItemInfo;


    private List<GameObject> Items; 

    private UIEventListener BgUIEventListener;
    #region Window

    public override void OnEnter()
    {
        if (TitleLabel == null)
        {
            TitleLabel = transform.FindChild("Label title").gameObject;
            CountLabel = transform.FindChild("Label ocunt").gameObject;
            ItemContainer = transform.FindChild("Container").gameObject;
            SignConfirm = transform.FindChild("Panel/SignConfirm").gameObject;
            SignCurrencyInfo = transform.FindChild("Panel/SignCurrencyInfo").gameObject;
            SignItemInfo = transform.FindChild("Panel/SignItemInfo").gameObject;
            ItemPrefab = Resources.Load("Prefabs/Component/SignItem") as GameObject;

            BgUIEventListener = UIEventListener.Get(transform.FindChild("Sprite bg").gameObject);
            BgUIEventListener.onClick += ClickHandler;

            Items = new List<GameObject>();
        }

        var lb = TitleLabel.GetComponent<UILabel>();
        var m = SystemModelLocator.Instance.SignLoadMsg.Month + 1;
        lb.text = m + "月签到奖励";
        lb = CountLabel.GetComponent<UILabel>();
        lb.text = "本月累计签到次数:" + SystemModelLocator.Instance.SignLoadMsg.SignTimes;


        while (Items.Count < SystemModelLocator.Instance.SignLoadMsg.DayOfMonth)
        {
            Items.Add(NGUITools.AddChild(ItemContainer, ItemPrefab));
            //var theitem = Items[Items.Count - 1].GetComponent<KxItemRender>();
        }

        while (Items.Count > SystemModelLocator.Instance.SignLoadMsg.DayOfMonth)
        {
            var theitem = Items[Items.Count - 1];
            theitem.SetActive(false);
            Items.RemoveAt(Items.Count - 1);
        }

        var sign = SystemModelLocator.Instance.SighTemplates.SignTmpls[m];
        //var rewards = SystemModelLocator.Instance.RewardTemplates.RewardTmpls;

        var basex = -430;
        var basey = 180;
        var offsetx = 122;
        var offsety = -118;
        int xx = 0;
        int yy = 0;

        //PopTextManager.PopTip("CanSignNode " + SystemModelLocator.Instance.SignLoadMsg.CanSignNode);
        for (int i = 0; i < SystemModelLocator.Instance.SignLoadMsg.DayOfMonth; i++)
        {
            //SystemModelLocator.Instance.SighTemplates.SignTmpls
            var theitem = Items[i];
            theitem.transform.localPosition = new Vector3(basex + offsetx * xx, basey + offsety * yy, 0);
            var item = theitem.GetComponent<SignItemControl>();
            item.SetData(
                sign.RewardId[i], 
                i < SystemModelLocator.Instance.SignLoadMsg.SignTimes,
                i == SystemModelLocator.Instance.SignLoadMsg.CanSignNode - 1,
                OnSelectHandler);
            xx++;
            if (xx >= 8)
            {
                xx = 0;
                yy++;
            }
        }
    }

    public override void OnExit()
    {
    }

    private void OnSelectHandler(GameObject obj)
    {
        var item = obj.GetComponent<SignItemControl>();
        if (item.CanSelect)
        {
            SystemModelLocator.Instance.RewardId = item.RewardId;
            SignConfirm.SetActive(true);
            var com = SignConfirm.GetComponent<SignConfirmWindow>();
            com.ConfirmType = "sign";
            com.OnEnter();

        }
        else if (item.RewardTemplate.RewardType == SignItemControl.SIGN_REWARD_TYPE_HERO)
        {
            SignItemInfo.SetActive(true);
            var info = SignItemInfo.GetComponent<SignItemInfo>();
            info.OnEnterHero(item.RewardTemplate);
        }
        else if (item.RewardTemplate.RewardType == SignItemControl.SIGN_REWARD_TYPE_ITEM)
        {
            SignItemInfo.SetActive(true);
            var info = SignItemInfo.GetComponent<SignItemInfo>();
            info.OnEnterEquip(item.RewardTemplate);
        }
        else
        {
            var info = SignCurrencyInfo.GetComponent<SignCourencyInfo>();
            SignCurrencyInfo.SetActive(true);
            info.OnEnter(item.RewardId);
        }
       
    }

    private void ClickHandler(GameObject obj)
    {
        WindowManager.Instance.Show<SignWindow>(false);
    }

    #endregion

    #region Mono

    // Use this for initialization
    void Start()
    {
    }

    #endregion
}
