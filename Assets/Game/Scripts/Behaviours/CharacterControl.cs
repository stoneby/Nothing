using com.kx.sglm.gs.battle.share;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject BaseObj;
    public GameObject AnimObj;
    public GameObject FootObj;
    public GameObject JobObj;
    public GameObject AttrackObj;
    public GameObject TopTimesObj;
    public GameObject SpritePrefab;
    public GameObject SpriteObj;
    public GameObject PoisonPrefab;
    public GameObject BuffObj;
    public GameObject FriendLabelObj;

    /// <summary>
    /// Animator if any.
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// Animation if any.
    /// </summary>
    public Animation Animation;

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

    private GameObject topAttrackObj;
    private bool isSelected;

    private float afterTime;

    private List<string> animationList; 

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

        // character index should be range in [0, 5].
        // [NOTE] We only have 2 types of characters for now.
        CharacterIndex = (tempid % 2 == 0) ? 0 : 1;

        JobIndex = TemplateData.Job;
        Attrack = Data.BattleProperty.get(FighterAProperty.ATK);
        Restore = Data.BattleProperty.get(FighterAProperty.RECOVER);

        var uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == BattleTypeConstant.FootPink) ? "job_6" : "job_" + JobIndex;
        var uilb = AttrackObj.GetComponent<UILabel>();
        uilb.text = (FootIndex == BattleTypeConstant.FootPink) ? Restore.ToString() : Attrack.ToString();

        Logger.LogWarning("Character inex ; " + CharacterIndex + ", sprite name: " + uisp.spriteName + ", attack: " + uilb.text);

        if (isfriend == BattleTypeConstant.IsHero)
        {
            FriendLabelObj.SetActive(false);
        }
        else 
        {
            FriendLabelObj.SetActive(true);
            uilb = FriendLabelObj.GetComponent<UILabel>();
            uilb.text = isfriend == BattleTypeConstant.IsFriend ? "Friend" : "Guest";
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

    /// <summary>
    /// Play character state.
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="loop">Flag indicates if state is in loop mode</param>
    public void PlayState(Character.State state, bool loop)
    {
        Animation[animationList[(int)state]].wrapMode = (loop) ? WrapMode.Loop : WrapMode.Once;
        Animation.Play(animationList[(int)state]);
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
            if (topAttrackObj != null) Destroy(topAttrackObj);
        }
    }

    private IEnumerator PopPlay()
    {
        PopTextManager.ShowText(AttrackValue.ToString(), 0.5f, -25, 0, 70, transform.localPosition);
        yield return new WaitForSeconds(0.5f);
        if (isSelected)
        {
            topAttrackObj = PopTextManager.ShakeText(AttrackValue.ToString(), -25, 25, transform.localPosition);
        }
    }

    #region Private Methods

    private T GetAnimationStuff<T>() where T : Component
    {
        var animations = transform.GetComponentsInChildren<T>();
        return animations.Any() ? animations.First() : null;
    }

    #endregion

    #region Mono

    protected void Awake()
    {
        Animation = GetAnimationStuff<Animation>();
        Animator = GetAnimationStuff<Animator>();

        if (Animation != null)
        {
            animationList = new List<string>(Animation.Cast<AnimationState>().Select(item => item.name));
        }
    }

    #endregion
}
