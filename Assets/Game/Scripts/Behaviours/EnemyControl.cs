using System.Collections;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.utils;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public GameObject EnemySprite;
    public GameObject BloodBar;
    public GameObject BloodLabel;
    public GameObject CdLabel;
    //public GameObject Enemy;

    public FighterInfo Data;

    private float value;
    private float maxValue;
//    private int baseCd;
    private int cd;
//    private int spcd;

    //Boss sprite name of different states.
    private const string BossNormal = "BossNormal";
    private const string BossWhite = "BossWhite";
    private const string BossBlack = "BossBlack";


    public void playBigAttrack()
    {
        StartCoroutine(DoPlayBigAttrack());
    }

    IEnumerator DoPlayBigAttrack()
    {
        yield return new WaitForSeconds(0.4f);
        var tc = EnemySprite.AddComponent<TweenColor>();
        tc.from = new Color(255, 0, 0);
        tc.to = new Color(0, 255, 0);
        tc.duration = 0.2f;
        tc.style = UITweener.Style.PingPong;
        tc.PlayForward();
        Destroy(tc, 2);
    }

    public void SetValue(FighterInfo data)
    {
        Data = data;
        value = Data.BattleProperty.get(FighterAProperty.HP); ;
        maxValue = value;
//        cd = baseCd = 3;//Data.BattleProperty.get(FighterAProperty.);
//        spcd = 2 * baseCd;
        ShowValue();
    }

    public  float HP
    {
        get
        {
            return value;
        }
    }

    public void SetRoundCount(int thecd)
    {
        cd = thecd;
        ShowValue();
    }

    public void SetHP(int thehp)
    {
        var lose = value - thehp;
        value = thehp;

        if (lose > 0) PopTextManager.ShowText("-" + lose, 0.6f, 0, 40, 120, gameObject.transform.localPosition);

        ShowValue();
    }

    //处理掉血，返回是否死亡
//    public bool LoseBlood(int lose)
//    {
//
//        value = (value - lose > 0) ? value - lose : 0;
//
//        if (lose != 0) PopTextManager.ShowText("-" + lose, 0.6f, 0, 40, 120, gameObject.transform.localPosition);
//
//        ShowValue();
//        return value <= 0;
//    }

    void ShowValue()
    {
        var sd = BloodBar.GetComponent<UISlider>();
        sd.value = value / maxValue;
        var lb = BloodLabel.GetComponent<UILabel>();
        lb.text = value + "/" + maxValue;

        lb = CdLabel.GetComponent<UILabel>();
        lb.color = cd == 1 ? new Color(255, 0, 0) : new Color(0, 255, 0);
        lb.text = "CD:" + cd;
    }

    public void PlayBeen()
    {
        iTweenEvent.GetEvent(EnemySprite, "ShakeTween").Play();
    }

    public void PlayBigBeen()
    {
        iTweenEvent.GetEvent(EnemySprite, "ShakeBigTween").Play();
    }

    public float PlayAttrack()
    {
        StartCoroutine(DoPlayAttarck());
        return GameConfig.MonsterAttrackStepTime * 3;
    }

    IEnumerator DoPlayAttarck()
    {
        var sp = EnemySprite.GetComponent<UISprite>();
        yield return new WaitForSeconds(GameConfig.MonsterAttrackStepTime);
        sp.spriteName = BossWhite;
        yield return new WaitForSeconds(GameConfig.MonsterAttrackStepTime);
        sp.spriteName = BossBlack;
        yield return new WaitForSeconds(GameConfig.MonsterAttrackStepTime);
        sp.spriteName = BossWhite;
        yield return new WaitForSeconds(GameConfig.MonsterAttrackStepTime);
        sp.spriteName = BossNormal;
    }
}
