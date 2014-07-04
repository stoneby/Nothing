using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    /// <summary>
    /// Buff type
    /// </summary>
    /// <remarks>1 based according to server logic.</remarks>
    public enum BuffType
    {
        Poison = 0,
        Palsy,
        Sleep,
        Burn,
        Freeze,
        Petrify,
        Seal,
        Delay,
        IncreaseData,
        Relieve
    }

    public List<BuffDisplayer> BuffDisplayerList;
    public List<Color> BuffColorList;

    public void ShowIntroduce(BuffType type, GameObject parent)
    {
        var buffDisplayer = BuffDisplayerList[(int)type];
        buffDisplayer.ShowIntro(parent);
    }

    public void ShowLoop(BuffType type, GameObject parent)
    {
        var buffDisplayer = BuffDisplayerList[(int)type];
        buffDisplayer.ShowLoop(parent);
    }

    public void Show(BuffType type, GameObject parent)
    {
        var buffDisplayer = BuffDisplayerList[(int)type];
        buffDisplayer.Show(parent);
    }

    public void Stop(BuffType type, GameObject parent)
    {
        var buffDisplayer = BuffDisplayerList[(int)type];
        buffDisplayer.Stop(parent);
    }

    private void Awake()
    {
        if (BuffDisplayerList == null)
        {
            Logger.LogError("BuffList should not be null.");
            return;
        }

        var buffTypeSize = Enum.GetNames(typeof(BuffType)).Count();
        var buffDisplayerCount = BuffDisplayerList.Count;
        if (buffTypeSize != buffDisplayerCount)
        {
            Logger.LogError("Buff type size from BuffType: " + buffTypeSize + " should be the same as BuffDisplayerList count: " + buffDisplayerCount);
        }
    }
}
