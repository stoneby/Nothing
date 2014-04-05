using System.Collections;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject BaseObj;
    public GameObject AnimObj;
    public GameObject FootObj;
    public GameObject JobObj;
    public GameObject AttrackObj;
    public GameObject TopTimesObj;
    public GameObject TopAttrackObj;
    public GameObject SpritePrefab;
    public GameObject SpriteObj;
    public GameObject PoisonPrefab;
    public GameObject BuffObj;

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
        FootIndex = footindex;
        var uisp = FootObj.GetComponent<UISprite>();
        uisp.spriteName = "foot_" + footindex;
        uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == 4) ? "icon_zhiye_1" : "icon_zhiye_" + JobIndex;
        var uilb = AttrackObj.GetComponent<UILabel>();
        uilb.text = (FootIndex == 4) ? ("" + Restore) : ("" + Attrack);
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

    public void SetCharacter(int characterindex, int zhiyeindex, int theattrack, int theback)
    {
        CharacterIndex = characterindex;
        JobIndex = zhiyeindex;
        Attrack = theattrack;
        Restore = theback;

        var uisa = AnimObj.GetComponent<UISpriteAnimation>();
        uisa.namePrefix = "c_" + CharacterIndex + "_0_";
        uisa.framesPerSecond = 8;
        var uisp = JobObj.GetComponent<UISprite>();
        uisp.spriteName = (FootIndex == 4) ? "icon_zhiye_1" : "icon_zhiye_" + JobIndex;
        var uilb = AttrackObj.GetComponent<UILabel>();
        uilb.text = (FootIndex == 4) ? Restore.ToString() : Attrack.ToString();
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
        UILabel uilb;
        if (isselected)
        {
            uilb = TopTimesObj.GetComponent<UILabel>();
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
            uilb.text = (selectindex == 0) ? "" : "X" + BattleType.MoreHitTimes[selectindex].ToString();
            uilb = TopAttrackObj.GetComponent<UILabel>();
            AttrackValue = (FootIndex == 4) ? (int)(BattleType.MoreHitTimes[selectindex] * Restore) : (int)(BattleType.MoreHitTimes[selectindex] * Attrack);
            uilb.text = AttrackValue.ToString();
            iTweenEvent.GetEvent(TopAttrackObj, "ShakeAttrackLabel").Play();
        }
        else
        {
            uilb = TopTimesObj.GetComponent<UILabel>();
            uilb.text = "";
            uilb = TopAttrackObj.GetComponent<UILabel>();
            uilb.text = "";
            iTweenEvent.GetEvent(TopAttrackObj, "ShakeAttrackLabel").Stop();
        }
    }
}
