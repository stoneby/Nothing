using System.Collections.Generic;
using Template.Auto.Reward;
using UnityEngine;

public class SignItemControl : MonoBehaviour
{
    /** 签到奖励类型-武将 */
	public static int SIGN_REWARD_TYPE_HERO = 1;
	/** 签到奖励类型-道具 */
	public static int SIGN_REWARD_TYPE_ITEM = 2;
	/** 签到奖励类型-货币 */
	public static int SIGN_REWARD_TYPE_CURRENCY = 3;


    private GameObject GoodsIcon;
    private GameObject ItemIcon;
    private GameObject HeadIcon;
    private GameObject HeadContainer;
    private GameObject JobSprite;
    private List<GameObject> Stars;
    private GameObject CountLabel;
    private GameObject SelectSprite;
    private GameObject ClickSprite;

    private UIEventListener BtnUIEventListener;

    public delegate void OnSelectedCallback(GameObject obj);

    /// <summary>
    /// Notification triggered when swipe happens.
    /// </summary>
    private OnSelectedCallback OnSelected;

    public EffectSpawner LightEffect;

	// Use this for initialization
	void Start () 
    {
        Stars = new List<GameObject>();
        Stars.Add(transform.FindChild("Container item/Sprite star1").gameObject);
        Stars.Add(transform.FindChild("Container item/Sprite star2").gameObject);
        Stars.Add(transform.FindChild("Container item/Sprite star3").gameObject);
        Stars.Add(transform.FindChild("Container item/Sprite star4").gameObject);
        Stars.Add(transform.FindChild("Container item/Sprite star5").gameObject);

        ItemIcon = transform.FindChild("Sprite item").gameObject;
        GoodsIcon = transform.FindChild("Sprite coin").gameObject;
        HeadIcon = transform.FindChild("Sprite hero").gameObject;
        HeadContainer = transform.FindChild("Container item").gameObject;
        JobSprite = transform.FindChild("Container item/Sprite job").gameObject;
        CountLabel = transform.FindChild("Label count").gameObject;

        SelectSprite = transform.FindChild("Sprite select").gameObject;
        ClickSprite = transform.FindChild("Sprite click").gameObject;

        BtnUIEventListener = UIEventListener.Get(gameObject);
        BtnUIEventListener.onClick += ClickHandler;

        SelectSprite.SetActive(false);
        ClickSprite.SetActive(false);

	    SetContent();
    }

    private void ClickHandler(GameObject obj)
    {
        if (OnSelected != null)
        {
            OnSelected(gameObject);
        }
    }

    public bool CheckFull()
    {
        bool k = false;
        if (RewardTemplate.RewardType == SIGN_REWARD_TYPE_HERO)
        {
            k = HeroModelLocator.Instance.IsHeroFull();
            if (k)
            {
                Alert.Show(AssertionWindow.Type.Ok, "系统提示", "武将列表已满，点击“确定”前往武将界面？", AlertHeroHandler);
            }
        }
        else if (RewardTemplate.RewardType == SIGN_REWARD_TYPE_ITEM)
        {
            k = ItemModeLocator.Instance.IsItemFull();
            if (k)
            {
                Alert.Show(AssertionWindow.Type.Ok, "系统提示", "武将列表已满，点击“确定”前往武将界面？", AlertItemHandler);
            }
        }
        return k;
    }

    private static void AlertHeroHandler(GameObject sender = null)
    {
        MainMenuBarWindow.OpenHeroWin();
    }

    private static void AlertItemHandler(GameObject sender = null)
    {
        MainMenuBarWindow.OpenEquipWin();
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public int RewardId;
    public bool HaveSign;
    public bool CanSelect;
    public RewardTemplate RewardTemplate;
    //public bool CanTouch;

    public string Name;
    public int Count;

    //奖励的id，是否已经签到，是否可以选择
    public void SetData(int rewardid, bool havesign, bool canselect, OnSelectedCallback selectfunc = null)
    {
        RewardId = rewardid;
        HaveSign = havesign;
        CanSelect = canselect;
        OnSelected = selectfunc;

        Debug.Log("rewardid:" + rewardid);
        //PopTextManager.PopTip("rewardid:" + rewardid);

        RewardTemplate = SystemModelLocator.Instance.RewardTemplates.RewardTmpls[rewardid];

        if (RewardTemplate.RewardType == SIGN_REWARD_TYPE_HERO)
        {
            var tem = HeroModelLocator.Instance.GetHeroByTemplateId(RewardTemplate.RewardTempId);
            Name = tem.Name;
            Count = 1;
        }
        else if (RewardTemplate.RewardType == SIGN_REWARD_TYPE_ITEM)
        {
            Name = ItemModeLocator.Instance.GetName(RewardTemplate.RewardTempId);
            Count = 1;
        }
        else
        {
            Name =
                MailConstant.GetCurrencyNameById(RewardTemplate.RewardTempId);

            Count = RewardTemplate.RewardCount;
        }
        
        SetContent();
        
    }

    private void SetContent()
    {
        if (GoodsIcon == null) return;
        SelectSprite.SetActive(HaveSign);
        //ClickSprite.SetActive(CanSelect);
        if (LightEffect != null)
        {
            if (CanSelect)
            {
                LightEffect.Play();
            }
            else
            {
                LightEffect.Stop();
            }
        }
       

        if (RewardTemplate.RewardType == SIGN_REWARD_TYPE_HERO)
        {
            GoodsIcon.SetActive(false);
            HeadIcon.SetActive(true);
            ItemIcon.SetActive(false);
            HeadContainer.SetActive(true);
            CountLabel.SetActive(false);

            var sp = HeadIcon.GetComponent<UISprite>();
            var tem = HeroModelLocator.Instance.GetHeroByTemplateId(RewardTemplate.RewardTempId);
            if (tem != null)
            {
                HeroConstant.SetHeadByIndex(sp, tem.Icon - 1);

                var jobsp = JobSprite.GetComponent<UISprite>();
                jobsp.spriteName = "job_" + tem.Job;

                for (int i = 0; i < Stars.Count; i++)
                {
                    Stars[i].SetActive(i < tem.Star);
                }
            }
        }
        else if (RewardTemplate.RewardType == SIGN_REWARD_TYPE_ITEM)
        {
            GoodsIcon.SetActive(false);
            HeadIcon.SetActive(false);
            ItemIcon.SetActive(true);
            HeadContainer.SetActive(true);
            CountLabel.SetActive(false);

            var sp = ItemIcon.GetComponent<UISprite>();

            //var icon = ItemModeLocator.Instance.GetIconId(RewardTemplate.RewardTempId);
            ItemType.SetHeadByTemplate(sp, RewardTemplate.RewardTempId);
            var q = ItemModeLocator.Instance.GetQuality(RewardTemplate.RewardTempId);
            int k = ItemHelper.GetStarCount(q);
            for (int i = 0; i < Stars.Count; i++)
            {
                Stars[i].SetActive(i < k);
            }
            var jobsp = JobSprite.GetComponent<UISprite>();
            jobsp.spriteName = "job_" + ItemModeLocator.Instance.GetJob(RewardTemplate.RewardTempId);
        }
        else
        {
            GoodsIcon.SetActive(true);
            HeadIcon.SetActive(false);
            ItemIcon.SetActive(false);
            HeadContainer.SetActive(false);
            CountLabel.SetActive(OnSelected != null);

            var sp = GoodsIcon.GetComponent<UISprite>();
            sp.spriteName =
                MailConstant.GetCurrencyIconById(RewardTemplate.RewardTempId);

            var lb = CountLabel.GetComponent<UILabel>();
            lb.text = RewardTemplate.RewardCount.ToString();
        }
    }
}
