using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.hero.properties;
using System;
using System.Collections;
using Template.Auto.Hero;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public UISprite FootSprite;
    public UISprite JobSprite;
    public UILabel BaseAttackLabel;
    public UILabel AttackTimesLabel;
    public UILabel FriendLabel;
    public UILabel AttackLabel;

    public BuffBarController BuffBarController;
    public Character CharacterData;

    public int JobIndex;
    public int Attack;
    public int Restore;

    public bool IsActive;
    public int AttackValue;
    public bool HaveSp;

    public int AnimationIndex;

    public int FootIndex
    {
        get { return CharacterData.ColorIndex; }
    }

    private HeroTemplate templateData;

    private float afterTime;

    private const string MultiplierFix = "x";

    public void SetFootIndex()
    {
        if (FootIndex <= 0 || FootIndex > Character.TotalColorCount)
        {
            Logger.LogError("Foot index error, should be in range (0, " + Character.TotalColorCount + "], but is: " + FootIndex);
        }

        FootSprite.spriteName = "ground_" + FootIndex;
        JobSprite.spriteName = (FootIndex == (int)FootColorType.Pink) ? "job_0" : "job_" + JobIndex;
    }

    public void SetCanSelect(bool flag)
    {
        var uisp = FootSprite.GetComponent<UISprite>();
        uisp.alpha = flag ? 1 : 0.3f;
    }

    public void SetCharacterAfter(float aftertime)
    {
        afterTime = aftertime;
        StartCoroutine(DoSetCharacterAfter());
    }

    IEnumerator DoSetCharacterAfter()
    {
        yield return new WaitForSeconds(afterTime);
        PlayCharacter(Character.State.Idle, true);
    }

    public void SetCharacter(CharacterType characterType = CharacterType.Hero)
    {
        var data = CharacterData.Data;
        var tempid = Int32.Parse(data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
        templateData = HeroModelLocator.Instance.GetHeroByTemplateId(tempid);

        JobIndex = templateData.Job;
        var uisp = JobSprite.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == (int)FootColorType.Pink) ? "job_0" : "job_" + JobIndex;

        var isFriend = (characterType != CharacterType.Hero);
        FriendLabel.gameObject.SetActive(isFriend);
        if (isFriend)
        {
            FriendLabel.text = characterType.ToString();
        }
    }

    /// <summary>
    /// Adjust attack value in case any zero effect buff occurs.
    /// </summary>
    public void AdjustAttackValue()
    {
        Attack = (CharacterData.BuffController.ZeroAttack) ? 0 : Attack;
        Restore = (CharacterData.BuffController.ZeroAttack) ? 0 : Restore;
    }

    public void SetAttackLabel(SingleFighterRecord record)
    {
        Attack = record.getIntProp(RoleAProperty.ATK);
        Restore = record.getIntProp(RoleAProperty.RECOVER);
        BaseAttackLabel.text = "" + ((FootIndex == (int)FootColorType.Pink) ? (Restore) : (Attack));
    }

    public void PlayCharacter(Character.State state, bool loop)
    {
        CharacterData.PlayState(state, loop);
        var enableGroup = (state != Character.State.Run);
        NGUITools.SetActive(FootSprite.gameObject, enableGroup);
    }

    public void ShowAttack(bool show, int selectindex = -1)
    {
        if (show)
        {
            AttackTimesLabel.text = (selectindex == 0) ? "" : (MultiplierFix + BattleTypeConstant.MoreHitTimes[selectindex]);
            AttackValue = (FootIndex == (int)FootColorType.Pink) ? (int)(BattleTypeConstant.MoreHitTimes[selectindex] * Restore) : (int)(BattleTypeConstant.MoreHitTimes[selectindex] * Attack);
            AttackLabel.text = "" + AttackValue;
        }
        else
        {
            AttackTimesLabel.text = string.Empty;
            AttackLabel.text = string.Empty;
        }
    }
}
