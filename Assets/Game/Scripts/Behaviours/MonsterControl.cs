using com.kx.sglm.gs.battle.share.data;
using UnityEngine;

/// <summary>
/// Enemy Controller that control behaviours of enemy.
/// </summary>
public class MonsterControl : MonoBehaviour
{
    #region Public Fields

    public GameObject CdLabel;
    public GameObject AimSprite;
    public bool IsShowAim;

    /// <summary>
    /// Be attacked location.
    /// </summary>
    public GameObject AttackLocation;

    public GameObject BattleBG;

    /// <summary>
    /// Blood bar controller.
    /// </summary>
    public BloodBarController BloodController;
    public BuffBarController BuffBarController;

    public BattlegroundController BattleController
    {
        get
        {
            if (!BattleBG)
            {
                BattleBG = GameObject.FindGameObjectWithTag("BattleBG");
            }

            if (BattleBG)
            {
                return BattleBG.GetComponent<BattlegroundController>();
            }
            else
            {
                Logger.LogError("!!!!!!!!!Can't get taged \"BattleBG\" object in MonsterControl.");
                return null;
            }
        }
    }

    /// <summary>
    /// Base widget of moving standard.
    /// </summary>
    public UIWidget BaseWidget
    {
        get { return BattleController.GetComponent<UIWidget>(); }
    }

    /// <summary>
    /// Speed of moving to next round.
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// Duration of moving to next round.
    /// </summary>
    public float MoveDuration;

    public Vector3 MoveDistance
    {
        get { return new Vector3(MoveDuration * MoveSpeed * BaseWidget.width, 0, 0); }
    }

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
        cdLabel.color = (cd == WarningCdValue) ? Color.red : Color.white;
        cdLabel.text = "" + cd;
    }

    /// <summary>
    /// Show aim to sprite.
    /// </summary>
    /// <param name="flag">If set to <c>true</c> flag.</param>
    public void ShowAimTo(bool flag)
    {
        AimSprite.SetActive(flag);
        if (flag)
        {
            var lockController = gameObject.GetComponent<ObjectMoveController>();
            lockController.CurrentObject = AimSprite;
            lockController.LockMove(true, AimSprite.transform.position);
        }
        else
        {
            var lockController = gameObject.GetComponent<ObjectMoveController>();
            lockController.StopMove();
        }
        IsShowAim = flag;
    }

    /// <summary>
    /// Play shake.
    /// </summary>
    public void PlayShake()
    {
    }

    /// <summary>
    /// Play big shake.
    /// </summary>
    public void PlayBigShake()
    {
    }

    /// <summary>
    /// Play attack animation.
    /// </summary>
    /// <returns>The attrack.</returns>
    public float PlayAttack()
    {
        CharacterData.PlayState(Character.State.Attack, false);
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
        ShowBlood(true);
        ShowAimTo(false);
    }

    public void Move()
    {
        // using itween here to make consistent with texture looper, to keep in the same frame. NGUI tween has issue by eating frames.
        iTween.MoveTo(CharacterData.gameObject,
            iTween.Hash("position", (CharacterData.transform.localPosition - MoveDistance), "time", MoveDuration, "easetype", "linear",
                "islocal", true));
    }

    #endregion

    #region Mono

    private void Awake()
    {
        // default hide aim sprite.
        AimSprite.SetActive(false);
        BattleBG = GameObject.FindGameObjectWithTag("BattleBG");
    }

    #endregion
}
