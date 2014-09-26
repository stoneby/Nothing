using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDisplayer : MonoBehaviour
{
    public MyPoolManager BuffIntroPool;
    public MyPoolManager BuffLoopPool;

    public float Interval;

    public bool IsPlaying(GameObject parent)
    {
        return isPlayingMap.ContainsKey(parent) && isPlayingMap[parent];
    }

    private float introDuration;
    private float loopDuration;

    private Dictionary<GameObject, GameObject> buffIntroMap;
    private Dictionary<GameObject, GameObject> buffLoopMap;
    private Dictionary<GameObject, bool> isPlayingMap;

    public void ShowIntro(GameObject parent)
    {
        if (BuffIntroPool == null)
        {
            return;
        }
        introDuration = Take(parent, BuffIntroPool, buffIntroMap);
        ShowBuff(buffIntroMap[parent], parent, false);
    }

    public void ShowLoop(GameObject parent)
    {
        if (BuffLoopPool == null)
        {
            return;
        }

        loopDuration = Take(parent, BuffLoopPool, buffLoopMap);
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

        Stop(parent, BuffIntroPool, buffIntroMap);
        Stop(parent, BuffLoopPool, buffLoopMap);
    }

    private void Stop(GameObject parent, MyPoolManager pool, IDictionary<GameObject, GameObject> buffMap)
    {
        if (pool == null)
        {
            return;
        }
        var buff = buffMap[parent];
        buffMap.Remove(parent);
        pool.Return(buff);
        StopBuff(buff);
    }

    private IEnumerator DoShow(GameObject parent)
    {
        if (BuffIntroPool != null)
        {
            ShowIntro(parent);
            yield return new WaitForSeconds(introDuration + Interval);
        }

        if (BuffLoopPool != null)
        {
            ShowLoop(parent);
            yield return new WaitForSeconds(loopDuration);
        }
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
        renderQueue.RenderQueue = RenderQueue.BattleEffect;

        var effectConroller = buff.GetComponent<EffectController>() ?? buff.AddComponent<EffectController>();
        return effectConroller.Duration;
    }

    private float Take(GameObject parent, MyPoolManager pool, IDictionary<GameObject, GameObject> buffMap)
    {
        if (pool == null)
        {
            return 0f;
        }

        if (!buffMap.ContainsKey(parent))
        {
            buffMap[parent] = pool.Take();
        }
        var buff = buffMap[parent];
        buff.SetActive(true);
        SetParent(buff, parent);

        return Initialize(buffMap[parent]);
    }

    private void Awake()
    {
        buffIntroMap = new Dictionary<GameObject, GameObject>();
        buffLoopMap = new Dictionary<GameObject, GameObject>();
        isPlayingMap = new Dictionary<GameObject, bool>();
    }
}
