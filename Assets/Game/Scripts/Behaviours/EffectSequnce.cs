using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class EffectSequnce : MonoBehaviour
{
    public int RenderQ;
    public enum EffectType
    {
        None,
        Particle
    }

    [Serializable]
    public class EffectInfo
    {
        public GameObject EffectObj;
        public EffectType EffectType;
        public float StartTime;
        public float Douration;
        public bool UseMove;
        public float MoveDelay;
        public float MoveDouration;
        public Vector3 PositonFrom;
        public Vector3 PositionTo;
        public bool UseRotation;
        public float RotationDelay;
        public float RoationDouration;
        public Vector3 RotationFrom;
        public Vector3 RotationTo;
        public bool IsLocal = true;
    }

    public List<EffectInfo> EffectInfos = new List<EffectInfo>();
    private readonly Dictionary<EffectInfo, GameObject> cloneObjects = new Dictionary<EffectInfo, GameObject>(); 

    public void Play()
    {
        Stop();
        EffectInfos.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
        StartCoroutine(PlaySequnceEffect());
        var infos = new List<EffectInfo>(EffectInfos);
        infos.Sort((x, y) => (x.StartTime + x.Douration).CompareTo(y.StartTime + y.Douration));
        StartCoroutine(CloseSequnceEffect(infos));
    }

    private IEnumerator CloseSequnceEffect(List<EffectInfo> infos)
    {
        var lastWait = 0f;
        foreach (var effectInfo in infos)
        {
            var curWait = effectInfo.StartTime + effectInfo.Douration;
            yield return new WaitForSeconds(curWait - lastWait);
            if (cloneObjects.ContainsKey(effectInfo))
            {
                NGUITools.Destroy(cloneObjects[effectInfo]);
            }
            lastWait = curWait;
        }
        infos.Clear();
    }

    private IEnumerator PlaySequnceEffect()
    {
        cloneObjects.Clear();
        for(int index = 0; index < EffectInfos.Count; index++)
        {
            yield return
                new WaitForSeconds(index == 0
                                       ? EffectInfos[index].StartTime
                                       : EffectInfos[index].StartTime - EffectInfos[index - 1].StartTime);
            var effectInfo = EffectInfos[index];
            var effectObj = effectInfo.EffectObj;
            var clone = NGUITools.AddChild(gameObject, effectObj);
            NGUITools.SetActive(clone, false);
            clone.transform.localPosition = effectInfo.PositonFrom;
            NGUITools.SetActive(clone.gameObject, true);
            if(!cloneObjects.ContainsKey(effectInfo))
            {
                cloneObjects.Add(effectInfo, clone);
                PlayEffect(effectInfo, true);
                if (effectInfo.UseMove)
                {
                    iTween.MoveTo(clone,
                                  iTween.Hash("position", effectInfo.PositionTo,
                                              "delay", effectInfo.MoveDelay,
                                              "time", effectInfo.MoveDouration,
                                              "islocal", effectInfo.IsLocal));
                }
                if(effectInfo.UseRotation)
                {
                    iTween.RotateTo(clone,
                                    iTween.Hash("rotation", effectInfo.RotationTo,
                                                "delay", effectInfo.RotationDelay,
                                                "time", effectInfo.RoationDouration,
                                                "islocal", effectInfo.IsLocal));
                }
            }
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        DestoryClones();
    }

    private void DestoryClones()
    {
        var clones = cloneObjects.Values.ToList();
        for(var i = clones.Count - 1; i >= 0; i--)
        {
            var clone = clones[i];
            NGUITools.Destroy(clone);
        }
        cloneObjects.Clear();
    }

    private void PlayEffect(EffectInfo effectInfo, bool play)
    {
        if(cloneObjects.ContainsKey(effectInfo))
        {
            var effectObject = cloneObjects[effectInfo];
            if (effectInfo.EffectType == EffectType.Particle)
            {
                var renderQ = effectObject.GetComponent<SetRenderQueue>() ?? effectObject.AddComponent<SetRenderQueue>();
                renderQ.RenderQueue = RenderQ;
                var particles = effectObject.GetComponentsInChildren<ParticleSystem>();
                foreach (var particle in particles)
                {
                    if (play)
                    {
                        particle.Play();
                    }
                    else
                    {
                        particle.Stop();
                    }
                }
            }
        }
    }
}
