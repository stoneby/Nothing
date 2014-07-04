using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PaticleDelay : MonoBehaviour
{
    public bool PlayOnce;
    //[HideInInspector]
    //public bool IsPlaying;
    public float StartDelay;
    private float sampleTime;
    private double lastTime;
    private double startTime;
    private float duration;

    private readonly List<ParticleSystem> particleDelays = new List<ParticleSystem>();

    private readonly Dictionary<ParticleSystem, bool> psLoopStatusKeeper = new Dictionary<ParticleSystem, bool>();
    private readonly Dictionary<ParticleSystem, float> psDelaysKeeper = new Dictionary<ParticleSystem, float>();
    //private readonly Dictionary<ParticleSystem, bool> psPlayAwakeKeeper = new Dictionary<ParticleSystem, bool>();
    private float delta;

    public void Reset()
    {
        Stop();
        sampleTime = 0f;
#if UNITY_EDITOR
        lastTime = EditorApplication.timeSinceStartup;
        startTime = EditorApplication.timeSinceStartup;
#endif
        duration = GetDuration();
        GetSortedPss();
        GetDelays();
        GetLoopStatus();
    }

    private void ResetPlayOnAwake()
    {
        foreach (var delay in particleDelays)
        {
            delay.playOnAwake = false;
            delay.Stop();
        }
    }

    private void Awake()
    {
        GetSortedPss();
        ResetPlayOnAwake();
        GetDelays();
        duration = GetDuration();
        PlayOnRun();
    }

    public void PlayOnRun()
    {
        foreach (var delay in particleDelays)
        {
            delay.startDelay = psDelaysKeeper[delay] + delta;
            delay.Play();
        }
    }

    public void StopOnRun()
    {
        foreach (var delay in particleDelays)
        {
            delay.Stop();
        }
    }

    private void GetDelays()
    {
        foreach (var delay in particleDelays)
        {
            psDelaysKeeper[delay] = delay.startDelay;
        }
    }


    private void SetDelays()
    {
        foreach (var delay in psDelaysKeeper)
        {
            delay.Key.startDelay = delay.Value;
        }
    }

    private void GetSortedPss()
    {
        particleDelays.Clear();
        var pss = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var system in pss)
        {
            particleDelays.Add(system);
        }
        particleDelays.Sort((p, q) => p.startDelay.CompareTo(q.startDelay));
        delta = StartDelay - particleDelays[0].startDelay;
    }


    public void Play()
    {
#if UNITY_EDITOR
        if ((float)(EditorApplication.timeSinceStartup) < startTime + delta)
        {
            lastTime = EditorApplication.timeSinceStartup;
            return;
        }
        var deltaTime = (float)(EditorApplication.timeSinceStartup - lastTime);
        lastTime = EditorApplication.timeSinceStartup;
        sampleTime += deltaTime;
#endif

        if (PlayOnce && sampleTime > duration)
        {
#if UNITY_EDITOR
            UnregisterEvent();
#endif
            Stop();
            return;
        }
        Sample(sampleTime);
    }

    public void Stop()
    {
        Sample(0f);
        SetLoopStatus();
        SetDelays();
    }

    private void Sample(float time)
    {
        foreach (var ps in particleDelays)
        {
            ps.loop = true;
            ps.Simulate(time);
        }
    }

    private float GetDuration()
    {
        duration = 0f;
        foreach (var ps in particleDelays)
        {
            duration = Mathf.Max(duration, ps.duration);
        }
        Debug.Log("Duration is: " + duration);
        return duration;
    }

    private void GetLoopStatus()
    {
        foreach (var ps in particleDelays)
        {
            psLoopStatusKeeper[ps] = ps.loop;
        }
    }

    private void SetLoopStatus()
    {
        foreach (var ps in particleDelays)
        {
            ps.loop = psLoopStatusKeeper[ps];
        }
    }

    private void DoUpdate()
    {
        Play();
    }

    public void RegisterEvent()
    {
#if UNITY_EDITOR
        //IsPlaying = true;
        EditorApplication.update += DoUpdate;
#endif
    }

    public void UnregisterEvent()
    {
#if UNITY_EDITOR
        //IsPlaying = false;
        EditorApplication.update -= DoUpdate;
#endif
    }
}
