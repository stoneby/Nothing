
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.kx.sglm.gs.battle.share.data.record;
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
        recordList.ForEach(item =>
        {
            // update buff manager count.
            BuffCountManager.Clear();

            var buffIndex = item.ShowId - 1;
            var buffSize = Enum.GetNames(typeof(BuffManager.BuffType)).Count();
            if (buffIndex < 0 || buffIndex > buffSize)
            {
                Logger.LogError("Buff index: " + buffIndex + " should be in range of 0 and " + buffSize + ".");
                return;
            }
            BuffCountManager[(BuffManager.BuffType)buffIndex] = item.LeftRound;
        });

        // set zero attack flag.
        ZeroAttack = BuffCountManager.ContainsKey(BuffManager.BuffType.Palsy) && (BuffCountManager[BuffManager.BuffType.Palsy] > 0);
    }

    public void ShowBuff()
    {
        var buffManager = BuffManager.Instance;
        foreach (var pair in BuffCountManager)
        {
            var buffType = pair.Key;
            var counter = pair.Value;
            if (counter > 0)
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
        foreach (var hurt in HurtValueList)
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
