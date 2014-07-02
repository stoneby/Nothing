#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Simulate : MonoBehaviour
{
    public bool PlayOnce;

    private float sampleTime;
    private double lastTime;
    private float duration;

    [HideInInspector]
    public bool IsPlaying;

    private readonly Dictionary<ParticleSystem, bool> psLoopStatusKeeper = new Dictionary<ParticleSystem, bool>();
    private readonly Dictionary<Animation, WrapMode> anLoopStatusKeeper = new Dictionary<Animation, WrapMode>(); 

    private void DoUpdate()
    {
        Play();
    }

    public void Reset()
    {
        sampleTime = 0f;
        lastTime = EditorApplication.timeSinceStartup;
        duration = GetDuration();
        GetLoopStatus();
    }

    public void RegisterEvent()
    {
        IsPlaying = true;
        EditorApplication.update += DoUpdate;
    }

    public void UnregisterEvent()
    {
        IsPlaying = false;
        EditorApplication.update -= DoUpdate;
    }

    public void Play()
    {
        var deltaTime = (float)(EditorApplication.timeSinceStartup - lastTime);
        lastTime = EditorApplication.timeSinceStartup;
        sampleTime += deltaTime;

        if (PlayOnce && sampleTime > duration)
        {
            UnregisterEvent();
            Stop();
            return;
        }
        Sample(sampleTime);
    }

    public void Stop()
    {
        Sample(0f);
        SetLoopStatus();
    }

    private void Sample(float time)
    {
        var pss = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in pss)
        {
            ps.loop = true;
            ps.Simulate(time);
        }

        var anis = GetComponentsInChildren<Animation>(true);
        foreach (var an in anis)
        {
            an.wrapMode = WrapMode.Loop;
            an.gameObject.SampleAnimation(an.clip, time);
        }
    }

    private float GetDuration()
    {
        duration = 0f;
        var pss = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in pss)
        {
            duration = Mathf.Max(duration, ps.duration);
        }
        Debug.Log("Duration is: " + duration);
        return duration;
    }

    private void GetLoopStatus()
    {
        var pss = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in pss)
        {
            psLoopStatusKeeper[ps] = ps.loop;
            Debug.Log("Get status ps: " + ps.name + ", loop: " + ps.loop);
        }

        var anis = GetComponentsInChildren<Animation>(true);
        foreach (var an in anis)
        {
            anLoopStatusKeeper[an] = an.wrapMode;
        }
    }

    private void SetLoopStatus()
    {
        var pss = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in pss)
        {
            ps.loop = psLoopStatusKeeper[ps];
            Debug.Log("Set status ps: " + ps.name + ", loop: " + ps.loop);
        }

        var anis = GetComponentsInChildren<Animation>(true);
        foreach (var an in anis)
        {
            an.wrapMode = anLoopStatusKeeper[an];
        }
    }
}
#endif