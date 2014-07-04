using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDisplayer : MonoBehaviour
{
    public MyPoolManager BuffIntroPool;
    public MyPoolManager BuffLoopPool;

    public float Interval;

    public float Duration { get; private set; }

    public bool IsPlaying(GameObject parent)
    {
        return isPlayingMap.ContainsKey(parent) && isPlayingMap[parent];
    }

    private float loopDuration;

    private Dictionary<GameObject, GameObject> buffIntroMap;
    private Dictionary<GameObject, GameObject> buffLoopMap;
    private Dictionary<GameObject, bool> isPlayingMap;

    public void ShowIntro(GameObject parent)
    {
        Take(parent);
        ShowBuff(buffIntroMap[parent], parent, false);
    }

    public void ShowLoop(GameObject parent)
    {
        Take(parent);
        ShowBuff(buffLoopMap[parent], parent, true);
    }

    public void Show(GameObject parent)
    {
        // pass by playing state.
        if (isPlayingMap.ContainsKey(parent) && isPlayingMap[parent])
        {
            return;
        }

        Logger.LogWarning("Show buff in parent: " + parent.name);

        isPlayingMap[parent] = true;

        Take(parent);
        StartCoroutine(DoShow(parent));
    }

    public void Stop(GameObject parent)
    {
        // pass by not playing state.
        if (!isPlayingMap.ContainsKey(parent) || !isPlayingMap[parent])
        {
            return;
        }

        Logger.LogWarning("Stop buff." + parent.name);

        isPlayingMap[parent] = false;

        var buffIntro = buffIntroMap[parent];
        var buffLoop = buffLoopMap[parent];
        buffIntroMap.Remove(parent);
        buffLoopMap.Remove(parent);
        BuffIntroPool.Return(buffIntro);
        BuffLoopPool.Return(buffLoop);
        StopBuff(buffIntro);
        StopBuff(buffLoop);
    }

    private IEnumerator DoShow(GameObject parent)
    {
        ShowIntro(parent);
        yield return new WaitForSeconds(Duration + Interval);
        ShowLoop(parent);
        yield return new WaitForSeconds(loopDuration);
    }

    private static void ShowBuff(GameObject buff, GameObject parent, bool loop)
    {
        // make buff child on character.
        SetParent(buff, parent);

        var buffController = buff.GetComponent<EffectController>() ?? buff.AddComponent<EffectController>();
        buffController.Play(loop);
    }

    private static void SetParent(GameObject buff, GameObject parent)
    {
        buff.transform.parent = parent.transform;
        buff.transform.position = parent.transform.position;
        buff.transform.localScale = Vector3.one;
    }

    private static void StopBuff(GameObject buff)
    {
        var buffController = buff.GetComponent<EffectController>() ?? buff.AddComponent<EffectController>();
        buffController.Stop();
    }

    private float Initialize(GameObject buff)
    {
        var renderQueue = buff.GetComponent<SetRenderQueue>() ?? buff.AddComponent<SetRenderQueue>();
        renderQueue.renderQueue = RenderQueue.Overlay;

        var effectConroller = buff.GetComponent<EffectController>() ?? buff.AddComponent<EffectController>();
        return effectConroller.Duration;
    }

    private void Take(GameObject parent)
    {
        if (!buffIntroMap.ContainsKey(parent))
        {
            buffIntroMap[parent] = BuffIntroPool.Take();
        }
        if (!buffLoopMap.ContainsKey(parent))
        {
            buffLoopMap[parent] = BuffLoopPool.Take();
        }

        var buffIntro = buffIntroMap[parent];
        var buffLoop = buffLoopMap[parent];
        buffIntro.SetActive(true);
        buffLoop.SetActive(true);

        SetParent(buffIntro, parent);
        SetParent(buffLoop, parent);

        Duration = Initialize(buffIntro);
        loopDuration = Initialize(buffLoop);
    }

    private void Awake()
    {
        buffIntroMap = new Dictionary<GameObject, GameObject>();
        buffLoopMap = new Dictionary<GameObject, GameObject>();
        isPlayingMap = new Dictionary<GameObject, bool>();
    }
}
