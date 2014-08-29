using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class NineAttackController : MonoBehaviour
{
    #region Private Fields

    private GameObject effectContainer;

    private List<GameObject> NineAttackSuitZhan = new List<GameObject>();
    private List<GameObject> NineAttackSuitDa = new List<GameObject>();
    private List<GameObject> NineAttackSuitMo = new List<GameObject>();
    private List<GameObject> NineAttackSuitTu = new List<GameObject>();
    private List<GameObject> NineAttackSuitGong = new List<GameObject>();

    #endregion

    #region Private Methods

    public void PlayEffect()
    {
        StartCoroutine(DoPlayEffect());
    }

    private IEnumerator DoPlayEffect()
    {
        effectControllerList[0].gameObject.SetActive(true);
        effectControllerList[0].Play(false);
        yield return new WaitForSeconds(effectControllerList[0].Duration - OverLayTime);
        effectControllerList[1].gameObject.SetActive(true);
        effectControllerList[1].Play(true);
        yield return new WaitForSeconds(LoopingTime - OverLayTime);
        effectControllerList[1].Stop();
        effectControllerList[2].gameObject.SetActive(true);
        effectControllerList[2].Play(false);
    }

    public void StopEffect()
    {
        StopAllCoroutines();
        effectControllerList.ForEach(effectController =>
        {
            effectController.Stop();
            effectController.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Public Fields

    #endregion

    #region Public Methods

    public List<GameObject> EffectList;

    public GameObject StartPoint;
    public GameObject EndPoint;

    public float LoopingTime;
    public float OverLayTime;

    #endregion

    #region Private Fields

    private List<EffectController> effectControllerList;

    private const int EffectCount = 3;

    #endregion

    private void Initialize()
    {
        effectControllerList = new List<EffectController>();
        EffectList.ForEach(effect =>
        {
            var effectController = effect.GetComponent<EffectController>() ?? effect.AddComponent<EffectController>();
            effectControllerList.Add(effectController);
        });

        if (effectControllerList.Count != EffectCount)
        {
            Logger.LogError("Effect count should be equals to " + EffectCount);
            return;
        }

        // make the 1st and 2nd effect to start point.
        effectControllerList[0].transform.position = StartPoint.transform.position;
        effectControllerList[1].transform.position = StartPoint.transform.position;
        // make the 3rd effect to end point.
        effectControllerList[2].transform.position = EndPoint.transform.position;
    }

    #region Mono

    void Awake()
    {
        if (EffectList == null)
        {
            Logger.LogError("EffectList should not be null.");
            return;
        }

        Initialize();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect((float)Screen.width / 10, 50, 200, 50), "Start"))
        {
            PlayEffect();
        }

        if (GUI.Button(new Rect((float)Screen.width / 10, 300, 200, 50), "Stop"))
        {
            StopEffect();
        }
    }

    #endregion
}
