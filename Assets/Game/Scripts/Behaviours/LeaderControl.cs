using Assets.Game.Scripts.Common.Model;
using Template.Auto.Skill;
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

    public int HeadIndex;

    private GameObject shineObj;
    private int baseCd;
    private int currentCd;
    private int cd;

    private bool alertFlag = true;
    private FighterInfo figherData;
    private HeroBattleSkillTemplate skillData;

    private UIEventListener headUIEventListener;

    void Start()
    {
        headUIEventListener = UIEventListener.Get(SpriteHead);
        headUIEventListener.onClick += OnHeadClick;
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
        figherData = data;
        skillData = HeroModelLocator.Instance.GetLeaderSkillTemplateById(figherData.ActiveSkillId);
        var sp = SpriteHead.GetComponent<UISprite>();
        if (skillData == null)
        {
            sp.spriteName = "head_0";
        }
        else
        {
            sp.spriteName = "head_" + HeadIndex;
            baseCd = skillData.CostMP;
        }
    }

    public void Reset(int currentcd)
    {
        currentCd = currentcd;
        if (HeadIndex > 0 && skillData != null)
        {
            SpriteLight.SetActive(currentcd >= baseCd);
        }
        else
        {
            SpriteLight.SetActive(false);
        }
    }

    private void OnHeadClick(GameObject game)
    {
        if (HeadIndex > 0 && currentCd >= baseCd && skillData != null)
        {
            alertFlag = false;
            Alert.Show(AssertionWindow.Type.OkCancel, skillData.BaseTmpl.Name, skillData.BaseTmpl.Desc, OnAssertButtonClicked, OnCancelClicked);
            BattleModelLocator.Instance.CanSelectHero = false;
        }
    }

    private void OnAssertButtonClicked(GameObject sender)
    {
        if (alertFlag) return;
        alertFlag = true;
        var action = new UseActiveSkillAction {FighterIndex = LeaderIndex};
        BattleModelLocator.Instance.MainBattle.handleBattleEvent(action);
        BattleModelLocator.Instance.Skill = skillData;
        var e = new LeaderUseEvent {CDCount = baseCd, SkillIndex = LeaderIndex};
        EventManager.Instance.Post(e);
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    private void OnCancelClicked(GameObject sender)
    {
        BattleModelLocator.Instance.CanSelectHero = true;
    }
}
