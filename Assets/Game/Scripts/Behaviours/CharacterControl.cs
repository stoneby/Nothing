using System;
using System.Collections;
using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.utils;
using KXSGCodec;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject BaseObj;
    public GameObject AnimObj;
    public GameObject FootObj;
    public GameObject JobObj;
    public GameObject AttrackObj;
    public GameObject TopTimesObj;
    private GameObject TopAttrackObj;
    public GameObject SpritePrefab;
    public GameObject SpriteObj;
    public GameObject PoisonPrefab;
    public GameObject BuffObj;
    public GameObject FriendLabelObj;

    public FighterInfo Data;
    private HeroTemplate TemplateData;

    public int CharacterIndex;
    public int FootIndex;
    public int JobIndex;
    public int Attrack;	//攻击力，
    public int Restore; //回复力

    public int XIndex;
    public int YIndex;

    public bool IsActive;
    public int AttrackValue;
    public bool HaveSp;

    public int AnimationIndex;

    private bool isSelected = false;

    private float afterTime;

    public void SetIndex(int xindex, int yindex)
    {
        XIndex = xindex;
        YIndex = yindex;
    }

    public void ShowSpEffect(bool isshow)
    {
        HaveSp = isshow;
        if (isshow)
        {
            if (SpriteObj == null)
            {
                SpriteObj = NGUITools.AddChild(BaseObj, SpritePrefab);
                SpriteObj.transform.localPosition = new Vector3(-25, 0, 0);
                var sp = SpriteObj.GetComponent<UISprite>();
                sp.alpha = 0.8f;
                sp.depth = 7;
            }
        }
        else
        {
            if (SpriteObj != null)
            {
                Destroy(SpriteObj);
                SpriteObj = null;
            }
        }
    }

    public void SetFootIndex(int footindex)
    {
        Logger.Log("Foot ===== " + footindex);
        FootIndex = footindex;
        var uisp = FootObj.GetComponent<UISprite>();
        uisp.spriteName = "pck_" + footindex;
        uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == BattleTypeConstant.FootPink) ? "job_6" : "job_" + JobIndex;
        var uilb = AttrackObj.GetComponent<UILabel>();
        uilb.text = (FootIndex == BattleTypeConstant.FootPink) ? ("" + Restore) : ("" + Attrack);
        //uilb.text += "-" + footindex.ToString();
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
        PlayCharacter(0);
    }

    public void SetCharacter(FighterInfo data, int isfriend = BattleTypeConstant.IsHero)
    {
        Data = data;
        var tempid = Int32.Parse(Data.getProp(BattleKeyConstants.BATTLE_KEY_HERO_TEMPLATE));
        Logger.Log(tempid);
        TemplateData = HeroModelLocator.Instance.GetHeroByTemplateId(tempid);

        CharacterIndex = (tempid % 2 == 0) ? 1 : 5;
        JobIndex = TemplateData.Job;
        Attrack = Data.BattleProperty.get(FighterAProperty.ATK);
        Restore = Data.BattleProperty.get(FighterAProperty.RECOVER);

        var uisa = AnimObj.GetComponent<UISpriteAnimation>();
        uisa.namePrefix = "c_" + CharacterIndex + "_0_";
        uisa.framesPerSecond = 8;
        var uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == BattleTypeConstant.FootPink) ? "job_6" : "job_" + JobIndex;
        var uilb = AttrackObj.GetComponent<UILabel>();
        uilb.text = (FootIndex == BattleTypeConstant.FootPink) ? Restore.ToString() : Attrack.ToString();

        if (isfriend == BattleTypeConstant.IsHero)
        {
            FriendLabelObj.SetActive(false);
        }
        else 
        {
            FriendLabelObj.SetActive(true);
            uilb = FriendLabelObj.GetComponent<UILabel>();
            if (isfriend == BattleTypeConstant.IsFriend)
            {
                uilb.text = "Friend";
            }
            else
            {
                uilb.text = "Guest";
            }
        }
    }

    public string GetNamePrefix()
    {
        return "c_" + CharacterIndex + "_";
    }

    public void ResetCharacter()
    {
        if (AnimObj != null)
        {
            AnimObj.SetActive(false);
            AnimObj.SetActive(true);
        }
        if (SpriteObj != null)
        {
            SpriteObj.SetActive(false);
            SpriteObj.SetActive(true);
        }
    }

    public void PlayCharacter(int animindex)
    {
        AnimationIndex = animindex;
        var uisa = AnimObj.GetComponent<UISpriteAnimation>();
        uisa.namePrefix = "c_" + CharacterIndex + "_" + animindex + "_";
        if (animindex == 2 || animindex == 3)
        {
            NGUITools.SetActive(FootObj, false);
        }
        else
        {
            NGUITools.SetActive(FootObj, true);
        }

    }

    public void Stop()
    {
        var uisa = AnimObj.GetComponent<UISpriteAnimation>();
        uisa.loop = false;
    }

    public void Play()
    {
        var uisa = AnimObj.GetComponent<UISpriteAnimation>();
        uisa.loop = true;
        uisa.Reset();
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
            uilb.text = (selectindex == 0) ? "" : "X" + BattleTypeConstant.MoreHitTimes[selectindex].ToString();
            AttrackValue = (FootIndex == BattleTypeConstant.FootPink) ? (int)(BattleTypeConstant.MoreHitTimes[selectindex] * Restore) : (int)(BattleTypeConstant.MoreHitTimes[selectindex] * Attrack);
            StartCoroutine(PopPlay());
        }
        else
        {
            uilb.text = "";
            if (TopAttrackObj != null) Destroy(TopAttrackObj);
        }
    }

    private IEnumerator PopPlay()
    {
        PopTextManager.ShowText(AttrackValue.ToString(), 0.5f, -25, 0, 70, transform.localPosition);
        yield return new WaitForSeconds(0.5f);
        if (isSelected)TopAttrackObj = PopTextManager.ShakeText(AttrackValue.ToString(), -25, 25, transform.localPosition);
    }
}
