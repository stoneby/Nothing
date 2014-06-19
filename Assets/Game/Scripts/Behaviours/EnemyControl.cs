using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.hero.properties;
using System.Collections;
using UnityEngine;

/// <summary>
/// Enemy Controller that control behaviours of enemy.
/// </summary>
public class EnemyControl : MonoBehaviour
{
    #region Public Fields

    public GameObject EnemySprite;
    public GameObject CdLabel;

    public GameObject AimSprite;

    public GameObject AttackLocation;

    public BloodBarController BloodController;

    [HideInInspector]
    public FighterInfo Data;

    public delegate void OnClickDelegate(FighterInfo data);

    #endregion

    #region Private Fields

    private int cd;

    private const string BossNormal = "BossNormal";
    private const string BossWhite = "BossWhite";
    private const string BossBlack = "BossBlack";

    private UIEventListener MonsterClickUIEventListener;
    private OnClickDelegate OnClickFunc;

    #endregion

    #region Public Methods

    public void Init(OnClickDelegate onclickfunc, FighterInfo data)
    {
        SetValue(data);
        OnClickFunc = onclickfunc;
        MonsterClickUIEventListener = UIEventListener.Get(gameObject);
        MonsterClickUIEventListener.onClick += OnClickHandler;
    }

    public void OnDestory()
    {
        if (MonsterClickUIEventListener != null) MonsterClickUIEventListener.onClick -= OnClickHandler;
    }

    public void PlayBigAttrack()
    {
        StartCoroutine(DoPlayBigAttrack());
    }

    public void SetValue(FighterInfo data)
    {
        Data = data;

        BloodController.CurrentValue = Data.BattleProperty[RoleAProperty.HP]; ;
        BloodController.MaxValue = BloodController.CurrentValue;
        BloodController.ShowValue();

        SetCdLabel();
    }


    public float Health
    {
        get
        {
            return BloodController.CurrentValue;
        }
    }

    public void SetRoundCount(int thecd)
    {
        cd = thecd;
        BloodController.ShowValue();

        SetCdLabel();
    }

    public void SetHealth(int thehp)
    {
        BloodController.CurrentValue = thehp;
        BloodController.ShowValue();
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

    public void ShowBlood(bool show)
    {
        BloodController.ShowBlood(show);
    }

    #endregion

    #region Private Methods

    private void SetCdLabel()
    {
        var cdLabel = CdLabel.GetComponent<UILabel>();
        cdLabel.color = cd == 1 ? new Color(255, 0, 0) : new Color(0, 255, 0);
        cdLabel.text = "CD:" + cd;
    }

    private IEnumerator DoPlayAttarck()
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

    private void OnClickHandler(GameObject game = null)
    {
        OnClickFunc(Data);
    }

    private IEnumerator DoPlayBigAttrack()
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

    #endregion
}
