using Assets.Game.Scripts.Common.Model;
using com.kx.sglm.gs.battle.share.data;
using com.kx.sglm.gs.battle.share.input;
using Template.Auto.Skill;
using UnityEngine;

public class LeaderControl : MonoBehaviour
{
    public GameObject SpriteBg;
    public GameObject SpriteLight;
    public UISprite SpriteHead;

    public LeaderData Data;

    public Character Character;

    /// <summary>
    /// Seal particle effect.
    /// </summary>
    public GameObject SealEffect;

    public delegate void ActiveLeaderSkill(LeaderData data);

    /// <summary>
    /// Callback on active leader skill.
    /// </summary>
    public ActiveLeaderSkill OnActiveLeaderSkill;

    private GameObject shineObj;
    private int currentCD;

    private FighterInfo figherData;
    private HeroBattleSkillTemplate skillData;

    private EffectController sealEffectClone;

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
        SpriteLight.SetActive(false);
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
        SpriteLight.SetActive((skillData != null && currentcd >= Data.BaseCd));
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
        Alert.Show(AssertionWindow.Type.OkCancel, skillData.Name, skillData.Desc, OnAssertButtonClicked, OnCancelClicked);
        BattleModelLocator.Instance.CanSelectHero = false;
    }

    /// <summary>
    /// Callback on assert ok button clicked.
    /// </summary>
    /// <param name="sender">Sender</param>
    private void OnAssertButtonClicked(GameObject sender)
    {
        var action = new UseActiveSkillAction {FighterIndex = Data.LeaderIndex};
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
        if (sealEffectClone == null)
        {
            return;
        }
        sealEffectClone.gameObject.SetActive(true);
        collider.enabled = false;
        sealEffectClone.Play(true);
    }

    private void StopSeal()
    {
        if (sealEffectClone == null)
        {
            return;
        }
        sealEffectClone.gameObject.SetActive(false);
        collider.enabled = true;
        sealEffectClone.Stop();
    }

    private void Awake()
    {
        var sealEffect = Instantiate(SealEffect) as GameObject;
        sealEffect.transform.position = transform.position;
        sealEffect.transform.localScale = Vector3.one;
        sealEffect.transform.parent = transform;

        var renderQueue = sealEffect.GetComponent<SetRenderQueue>() ?? sealEffect.AddComponent<SetRenderQueue>();
        renderQueue.RenderQueue = RenderQueue.FaceEffect;

        sealEffectClone = sealEffect.GetComponent<EffectController>() ?? sealEffect.AddComponent<EffectController>();
        sealEffectClone.gameObject.SetActive(false);
    }
}
