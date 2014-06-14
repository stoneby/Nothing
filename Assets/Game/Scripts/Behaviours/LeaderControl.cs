using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.input;
using Template;
using UnityEngine;

public class LeaderControl : MonoBehaviour
{
    public GameObject SpriteBg;
    public GameObject SpriteLight;
    public GameObject SpriteHead;

    public int LeaderIndex;

    public int HeadIndex;//0标识无效

    private GameObject shineObj;
    private int baseCd;//需要气力值
    private int currentCd;
    private int cd;

    private FighterInfo Data;
    private SkillTemplate SkillData;

    private UIEventListener HeadUIEventListener;

    void Start()
    {
		HeadUIEventListener = UIEventListener.Get(SpriteHead);
		if (HeadUIEventListener != null) HeadUIEventListener.onClick += OnHeadClick;
        
    }

    public void Init(int headindex, int basecd, int theindex = 0)
    {
        SpriteBg = transform.FindChild("Sprite - bg").gameObject;
        SpriteHead = transform.FindChild("Sprite - head").gameObject;
        SpriteLight = transform.FindChild("Sprite - light").gameObject;

        

        SpriteLight.SetActive(false);
        HeadIndex = headindex;
        baseCd = basecd;
        LeaderIndex = theindex;
        var sp = SpriteHead.GetComponent<UISprite>();
        sp.spriteName = "head_" + HeadIndex;
    }

    public void SetData(FighterInfo data, int theindex)
    {
        LeaderIndex = theindex;
        Data = data;
        SkillData = HeroModelLocator.Instance.GetLeaderSkillTemplateById(Data.ActiveSkillId);
        var sp = SpriteHead.GetComponent<UISprite>();
        if (SkillData == null)
        {
            sp.spriteName = "head_0";
        }
        else
        {
            sp.spriteName = "head_" + HeadIndex;
            baseCd = SkillData.CostMp;
        }
    }

    public void Reset(int currentcd)
    {
        currentCd = currentcd;
        if (HeadIndex > 0 && SkillData != null)
        {
            if (currentcd >= baseCd)
            {
                SpriteLight.SetActive(true);
            }
            else
            {
                SpriteLight.SetActive(false);
            }
        }
        else
        {
            SpriteLight.SetActive(false);
        }
    }

    private bool AlertFlag = true;
    private void OnHeadClick(GameObject game)
    {
        if (HeadIndex > 0 && currentCd >= baseCd && SkillData != null)
        {
            AlertFlag = false;
            Alert.Show(AssertionWindow.Type.OkCancel, SkillData.Name, SkillData.Desc, OnAssertButtonClicked, OnCancelClicked);
            //PopTextManager.PopTip(SkillData.Name + ":" + SkillData.Desc);

            BattleModelLocator.Instance.CanSelectHero = false;

        }
    }

    private void OnAssertButtonClicked(GameObject sender)
    {
        if (AlertFlag) return;
        AlertFlag = true;
        var _action = new UseActiveSkillAction();
        _action.FighterIndex = LeaderIndex;
        BattleModelLocator.Instance.MainBattle.handleBattleEvent(_action);
        BattleModelLocator.Instance.Skill = SkillData;
        var e = new LeaderUseEvent();
        e.CDCount = baseCd;
        e.SkillIndex = LeaderIndex;
        EventManager.Instance.Post(e);
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    private void OnCancelClicked(GameObject sender)
    {
        BattleModelLocator.Instance.CanSelectHero = true;
    }
}
