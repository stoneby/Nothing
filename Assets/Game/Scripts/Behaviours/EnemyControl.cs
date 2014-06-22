using System.Collections;
using UnityEngine;
using com.kx.sglm.gs.battle.share.data;

/// <summary>
/// Enemy Controller that control behaviours of enemy.
/// </summary>
public class EnemyControl : MonoBehaviour
{
    #region Public Fields

    public GameObject EnemySprite;
    public GameObject CdLabel;
    public GameObject AimSprite;

	/// <summary>
	/// Be attacked location.
	/// </summary>
    public GameObject AttackLocation;

	/// <summary>
	/// Blood bar controller.
	/// </summary>
    public BloodBarController BloodController;

    [HideInInspector]
    public FighterInfo Data;

    public delegate void OnClickDelegate(FighterInfo data);

    /// <summary>
    /// On click callback function.
    /// </summary>
    [HideInInspector]
    public OnClickDelegate OnSelected;

	/// <summary>
	/// Enemy's health.
	/// </summary>
    public float Health
    {
        get
        {
            return BloodController.CurrentValue;
        }
    }

    #endregion

    #region Private Fields

    private const int WarningCdValue = 1;
    private const string CdHead = "CD:";

    private const string BossNormal = "BossNormal";
    private const string BossWhite = "BossWhite";
    private const string BossBlack = "BossBlack";

    private Vector3 originalPosition;

    #endregion

    #region Public Methods

    /// <summary>
    /// Set data.
    /// </summary>
    /// <param name="current">Current value</param>
    /// <param name="max">Max value</param>
	public void SetBloodBar(float current, float max)
	{
		BloodController.Set(current, max);
		BloodController.UpdateUI();
	}

    /// <summary>
    /// Set health of this enemy.
    /// </summary>
    /// <param name="health">Health.</param>
    public void SetHealth(int health)
    {
        BloodController.CurrentValue = health;
        BloodController.UpdateUI();
    }

    /// <summary>
    /// Set cd label.
    /// </summary>
    /// <param name="cd">Cd value</param>
    public void SetCdLabel(int cd)
    {
        var cdLabel = CdLabel.GetComponent<UILabel>();
        cdLabel.color = (cd == WarningCdValue) ? Color.red : Color.green;
        cdLabel.text = string.Format("{0} {1}", CdHead, cd);
    }

    /// <summary>
    /// Show aim to sprite.
    /// </summary>
    /// <param name="flag">If set to <c>true</c> flag.</param>
    public void ShowAimTo(bool flag)
    {
        AimSprite.SetActive(flag);
    }
   
    /// <summary>
    /// Play shake.
    /// </summary>
    public void PlayShake()
    {
        iTweenEvent.GetEvent(EnemySprite, "ShakeTween").Play();
    }

    /// <summary>
    /// Play big shake.
    /// </summary>
    public void PlayBigShake()
    {
        iTweenEvent.GetEvent(EnemySprite, "ShakeBigTween").Play();
    }

    /// <summary>
    /// Play attack animation.
    /// </summary>
    /// <returns>The attrack.</returns>
    public float PlayAttrack()
    {
        StartCoroutine(DoPlayAttarck());
        return GameConfig.MonsterAttrackStepTime * 3;
    }

    /// <summary>
    /// Show blood bar.
    /// </summary>
    /// <param name="show">If set to <c>true</c> show.</param>
    public void ShowBlood(bool show)
    {
        BloodController.Show(show);
    }

    public void Reset()
    {
        EnemySprite.transform.localPosition = originalPosition;
    }

    #endregion

    #region Private Methods

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

    #endregion

    #region Mono

    private void Awake()
    {
        // default hide aim sprite.
        AimSprite.SetActive(false);
        // record original position down to restore.
        originalPosition = EnemySprite.transform.localPosition;
    }

    #endregion
}
