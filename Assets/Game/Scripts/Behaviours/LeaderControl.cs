using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.input;
using Template.Auto.Skill;
using UnityEngine;

public class LeaderControl : MonoBehaviour
{
    public GameObject SpriteBg;
    public GameObject SpriteLight;
    public UISprite SpriteHead;

    public LeaderData Data;

    private GameObject shineObj;
    private int currentCD;

    private bool alertFlag = true;
    private FighterInfo figherData;
    private HeroBattleSkillTemplate skillData;
    private string defaultHeadSpriteName;
    private const string InvalidHeadSpriteName = "head_0";

    /// <summary>
    /// Reset status.
    /// </summary>
    public void Reset()
    {
        if (string.IsNullOrEmpty(defaultHeadSpriteName))
        {
            defaultHeadSpriteName = SpriteHead.spriteName;
        }

        SpriteLight.SetActive(false);
        SpriteHead.spriteName = defaultHeadSpriteName;
    }

    /// <summary>
    /// Set inner data before using.
    /// </summary>
    /// <param name="data">Fighter infor.</param>
    /// <param name="leaderIndex">Leader index.</param>
    public void SetData(FighterInfo data, int leaderIndex)
    {
        Data.LeaderIndex = leaderIndex;
        figherData = data;
        skillData = HeroModelLocator.Instance.GetLeaderSkillTemplateById(figherData.ActiveSkillId);
        if (skillData == null)
        {
            SpriteHead.spriteName = InvalidHeadSpriteName;
        }
        else
        {
            SpriteHead.spriteName = defaultHeadSpriteName;
            Data.BaseCd = skillData.CostMP;
            Debug.LogError("Leader base CD: " + Data.BaseCd + ", name: " + name);
        }
    }

    /// <summary>
    /// Set current cd.
    /// </summary>
    /// <param name="currentcd">Current cd</param>
    public void SetCD(int currentcd)
    {
        currentCD = currentcd;
        SpriteLight.SetActive((skillData != null && currentcd >= Data.BaseCd));
    }

    /// <summary>
    /// Callback on head got clicked.
    /// </summary>
    public void OnHeadClick()
    {
        if (currentCD < Data.BaseCd || skillData == null)
        {
            return;
        }

        alertFlag = false;
        Alert.Show(AssertionWindow.Type.OkCancel, skillData.BaseTmpl.Name, skillData.BaseTmpl.Desc, OnAssertButtonClicked, OnCancelClicked);
        BattleModelLocator.Instance.CanSelectHero = false;
    }

    /// <summary>
    /// Callback on assert ok button clicked.
    /// </summary>
    /// <param name="sender">Sender</param>
    private void OnAssertButtonClicked(GameObject sender)
    {
        if (alertFlag)
        {
            return;
        }

        alertFlag = true;
        var action = new UseActiveSkillAction {FighterIndex = Data.LeaderIndex};
        BattleModelLocator.Instance.MainBattle.handleBattleEvent(action);
        BattleModelLocator.Instance.Skill = skillData;
        var e = new LeaderUseEvent
        {
            CDCount = Data.BaseCd, SkillIndex = Data.LeaderIndex
        };
        EventManager.Instance.Post(e);
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    /// <summary>
    /// Callback on assert cancel button clicked.
    /// </summary>
    /// <param name="sender"></param>
    private void OnCancelClicked(GameObject sender)
    {
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    private void Awake()
    {
        defaultHeadSpriteName = SpriteHead.spriteName;
    }
}
