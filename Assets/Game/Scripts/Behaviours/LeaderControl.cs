using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.input;
using Template.Auto.Skill;
using UnityEngine;

public class LeaderControl : MonoBehaviour
{
    public GameObject SpriteBg;
    public UISprite SpriteHead;

    public LeaderData Data;

    public Character Character;

    /// <summary>
    /// Seal particle effect.
    /// </summary>
    public EffectSpawner SealEffect;
    public EffectSpawner LightEffect;

    public delegate void ActiveLeaderSkill(LeaderData data);

    /// <summary>
    /// Callback on active leader skill.
    /// </summary>
    public ActiveLeaderSkill OnActiveLeaderSkill;

    public UILabel CostMP;

    private int currentCD;

    private FighterInfo figherData;
    private HeroBattleSkillTemplate skillData;

    /// <summary>
    /// Initialize.
    /// </summary>
    public void Initialize()
    {
        if (Character == null)
        {
            Logger.LogError("Character should not be null, please set either in inspector or by script.");
            return;
        }

        HeroConstant.SetHeadByIndex(SpriteHead, Character.IDIndex);
    }

    /// <summary>
    /// Reset status.
    /// </summary>
    public void Reset()
    {
        LightEffect.Stop();
        StopSeal();
    }

    /// <summary>
    /// Set inner data before using.
    /// </summary>
    /// <param name="data">Fighter infor.</param>
    /// <param name="character">Character reference.</param>
    /// <param name="leaderIndex">Leader index.</param>
    public void SetData(FighterInfo data, Character character, int leaderIndex)
    {
        figherData = data;
        Character = character;
        skillData = HeroModelLocator.Instance.GetLeaderSkillTemplateById(figherData.ActiveSkillId);
        if (skillData != null)
        {
            Data.BaseCd = skillData.CostMP;
            CostMP.text = skillData.CostMP.ToString();
            Logger.LogWarning("Leader base CD: " + Data.BaseCd + ", name: " + name);
        }
        Data.LeaderIndex = leaderIndex;
    }

    /// <summary>
    /// Set current cd.
    /// </summary>
    /// <param name="currentcd">Current cd</param>
    public void SetCD(int currentcd)
    {
        currentCD = currentcd;
        var effectEnabled = (skillData != null && currentcd >= Data.BaseCd);
        if (effectEnabled)
        {
            LightEffect.Play();
        }
        else
        {
            LightEffect.Stop();
        }
    }

    public void PlaySeal(bool show)
    {
        if (show)
        {
            PlaySeal();
        }
        else
        {
            StopSeal();
        }
    }

    /// <summary>
    /// Callback on head got clicked.
    /// </summary>
    public void OnHeadClick()
    {
        if (currentCD < Data.BaseCd || skillData == null)
        {
            return;
        }

        var window = WindowManager.Instance.GetWindow<ActiveSkillConfirmWindow>();
        if (window == null)
        {
            Logger.Log("!!!!!!!!!!!!!Assert window equal to null.");
        }
        window.AssertType = ActiveSkillConfirmWindow.Type.OkCancel;
        window.Title = skillData.Name;
        window.Message = skillData.Desc;
        WindowManager.Instance.Show(typeof(ActiveSkillConfirmWindow), true);
        window.OkButtonClicked = OnAssertButtonClicked;
        window.CancelButtonClicked = OnCancelClicked;

        BattleModelLocator.Instance.CanSelectHero = false;
    }

    /// <summary>
    /// Set leader's collider enable or disable.
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetLeaderCollider(bool isEnable)
    {
        var colliderItem = gameObject.GetComponent<BoxCollider>();
        if (colliderItem)
        {
            colliderItem.enabled = isEnable;
        }
    }

    /// <summary>
    /// Callback on assert ok button clicked.
    /// </summary>
    /// <param name="sender">Sender</param>
    private void OnAssertButtonClicked(GameObject sender)
    {
        var action = new UseActiveSkillAction { FighterIndex = Data.LeaderIndex };
        BattleModelLocator.Instance.MainBattle.handleBattleEvent(action);
        BattleModelLocator.Instance.Skill = skillData;

        if (OnActiveLeaderSkill != null)
        {
            OnActiveLeaderSkill(Data);
        }

        BattleModelLocator.Instance.CanSelectHero = true;
    }

    /// <summary>
    /// Callback on assert cancel button clicked.
    /// </summary>
    /// <param name="sender"></param>
    private void OnCancelClicked(GameObject sender)
    {
        BattleModelLocator.Instance.CanSelectHero = true;
    }

    private void PlaySeal()
    {
        collider.enabled = false;
        SealEffect.Play();
    }

    private void StopSeal()
    {
        collider.enabled = true;
        SealEffect.Stop();
    }
}
