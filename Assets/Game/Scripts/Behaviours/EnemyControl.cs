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

    public BuffBarController BuffController;

    /// <summary>
    /// Base widget of moving standard.
    /// </summary>
    public UIWidget BaseWidget;

    /// <summary>
    /// Speed of moving to next round.
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// Duration of moving to next round.
    /// </summary>
    public float MoveDuration;

    /// <summary>
    /// High light value.
    /// </summary>
    public float HighLight;

    /// <summary>
    /// Character data.
    /// </summary>
    public Character CharacterData;

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
    /// Set blood bar.
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
        StopAll();
        ResetPosition();
        iTweenEvent.GetEvent(EnemySprite, "ShakeTween").Play();
    }

    /// <summary>
    /// Play big shake.
    /// </summary>
    public void PlayBigShake()
    {
        StopAll();
        ResetPosition();
        iTweenEvent.GetEvent(EnemySprite, "ShakeBigTween").Play();
    }

    /// <summary>
    /// Stop shake and big shake.
    /// </summary>
    public void StopAll()
    {
        iTween.Stop(gameObject);
    }

    /// <summary>
    /// Play attack animation.
    /// </summary>
    /// <returns>The attrack.</returns>
    public float PlayAttack()
    {
        StartCoroutine(DoPlayAttarck());
        return GameConfig.MonsterAttackStepTime * 3;
    }

    /// <summary>
    /// Show blood bar.
    /// </summary>
    /// <param name="show">If set to <c>true</c> show.</param>
    public void ShowBlood(bool show)
    {
        BloodController.Show(show);
    }

    /// <summary>
    /// Resetup.
    /// </summary>
    /// <remarks>Reset original position to the enemy.</remarks>
    public void Reset()
    {
        EnemySprite.transform.localPosition = originalPosition;
        ShowBlood(true);
        ShowAimTo(false);
    }

    public void ResetPosition()
    {
        EnemySprite.transform.localPosition = originalPosition;
    }

    public void Move()
    {
        StopAll();
        ResetPosition();

        var distance = new Vector3(MoveDuration * MoveSpeed * BaseWidget.width, 0, 0);
        // using itween here to make consistent with texture looper, to keep in the same frame. NGUI tween has issue by eating frames.
        iTween.MoveFrom(gameObject,
            iTween.Hash("position", (transform.localPosition + distance), "time", MoveDuration, "easetype", "linear",
                "islocal", true));
    }

    #endregion

    #region Private Methods

    private IEnumerator DoPlayAttarck()
    {
        var enemySprite = EnemySprite.GetComponent<UIWidget>();
        yield return new WaitForSeconds(GameConfig.MonsterAttackStepTime);
        //AttackWhite(enemySprite);
        yield return new WaitForSeconds(GameConfig.MonsterAttackStepTime);
        AttackBlack(enemySprite);
        yield return new WaitForSeconds(GameConfig.MonsterAttackStepTime);
        //AttackWhite(enemySprite);
        yield return new WaitForSeconds(GameConfig.MonsterAttackStepTime);
        AttackNormal(enemySprite);
    }

    private void AttackBlack(UIWidget enemySprite)
    {
        enemySprite.color = Color.black;
        enemySprite.HighLight = 0f;
        enemySprite.Invalidate(true);
        enemySprite.panel.Refresh();
    }

    private void AttackWhite(UIWidget enemySprite)
    {
        enemySprite.color = Color.white;
        enemySprite.HighLight = HighLight;
        enemySprite.Invalidate(true);
        enemySprite.panel.Refresh();
    }
    private void AttackNormal(UIWidget enemySprite)
    {
        enemySprite.color = Color.white;
        enemySprite.HighLight = 0f;
        enemySprite.Invalidate(true);
        enemySprite.panel.Refresh();
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
