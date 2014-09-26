using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Full nine attack controller, including 3 effects totally for each job.
/// </summary>
public class NineAttackController : MonoBehaviour
{
    #region Public Fields

    public GameObject Parent;

    public GameObject Start;
    public GameObject End;

    #endregion

    #region Private Fields

    private List<EffectController> effectControllerList;
    private const int EffectCount = 3;
    private const string NineAttackEffectBase = "Prefabs/Effect/NineAttackEffect/";

    private const float OverLayTimeAfterPeriod1 = 0;
    private const float OverLayTimeAfterPeriod2 = 0;
    private const float LoopTime1 = 2.0f;
    private const float LoopTime2 = 0.5f;
    private const float LoopTime3 = 0.5f;

    #endregion

    #region Public Properties

    public float Duration
    {
        get
        {
            return LoopTime1 + effectControllerList[1].Duration +
                   effectControllerList[2].Duration - OverLayTimeAfterPeriod1 - OverLayTimeAfterPeriod2;
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(int jobIndex)
    {
        var jobTotalCount = Enum.GetNames(typeof(HeroJob)).Count();
        if (jobIndex > jobTotalCount || jobIndex < 1)
        {
            Logger.LogError("Job index should be in range [1, " + jobTotalCount + "], but is: " + jobIndex);
            jobIndex = 1;
        }

        effectControllerList = new List<EffectController>();
        for (var i = 0; i < EffectCount; i++)
        {
            var effectPath = NineAttackEffectBase + jobIndex + "_" + (i + 1);
            var effectObject = Instantiate(Resources.Load(effectPath)) as GameObject;
            if (effectObject == null)
            {
                continue;
            }

            Utils.MoveToParent(Parent.transform, effectObject.transform);

            var setRenderQueue = effectObject.GetComponent<SetRenderQueue>() ??
                                 effectObject.AddComponent<SetRenderQueue>();
            setRenderQueue.RenderQueue = RenderQueue.Overlay;

            var effectController = effectObject.GetComponent<EffectController>() ?? effectObject.AddComponent<EffectController>();
            effectControllerList.Add(effectController);
        }

        if (effectControllerList.Count != EffectCount)
        {
            Logger.LogError("Effect list count should be equals to " + EffectCount + ", but is: " + effectControllerList.Count);
        }
    }

    public void PlayEffect()
    {
        StartCoroutine(DoPlayEffect());
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        effectControllerList.ForEach(effectController =>
        {
            effectController.Stop();
            Destroy(effectController.gameObject);
        });
        effectControllerList.Clear();
    }

    #endregion

    #region Private Methods

    private IEnumerator DoPlayEffect()
    {
        // make the 1st and 2nd effect to start point.
        effectControllerList[0].transform.position = Start.transform.position;
        effectControllerList[1].transform.position = Start.transform.position;
        // make the 3rd effect to end point.
        effectControllerList[2].transform.position = End.transform.position;

        effectControllerList[0].gameObject.SetActive(true);
        effectControllerList[0].Play(true);
        yield return new WaitForSeconds(LoopTime1 - OverLayTimeAfterPeriod1);
        effectControllerList[0].Stop();
        effectControllerList[0].gameObject.SetActive(false);

        effectControllerList[1].gameObject.SetActive(true);
        effectControllerList[1].Play(true);
        yield return new WaitForSeconds(effectControllerList[1].Duration - OverLayTimeAfterPeriod2);
        effectControllerList[1].Stop();
        effectControllerList[1].gameObject.SetActive(false);

        effectControllerList[2].gameObject.SetActive(true);
        effectControllerList[2].Play(true);
        yield return new WaitForSeconds(effectControllerList[2].Duration - OverLayTimeAfterPeriod2);
        effectControllerList[2].Stop();
        effectControllerList[2].gameObject.SetActive(false);
    }

    #endregion
}
