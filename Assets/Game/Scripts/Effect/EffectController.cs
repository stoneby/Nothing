using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public float Duration;

    private List<ParticleSystem> particleList;
    private List<Animation> animationList;

    public void Play(bool loop)
    {
        particleList.ForEach(item =>
        {
            item.loop = loop;
            item.Play();
        });
        animationList.ForEach(item =>
        {
            item.wrapMode = (loop) ? WrapMode.Loop : WrapMode.Once;
            item.Play();
        });
    }

    public void Stop()
    {
        particleList.ForEach(item => item.Stop());
        animationList.ForEach(item => item.Stop());
    }

    private float GetDuration()
    {
        var max = particleList.Select(particle => particle.duration).Concat(new[] {float.MinValue}).Max();
        return max;
    }

    private void Awake()
    {
        particleList = new List<ParticleSystem>();
        particleList.AddRange(GetComponentsInChildren<ParticleSystem>());

        animationList = new List<Animation>();
        animationList.AddRange(GetComponentsInChildren<Animation>());

        Duration = GetDuration();
    }
}
