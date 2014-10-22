using com.kx.sglm.gs.battle.share.data.record;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Buff data
/// </summary>
public class CharacterBuffController : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<BuffManager.BuffType, int> BuffCountManager;

    /// <summary>
    /// Base character hp value.
    /// </summary>
    public float BaseValue;

    /// <summary>
    /// List of character hp values after hurt.
    /// </summary>
    [HideInInspector]
    public List<float> HurtValueList;

    /// <summary>
    /// Flag indicates if attack value appears to zero.
    /// </summary>
    public bool ZeroAttack;

    /// <summary>
    /// Delegate to update hurt value.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="value">Hurt value to update</param>
    public delegate IEnumerator UpdateHurtValue(GameObject sender, float value);

    public UpdateHurtValue OnUpdateHurtValue;

    /// <summary>
    /// Delegate to seal occurs.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param show="show">Flag indicates if show or hide.</param>
    public delegate void Seal(GameObject sender, bool show);

    public Seal OnSeal;

    public float HurtDelay;

    public bool CanSelected
    {
        get
        {
            return
                !((BuffCountManager.ContainsKey(BuffManager.BuffType.Freeze) &&
                   BuffCountManager[BuffManager.BuffType.Freeze] > 0) ||
                  (BuffCountManager.ContainsKey(BuffManager.BuffType.Petrify) &&
                   BuffCountManager[BuffManager.BuffType.Petrify] > 0));
        }
    }

    /// <summary>
    /// Set buff data due to record list.
    /// </summary>
    /// <param name="recordList">Record list per character.</param>
    public void Set(List<FighterStateRecord> recordList)
    {
        // update buff manager count.
        BuffCountManager.Clear();

        recordList.ForEach(item =>
        {
            var buffIndex = item.ShowId - 1;
            var buffSize = Enum.GetNames(typeof(BuffManager.BuffType)).Count();
            if (buffIndex < 0 || buffIndex > buffSize)
            {
                Logger.LogError("Buff index: " + buffIndex + " should be in range of 0 and " + buffSize + ".");
                return;
            }
            //item.getParam(BattleKeyConstants.BATTLE_BUFF_CUR_SHIELD_ORDER);
            BuffCountManager[(BuffManager.BuffType)buffIndex] = item.LeftRound;
        });

        // set zero attack flag.
        ZeroAttack = (BuffCountManager.ContainsKey(BuffManager.BuffType.Palsy) && (BuffCountManager[BuffManager.BuffType.Palsy] > 0)) ||
            (BuffCountManager.ContainsKey(BuffManager.BuffType.Sleep) && (BuffCountManager[BuffManager.BuffType.Sleep] > 0));

        var hasSealed = BuffCountManager.ContainsKey(BuffManager.BuffType.Seal);
        if (hasSealed)
        {
            var isPlaying =  (BuffCountManager[BuffManager.BuffType.Seal] > 0);
            if (OnSeal != null)
            {
                OnSeal(gameObject, isPlaying);
            }
        }
    }

    public void ShowBuff()
    {
        var buffManager = BuffManager.Instance;
        foreach (var pair in BuffCountManager)
        {
            var buffType = pair.Key;
            var counter = pair.Value;
            // buff count equals 0 means stop, -1 means always show, greater than 0 means buff count left.
            if (counter != 0)
            {
                buffManager.Show(buffType, gameObject);
            }
            else
            {
                buffManager.Stop(buffType, gameObject);
            }
        }
    }

    public void ShowBuffCD(BuffBarController buffController)
    {
        foreach (var pair in BuffCountManager)
        {
            var buffType = pair.Key;
            var count = pair.Value;
            if (count <= 0)
            {
                buffController.gameObject.SetActive(false);
            }
            else
            {
                buffController.gameObject.SetActive(true);
                var label = buffController.BuffLabel.GetComponent<UILabel>();
                label.text = "" + count;
                label.color = BuffManager.Instance.BuffColorList[(int)buffType];
            }
        }
    }

    public void ResetBuff()
    {
        // reset buff manager.
        var buffManager = BuffManager.Instance;
        foreach (var buffType in BuffCountManager.Select(pair => pair.Key))
        {
            buffManager.Stop(buffType, gameObject);
        }
        BuffCountManager.Clear();

        // reset buff data.
        Reset();
    }

    public void ShowDebuff()
    {
        StartCoroutine(DoShowDebuff());
    }

    public void Reset()
    {
        if (BuffCountManager == null)
        {
            BuffCountManager = new Dictionary<BuffManager.BuffType, int>();
        }
        BuffCountManager.Clear();

        if (HurtValueList == null)
        {
            HurtValueList = new List<float>();
        }
        HurtValueList.Clear();
    }

    private IEnumerator DoShowDebuff()
    {
        var hurtList = HurtValueList.ToList();
        foreach (var hurt in hurtList)
        {
            if (OnUpdateHurtValue != null)
            {
                yield return StartCoroutine(OnUpdateHurtValue(gameObject, hurt));
            }
            yield return new WaitForSeconds(HurtDelay);
        }
    }

    private void Awake()
    {
        Reset();
    }
}
