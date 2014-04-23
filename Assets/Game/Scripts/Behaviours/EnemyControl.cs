using System.Collections;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public GameObject EnemySprite;
    public GameObject BloodBar;
    public GameObject BloodLabel;
    public GameObject CdLabel;
    //public GameObject Enemy;

    private float value;
    private float maxValue;
    private int baseCd;
    private int cd;
    private int spcd;

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

    public void SetValue(float thevalue, float themaxvalue, int thecd)
    {
        value = thevalue;
        maxValue = themaxvalue;
        cd = baseCd = thecd;
        spcd = 2 * baseCd;
        ShowValue();
    }

    public bool CanSpAttrack()
    {
        spcd--;
        if (spcd > 0)
        {
            return false;
        }
        spcd = 2 * baseCd;
        cd = baseCd;
        ShowValue();
        return true;
    }

    public bool CanAttrack()
    {
        cd--;
        if (cd > 0)
        {
            ShowValue();
            return false;
        }
        cd = baseCd;
        ShowValue();
        return true;
    }

    //处理掉血，返回是否死亡
    public bool LoseBlood(int lose)
    {

        value = (value - lose > 0) ? value - lose : 0;

        if (lose != 0) PopTextManager.ShowText("-" + lose, 0.6f, 0, 40, 120, gameObject.transform.localPosition);

        ShowValue();
        return value <= 0;
    }

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
        return 0.9f;
    }

    IEnumerator DoPlayAttarck()
    {
        //var sp = EnemySprite.GetComponent<UISprite>();
        var sp = EnemySprite.GetComponent<AdvanceImageTuner>(); 
        yield return new WaitForSeconds(0.3f);
        sp.Highlight = 0.89f;
        sp.Color = Color.white;
        //sp.spriteName = BossWhite;
        yield return new WaitForSeconds(0.3f);
        sp.Highlight = 0f;
        sp.Color = Color.black;
        //sp.spriteName = BossBlack;
        yield return new WaitForSeconds(0.3f);
        sp.Highlight = 0.89f;
        sp.Color = Color.white;
        //sp.spriteName = BossWhite;
        yield return new WaitForSeconds(0.3f);
        sp.Highlight = 0f;
        sp.Color = Color.white;
        //sp.spriteName = BossNormal;
    }
}
