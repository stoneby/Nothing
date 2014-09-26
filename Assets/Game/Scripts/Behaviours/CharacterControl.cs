using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data.record;
using com.kx.sglm.gs.hero.properties;
using System;
using System.Collections;
using Template.Auto.Hero;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject FootObj;
    public GameObject JobObj;
    public GameObject AttackObj;
    public GameObject TopTimesObj;
    public GameObject FriendLabelObj;

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
    private GameObject topAttackObj;
    private bool isSelected;

    private float afterTime;

    public void SetFootIndex()
    {
        if (FootIndex <= 0 || FootIndex > Character.TotalColorCount)
        {
            Logger.LogError("Foot index error, should be in range (0, " + Character.TotalColorCount + "], but is: " + FootIndex);
        }

        var uisp = FootObj.GetComponent<UISprite>();
        uisp.spriteName = "ground_" + FootIndex;
        uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == (int)FootColorType.Pink) ? "job_0" : "job_" + JobIndex;
    }

    public void SetCanSelect(bool flag)
    {
        if (FootObj == null) return;
        var uisp = FootObj.GetComponent<UISprite>();
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
        var uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == (int)FootColorType.Pink) ? "job_0" : "job_" + JobIndex;

        var isFriend = (characterType != CharacterType.Hero);
        FriendLabelObj.SetActive(isFriend);
        if (isFriend)
        {
            var uilb = FriendLabelObj.GetComponent<UILabel>();
            uilb.text = characterType.ToString();
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
        var uilb = AttackObj.GetComponent<UILabel>();
        Attack = record.getIntProp(RoleAProperty.ATK);
        Restore = record.getIntProp(RoleAProperty.RECOVER);
        uilb.text = "" + ((FootIndex == (int)FootColorType.Pink) ? (Restore) : (Attack));
    }

    public void PlayCharacter(Character.State state, bool loop)
    {
        CharacterData.PlayState(state, loop);
        if (state == Character.State.Run || state == Character.State.Run)
        {
            NGUITools.SetActive(FootObj, false);
        }
        else
        {
            NGUITools.SetActive(FootObj, true);
        }
    }

    public void SetSelect(bool isselected, int selectindex = -1)
    {
        isSelected = isselected;
        var uilb = TopTimesObj.GetComponent<UILabel>();
        if (isselected)
        {

            if (selectindex > 6)
            {
                uilb.color = new Color(255, 0, 248);
            }
            else if (selectindex > 3)
            {
                uilb.color = new Color(0, 255, 2);
            }
            else
            {
                uilb.color = new Color(234, 240, 240);
            }
            uilb.text = (selectindex == 0) ? "" : "X" + BattleTypeConstant.MoreHitTimes[selectindex];
            AttackValue = (FootIndex == (int)FootColorType.Pink) ? (int)(BattleTypeConstant.MoreHitTimes[selectindex] * Restore) : (int)(BattleTypeConstant.MoreHitTimes[selectindex] * Attack);
            StartCoroutine(PopPlay());
        }
        else
        {
            uilb.text = "";
            if (topAttackObj != null) Destroy(topAttackObj);
        }
    }

    private IEnumerator PopPlay()
    {
        PopTextManager.ShowText(AttackValue.ToString(), 0.5f, 0, 0, 70, CharacterData.transform.localPosition);
        yield return new WaitForSeconds(0.5f);
        if (isSelected)
        {
            topAttackObj = PopTextManager.ShakeText(AttackValue.ToString(), -25, 25, CharacterData.transform.localPosition);
        }
    }
}
