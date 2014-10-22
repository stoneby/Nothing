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

    public Vector3 Start;
    public Vector3 MoveEnd;
    public Vector3 End;
    public List<Vector3> AllEnd;

    public AudioSource NineAttackAudioSource;

    #endregion

    #region Private Fields

    private const bool IsLoopAudio = false;

    //Loaded Resources.
    private List<List<GameObject>> loadedEffects;
    private List<AudioClip> loadedAudioClips;

    private List<EffectController> allEffectControllerList;
    private List<EffectController> endEffectControllerList;
    private AudioClip clipToPlay;

    private readonly int attackTypeCount = Enum.GetNames(typeof(HeroJob)).Count();
    private const int EffectCount = 3;
    private const string NineAttackEffectBase = "Prefabs/Effect/NineAttackEffect/";
    private const string NineAttackSoundBase = "Sounds/9attack_job";

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
            return LoopTime1 + allEffectControllerList[1].Duration +
                   allEffectControllerList[2].Duration - OverLayTimeAfterPeriod1 - OverLayTimeAfterPeriod2;
        }
    }

    #endregion

    #region Public Methods

    public void LoadResource()
    {
        loadedEffects = new List<List<GameObject>>();
        loadedAudioClips = new List<AudioClip>();
        for (var i = 0; i < attackTypeCount; i++)
        {
            //Load effect resource.
            var tempGameObjectList = new List<GameObject>();
            for (var j = 0; j < EffectCount; j++)
            {
                var effectPath = NineAttackEffectBase + (i + 1) + "_" + (j + 1);
                var effectObject = Resources.Load(effectPath) as GameObject;

                if (effectObject == null)
                {
                    Logger.LogError("Can't load effectObject resource at:" + effectPath);
                    return;
                }

                tempGameObjectList.Add(effectObject);
            }

            if (tempGameObjectList.Count != EffectCount)
            {
                Logger.LogError("Not correct num:" + tempGameObjectList.Count + ", not equal to " + EffectCount);
                return;
            }
            loadedEffects.Add(tempGameObjectList);

            //Load audio resource.
            var audioPath = NineAttackSoundBase + (i + 1);
            var audioClip = Resources.Load(audioPath) as AudioClip;

            if (audioClip == null)
            {
                Logger.LogError("Can't load audioClip resource at:" + audioPath);
                return;
            }

            loadedAudioClips.Add(audioClip);
        }
    }

    public void Initialize(int jobIndex)
    {
        if (jobIndex > attackTypeCount || jobIndex < 1)
        {
            Logger.LogError("Job index should be in range [1, " + attackTypeCount + "], but is: " + jobIndex);
            jobIndex = 1;
        }

        allEffectControllerList = new List<EffectController>();
        for (var i = 0; i < EffectCount; i++)
        {
            var effectObject = Instantiate(loadedEffects[jobIndex - 1][i]) as GameObject;
            if (effectObject == null)
            {
                Logger.LogError("EffectObject is null with loading loadedEffects:" + (jobIndex - 1) + "," + i);
                continue;
            }

            Utils.MoveToParent(Parent.transform, effectObject.transform);

            var setRenderQueue = effectObject.GetComponent<SetRenderQueue>() ??
                                 effectObject.AddComponent<SetRenderQueue>();
            setRenderQueue.RenderQueue = RenderQueue.Overlay;

            var effectController = effectObject.GetComponent<EffectController>() ?? effectObject.AddComponent<EffectController>();
            allEffectControllerList.Add(effectController);
        }

        if (AllEnd.Count > 1)
        {
            endEffectControllerList = new List<EffectController>();
            endEffectControllerList.Add(allEffectControllerList[EffectCount - 1]);
            for (var j = 1; j < AllEnd.Count; j++)
            {
                var endEffectObject = Instantiate(allEffectControllerList[EffectCount - 1].gameObject) as GameObject;
                Utils.MoveToParent(Parent.transform, endEffectObject.transform);
                endEffectControllerList.Add(endEffectObject.GetComponent<EffectController>() ?? endEffectObject.AddComponent<EffectController>());
            }
        }

        if (allEffectControllerList.Count != EffectCount)
        {
            Logger.LogError("Effect list count should be equals to " + EffectCount + ", but is: " + allEffectControllerList.Count);
        }

        NineAttackAudioSource.loop = IsLoopAudio;
        clipToPlay = loadedAudioClips[jobIndex - 1];
    }

    public void Play()
    {
        StartCoroutine(DoPlay());
    }

    public void Cleanup()
    {
        //Clean up all effects reference.
        StopAllCoroutines();
        if (allEffectControllerList != null)
        {
            allEffectControllerList.ForEach(effectController =>
            {
                effectController.Stop();
                Destroy(effectController.gameObject);
            });
            allEffectControllerList.Clear();
        }

        if (endEffectControllerList != null)
        {
            endEffectControllerList.ForEach(effectController =>
            {
                effectController.Stop();
                Destroy(effectController.gameObject);
            });
            endEffectControllerList.Clear();
        }

        //Clean up all clips reference.
        clipToPlay = null;
    }

    #endregion

    #region Private Methods

    private IEnumerator DoPlay()
    {
        // make the 1st and 2nd effect to start point.
        allEffectControllerList[0].transform.position = Start;
        allEffectControllerList[1].transform.position = Start;

        //make the 2nd effect's rotation correct.
        allEffectControllerList[1].transform.localRotation = Utils.GetRotation(Start, MoveEnd);

        // make the 3rd effect to end point.
        allEffectControllerList[2].transform.position = End;
        if (AllEnd.Count > 1)
        {
            for (var i = 0; i < endEffectControllerList.Count; i++)
            {
                endEffectControllerList[i].transform.position = AllEnd[i];
            }
        }

        //Play audio.
        NineAttackAudioSource.PlayOneShot(clipToPlay);

        //Play effect.
        allEffectControllerList[0].gameObject.SetActive(true);
        allEffectControllerList[0].Play(true);
        yield return new WaitForSeconds(LoopTime1 - OverLayTimeAfterPeriod1);
        allEffectControllerList[0].Stop();
        allEffectControllerList[0].gameObject.SetActive(false);

        allEffectControllerList[1].gameObject.SetActive(true);
        allEffectControllerList[1].Play(true);
        yield return new WaitForSeconds(allEffectControllerList[1].Duration - OverLayTimeAfterPeriod2);
        allEffectControllerList[1].Stop();
        allEffectControllerList[1].gameObject.SetActive(false);

        if (AllEnd.Count == 1)
        {
            allEffectControllerList[2].gameObject.SetActive(true);
            allEffectControllerList[2].Play(true);
            yield return new WaitForSeconds(allEffectControllerList[2].Duration - OverLayTimeAfterPeriod2);
            allEffectControllerList[2].Stop();
            allEffectControllerList[2].gameObject.SetActive(false);
        }
        else if (AllEnd.Count > 1)
        {
            foreach (var item in endEffectControllerList)
            {
                item.gameObject.SetActive(true);
                item.Play(true);
            }
            yield return new WaitForSeconds(endEffectControllerList[0].Duration - OverLayTimeAfterPeriod2);
            foreach (var item in endEffectControllerList)
            {
                item.Stop();
                item.gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
